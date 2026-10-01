#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.ServerFormats;
using static Darkages.Types.Companions;

#endregion

namespace Darkages.Types
{
    /// <summary>동료 봇의 몸과 짐 — 레벨 맞춤(능력치·마법), 기본 장비, 주인이 주고 벗기는 장비·포션, 봇 장비창 알림.</summary>
    public static class CompanionKit
    {
        /// <summary>
        /// 봇을 <paramref name="level" /> 의 성직자로 만든다 — 레벨 1(Aisling.Create: 체력 150 · 마력 200 · 힘10 인트5 위즈5 콘5
        /// 덱스5)에서 레벨마다 원작 식(최대 체력 += 콘+30, 최대 마력 += 위즈+25 — <c>Monster.Levelup</c>)을 밟고, 레벨마다 받는
        /// 능력치 2점(<c>StatsPerLevel</c>)은 모두 위즈에 넣는다(회복량이 위즈에서 나온다 — 쿠로 = 위즈×8). 체력·마력은 가득.
        /// 마법은 <see cref="Companions.PriestSpells" /> 중 그 레벨까지 배울 수 있는 것만 남긴다. 옷은 성직자로 만들 때 입은 기본 옷 그대로다.
        /// </summary>
        public static void Prepare(Aisling bot, int level)
        {
            level = Math.Max(1, Math.Min(level, ServerContext.Config.PlayerLevelCap > 0 ? ServerContext.Config.PlayerLevelCap : 99));

            bot.Path = Class.Priest;
            bot._Str = 10;
            bot._Int = 5;
            bot._Wis = 5;
            bot._Con = 5;
            bot._Dex = 5;
            bot._MaximumHp = 150;
            bot._MaximumMp = 200;
            bot.StatPoints = 0;

            long total = 0;
            for (var at = 1; at < level; at++)
            {
                bot._MaximumHp += bot._Con + Monster.HpPerLevel;
                bot._MaximumMp += bot._Wis + Monster.MpPerLevel;
                bot._Wis = (byte) Math.Min(bot._Wis + ServerContext.Config.StatsPerLevel, ServerContext.Config.StatCap);
                total += ExperienceCurve.ToReach(at + 1);
            }

            bot.ExpLevel = level;
            bot.ExpTotal = (uint) Math.Min(uint.MaxValue, Math.Max(1, total));
            bot.ExpNext = Math.Max(1, ExperienceCurve.ToReach(level + 1));
            bot.CurrentHp = bot.MaximumHp;
            bot.CurrentMp = bot.MaximumMp;

            var wanted = PriestSpells.Where(s => s.Level <= level).Select(s => s.Name).ToList();

            foreach (var spell in bot.SpellBook.Spells.Values
                         .Where(s => s?.Template != null && !wanted.Contains(s.Template.Name)).ToList())
            {
                bot.SpellBook.Remove((byte) spell.Slot);
                bot.Client?.Send(new ServerFormat18((byte) spell.Slot));
            }

            foreach (var name in wanted)
                Spell.GiveTo(bot, name, 1);

            Dress(bot);
            bot.Client?.SendStats(StatusFlags.All);
        }

        // ── 기본 장비 ────────────────────────────────────────────────────────

        /// <summary>
        /// 성직자 직업 의상(5.99 <c>성직자방어구</c>) — 레벨 요구 차례, 남·여. 그림 번호 5·10·15·20·25 는 원작 <c>skill.tbl</c> 성직자
        /// 동작 줄의 ST(그 동작을 할 수 있는 옷)에 든다 — 직업 동작(128 주문 자세)은 직업 의상을 입어야 원작이 그린다.
        /// </summary>
        private static readonly (int Level, string Male, string Female)[] Robes =
        {
            (1, "셍즈", "로브"),
            (11, "레더로브", "미스틱로브"),
            (41, "맨틀", "엘레맨틀"),
            (71, "위저드로브", "홀리로브"),
            (99, "네크로브", "매직로브")
        };

        /// <summary>기본 무기 — 홀리파나(성직자무기, 레벨 11). 그 아래 레벨은 입을 수 없어 홀리마르시아(레벨 1).</summary>
        private static string Staff(int level) => level >= 11 ? "홀리파나" : "홀리마르시아";

        private const string GivenKey = "companion.given";

        /// <summary>
        /// 기본 장비를 입힌다: 무기·갑옷 자리가 비었거나 서버가 입힌 기본 것이면 지금 레벨의 것으로 바꾼다. 주인이 입혀 준 것
        /// (<see cref="GivenKey" /> 에 적힌 serial)은 건드리지 않는다. 서버가 입힌 것은 바꿀 때 없앤다 — 되돌려 받을 수 없으므로 부를
        /// 때마다 공짜 장비가 생기지 않는다.
        /// </summary>
        private static void Dress(Aisling bot)
        {
            if (bot.Client == null)
                return;

            var robe = Robes.Last(r => r.Level <= bot.ExpLevel);
            Wear(bot, ItemSlots.Weapon, Staff(bot.ExpLevel));
            Wear(bot, ItemSlots.Armor, bot.Gender == Gender.Female ? robe.Female : robe.Male);
        }

        private static void Wear(Aisling bot, byte place, string name)
        {
            var current = bot.EquipmentManager.Equipment[place]?.Item;

            if (current != null && (Given(bot).Contains(current.Serial) || current.Template?.Name == name))
                return;

            var item = Item.Create(bot, name);
            if (item == null)
            {
                ServerContext.Logger($"봇 기본 장비 {name} 템플릿이 없습니다.");
                return;
            }

            if (current != null)
                bot.EquipmentManager.TakeOff(place);

            bot.EquipmentManager.AddEquipment(place, item, false);
        }

        // ── 주인이 봇에게 주기·벗기기 ────────────────────────────────────────

        /// <summary>
        /// 주인 가방 한 칸을 봇에게. 장비면 원작 착용 규칙(레벨·직업·성별·내구 — <c>GameClient.CheckReqs</c> 와 같은 조건, 봇 기준)을
        /// 보고 입히고, 봇이 입던 것은 주인 가방(방금 빈 칸)으로 — 서버가 입힌 기본 것이면 없앤다. 겹치는 물건(포션)이면
        /// <paramref name="count" /> 개(0 은 다)를 봇 가방으로.
        /// </summary>
        public static void Give(Aisling owner, byte slot, int count)
        {
            if (owner?.Client == null || CompanionOf(owner.Username) is not { } name || FindOnline(name) is not { } bot)
            {
                owner?.Client?.SendMessage(0x02, "함께 있는 봇이 없습니다.");
                return;
            }

            var item = owner.Inventory.FindInSlot(slot);
            if (item?.Template == null)
                return;

            if (item.Template.Flags.HasFlag(ItemFlags.Stackable))
                GivePotions(owner, bot, item, count);
            else if (item.Template.Flags.HasFlag(ItemFlags.Equipable) && item.Template.EquipmentSlot > 0)
                GiveGear(owner, bot, item);
            else
                owner.Client.SendMessage(0x02, "봇에게 줄 수 없는 물건입니다.");

            SendKit(bot, owner);
        }

        /// <summary>봇의 장비 한 자리를 주인 가방으로. 가방이 꽉 차면 거절. 서버가 입힌 기본 장비는 벗기지 않는다.</summary>
        public static void TakeOff(Aisling owner, byte place)
        {
            if (owner?.Client == null || CompanionOf(owner.Username) is not { } name || FindOnline(name) is not { } bot)
            {
                owner?.Client?.SendMessage(0x02, "함께 있는 봇이 없습니다.");
                return;
            }

            var item = bot.EquipmentManager[place]?.Item;
            if (item == null)
                return;

            if (!Given(bot).Contains(item.Serial))
            {
                owner.Client.SendMessage(0x02, "봇의 기본 장비는 벗길 수 없습니다.");
                return;
            }

            if (owner.Inventory.FindEmpty() == byte.MaxValue)
            {
                owner.Client.SendMessage(0x02, "가방이 가득 차 벗길 수 없습니다.");
                return;
            }

            bot.EquipmentManager.TakeOff(place);
            SetGiven(bot, Given(bot).Where(serial => serial != item.Serial));
            item.GiveTo(owner, false);
            Dress(bot);
            bot.Client.Save();
            owner.Client.Save();
            SendKit(bot, owner);
        }

        private static void GiveGear(Aisling owner, Aisling bot, Item item)
        {
            var why = CannotWear(bot, item);
            if (why != null)
            {
                owner.Client.SendMessage(0x02, why);
                return;
            }

            var place = PlaceFor(bot, item.Template.EquipmentSlot);
            owner.Inventory.Remove(owner.Client, item);
            owner.CurrentWeight = Math.Max(0, owner.CurrentWeight - item.Template.CarryWeight);
            owner.Client.SendStats(StatusFlags.StructA);

            var old = bot.EquipmentManager.TakeOff(place);
            var given = Given(bot).ToList();

            if (old != null && given.Remove(old.Serial))
                old.GiveTo(owner, false);

            bot.EquipmentManager.AddEquipment(place, item, false);
            given.Add(item.Serial);
            SetGiven(bot, given);

            owner.Client.SendMessage(0x02, $"봇에게 {item.Template.Name}을(를) 입혔습니다.");
            bot.Client.Save();
            owner.Client.Save();
        }

        /// <summary>
        /// 두 짝을 끼는 것(반지 왼손·오른손, 장갑 왼팔·오른팔)은 템플릿이 정한 쪽이 차 있고 다른 쪽이 비었으면 다른 쪽에 끼운다 —
        /// 템플릿은 한쪽만 적어 두어(반지 7·8, 장갑 9·10) 봇에게 한 짝만 입혀졌다(사용자, 2026-09-27). 둘 다 차 있으면 정한 쪽을 바꾼다.
        /// </summary>
        private static int PlaceFor(Aisling bot, int place)
        {
            var other = place switch
            {
                ItemSlots.LHand => ItemSlots.RHand,
                ItemSlots.RHand => ItemSlots.LHand,
                ItemSlots.LArm => ItemSlots.RArm,
                ItemSlots.RArm => ItemSlots.LArm,
                _ => 0
            };

            bool Worn(int at) => bot.EquipmentManager[(byte) at]?.Item != null;

            return other != 0 && Worn(place) && !Worn(other) ? other : place;
        }

        private static void GivePotions(Aisling owner, Aisling bot, Item item, int count)
        {
            var have = Math.Max(1, (int) item.Stacks);
            var n = count <= 0 ? have : Math.Min(count, have);
            var roomy = bot.Inventory.Get(i => i != null && i.Template.Name == item.Template.Name &&
                                                i.Stacks + n <= i.Template.MaxStack).Any() ||
                        bot.Inventory.FindEmpty() != byte.MaxValue;

            if (!roomy)
            {
                owner.Client.SendMessage(0x02, "봇의 가방이 가득 찼습니다.");
                return;
            }

            // 봇에게 들어가지 못하면 주인 것도 줄이지 않는다 — 전에는 주인 가방에서 먼저 빼고 넣기 결과를 버려,
            // 넣기가 실패하면 포션이 사라졌다(코드 리뷰 2026-10-02).
            if (n >= have)
            {
                // 통째로 줄 때는 같은 물건을 넘긴다. Remove 가 지금 칸 번호를 쓰므로 먼저 빼고, 실패하면 주인에게 되돌린다.
                owner.Inventory.Remove(owner.Client, item);

                if (!item.GiveTo(bot, false))
                {
                    item.GiveTo(owner, false);
                    owner.Client.SendMessage(0x02, "봇에게 주지 못했습니다.");
                    return;
                }

                owner.CurrentWeight = Math.Max(0, owner.CurrentWeight - item.Template.CarryWeight);
                owner.Client.SendStats(StatusFlags.StructA);
            }
            else
            {
                var part = Item.Create(bot, item.Template);

                if (part == null)
                {
                    owner.Client.SendMessage(0x02, "봇에게 주지 못했습니다.");
                    return;
                }

                part.Stacks = (ushort) n;

                if (!part.GiveTo(bot, false))
                {
                    owner.Client.SendMessage(0x02, "봇에게 주지 못했습니다.");
                    return;
                }

                owner.Inventory.RemoveRange(owner.Client, item, n);
            }

            owner.Client.SendMessage(0x02, $"봇에게 {item.Template.Name} {n}개를 주었습니다.");
            bot.Client.Save();
            owner.Client.Save();
        }

        /// <summary>원작 착용 규칙(<c>GameClient.CheckReqs</c>)을 봇 기준으로. 입을 수 있으면 null, 아니면 주인에게 보일 까닭.</summary>
        public static string CannotWear(Aisling bot, Item item)
        {
            var template = item.Template;

            if (bot.ExpLevel < template.LevelRequired)
                return $"봇의 레벨({bot.ExpLevel})이 모자랍니다 — {template.Name}은(는) {template.LevelRequired}레벨부터.";

            if (template.Class != Class.Peasant && template.Class != bot.Path)
                return $"{template.Name}은(는) 성직자가 입을 수 없습니다.";

            if (template.Gender != Gender.Both && template.Gender != bot.Gender)
                return $"{template.Name}은(는) 봇의 성별에 맞지 않습니다.";

            if (item.Durability <= 0)
                return $"{template.Name}은(는) 고쳐야 입을 수 있습니다.";

            return null;
        }

        private static List<int> Given(Aisling bot) =>
            bot.PackVariables != null && bot.PackVariables.TryGetValue(GivenKey, out var text) && !string.IsNullOrEmpty(text)
                ? text.Split(',').Select(part => int.TryParse(part, out var serial) ? serial : 0).Where(s => s != 0).ToList()
                : new List<int>();

        // 사전을 고친 사본으로 통째로 바꿔 끼운다 — 자동 저장이 이 사전을 훑는 중에 고치면 안 된다(Pack599 와 같은 방식).
        private static void SetGiven(Aisling bot, IEnumerable<int> serials)
        {
            var copy = new Dictionary<string, string>(bot.PackVariables ?? new Dictionary<string, string>())
            {
                [GivenKey] = string.Join(",", serials)
            };
            bot.PackVariables = copy;
        }

        /// <summary>봇이 입은 장비와 든 포션을 주인에게(앱의 봇 장비창).</summary>
        internal static void SendKit(Aisling bot, Aisling owner)
        {
            var worn = bot.EquipmentManager.Equipment
                .Where(pair => pair.Value?.Item?.Template != null)
                .Select(pair => ((byte) pair.Key, pair.Value.Item))
                .ToList();
            var carried = bot.Inventory.Items.Values
                .Where(item => item?.Template != null && item.Template.Flags.HasFlag(ItemFlags.Stackable))
                .ToList();

            owner.Client?.Send(ServerFormat5E.Kit(bot.Serial, worn, carried));
        }
    }
}
