#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 동료 봇 — 사람을 따라다니며 회복·버프를 걸어 주는 성직자(사용자 결정 2026-09-26: 봇 프로그램이 성직자 캐릭터로
    /// 접속해 있고, 앱의 [동료 부르기] 단추(0xF1)로 부르며, 봇은 부른 사람 레벨에 맞춰 준비된다). 봇의 판단은 봇 프로그램
    /// (<c>mobile/bots/Lod.CompanionBot</c>)이 하고, 서버는 짝을 맺고 풀고, 봇을 준비시키고, 맵이 갈리면 옆으로 옮길 뿐이다.
    /// </summary>
    public class Companions : ObjectManager
    {
        /// <summary>
        /// 봇이 받는 성직자 회복·버프 마법과 그 레벨 — 5.99 사범 NPC 메뉴의 레벨이다(<c>Npc_Skill.txt</c>: 소라카
        /// "신성력강화[11] 쿠러스[11] 호르라마[15]", 소라카2 "에나르마[21] … 쿠라노[21]", "쿠라노소[55] 쿠라누스[63]",
        /// "수페라쿠라노[83] 쿠라네라[87] … 엑스쿠라노[99] 엑스쿠라네라[99]"). 쿠로는 직업을 고를 때 받는다(<c>Npc_Quest.txt:219</c>).
        /// 해제 둘(디나르콜리 = 수면 sleep, 디소루마 = 빙결 frozen — 스크립트의 mobnar_end·mobsor_end)도 준다.
        /// 공격(홀리볼트)·무적(이모탈)은 봇의 일이 아니라 주지 않는다.
        /// </summary>
        public static readonly (int Level, string Name)[] PriestSpells =
        {
            (1, "쿠로"),
            (11, "신성력강화"),
            (11, "쿠러스"),
            (15, "호르라마"),
            (21, "에나르마"),
            (21, "쿠라노"),
            (21, "디나르콜리"),
            (21, "디소루마"),
            (55, "쿠라노소"),
            (63, "쿠라누스"),
            (83, "수페라쿠라노"),
            (87, "쿠라네라"),
            (99, "엑스쿠라노"),
            (99, "엑스쿠라네라")
        };

        /// <summary>같은 맵에서 이만큼 넘게 떨어지면(막혀서 못 따라오면) 옆으로 옮긴다 — 화면(시야) 밖이다.</summary>
        public const int CatchUpDistance = 12;

        private static readonly object Gate = new object();

        // 봇 이름 → 부른 사람 이름. 둘 다 접속해 있는 동안만 산다.
        private static readonly Dictionary<string, string> OwnerOf =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        private static readonly Companions Finder = new Companions();

        /// <summary>봇 레벨 = 부른 사람 − 2, 적어도 1 (사용자 결정).</summary>
        public static int LevelFor(int ownerLevel) => Math.Max(1, ownerLevel - 2);

        public static bool IsBot(string name) =>
            ServerContext.Config.CompanionBots?.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)) ?? false;

        /// <summary>이 사람이 부른 봇의 이름, 없으면 null.</summary>
        public static string CompanionOf(string owner)
        {
            lock (Gate)
            {
                return OwnerOf.FirstOrDefault(p => string.Equals(p.Value, owner, StringComparison.OrdinalIgnoreCase)).Key;
            }
        }

        /// <summary>[봇 부르기]. 알림은 모두 부른 사람에게 한국어 한 줄로.</summary>
        public static void Call(Aisling caller)
        {
            if (caller?.Client == null || IsBot(caller.Username))
                return;

            Aisling bot;

            lock (Gate)
            {
                if (OwnerOf.Values.Any(o => string.Equals(o, caller.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    caller.Client.SendMessage(0x02, "이미 봇이 함께 있습니다.");
                    return;
                }

                if (caller.GroupParty != null && !caller.LeaderPrivileges)
                {
                    caller.Client.SendMessage(0x02, "그룹장만 봇을 부를 수 있습니다.");
                    return;
                }

                var online = (ServerContext.Config.CompanionBots ?? new List<string>())
                    .Select(FindOnline)
                    .Where(one => one != null && !one.Dead)
                    .ToList();

                bot = online.FirstOrDefault(one => !OwnerOf.ContainsKey(one.Username));

                if (bot == null)
                {
                    caller.Client.SendMessage(0x02, online.Any()
                        ? "봇이 모두 다른 분과 함께 있습니다."
                        : "지금 부를 수 있는 봇이 없습니다.");
                    return;
                }

                OwnerOf[bot.Username] = caller.Username;
            }

            if (bot.GroupParty != null)
                Party.RemovePartyMember(bot);

            Prepare(bot, LevelFor(caller.ExpLevel));
            MoveBeside(bot, caller);
            Party.AddPartyMember(caller, bot);

            bot.Client.Send(new ServerFormat5E(ServerFormat5E.Master, caller.Serial, caller.Username));
            caller.Client.Send(new ServerFormat5E(ServerFormat5E.Companion, bot.Serial, bot.Username));
            SendKit(bot, caller);
            caller.Client.SendMessage(0x02, $"봇 {bot.Username}님이 왔습니다 (Lv{bot.ExpLevel})");
        }

        /// <summary>[봇 보내기]. 파티에서 빼고 마을로.</summary>
        public static void Dismiss(Aisling caller)
        {
            if (caller?.Client == null)
                return;

            var name = CompanionOf(caller.Username);
            var bot = name == null ? null : FindOnline(name);

            if (name == null)
            {
                caller.Client.SendMessage(0x02, "함께 있는 봇이 없습니다.");
                return;
            }

            Release(name, bot, caller);
            caller.Client.SendMessage(0x02, $"봇 {name}님을 보냈습니다.");
        }

        /// <summary>
        /// 1초마다(<c>CompanionComponent</c>): 부른 사람이 나갔으면 봇을 보내고, 봇이 나갔으면 알리고, 맵이 갈렸거나 멀리
        /// 떨어졌으면 봇을 옆으로 옮긴다 — 봇은 워프 칸을 모르므로 주인이 워프로 사라지면 서버가 데려간다.
        /// </summary>
        public static void Tick()
        {
            KeyValuePair<string, string>[] pairs;

            lock (Gate)
            {
                pairs = OwnerOf.ToArray();
            }

            foreach (var (botName, ownerName) in pairs)
            {
                var bot = FindOnline(botName);
                var owner = FindOnline(ownerName);

                if (bot == null || owner == null)
                {
                    if (owner != null)
                        owner.Client.SendMessage(0x02, $"봇 {botName}님이 떠났습니다.");

                    Release(botName, bot, owner);
                    continue;
                }

                bot.Client.Send(ServerFormat5E.Status(owner.Serial, StatusesOf(owner)));
                bot.Client.Send(ServerFormat5E.Status(bot.Serial, StatusesOf(bot)));
                owner.Client.Send(ServerFormat5E.Life(bot.Serial, Percent(bot.CurrentHp, bot.MaximumHp),
                    Percent(bot.CurrentMp, bot.MaximumMp)));

                if (owner.Client.IsWarping || owner.Client.MapOpen || owner.Map == null)
                    continue;

                if (bot.CurrentMapId != owner.CurrentMapId ||
                    bot.Position.DistanceFrom(owner.Position) > CatchUpDistance)
                    MoveBeside(bot, owner);
            }
        }

        /// <summary>
        /// 봇을 <paramref name="level" /> 의 성직자로 만든다 — 레벨 1(Aisling.Create: 체력 150 · 마력 200 · 힘10 인트5 위즈5 콘5
        /// 덱스5)에서 레벨마다 원작 식(최대 체력 += 콘+30, 최대 마력 += 위즈+25 — <c>Monster.Levelup</c>)을 밟고, 레벨마다 받는
        /// 능력치 2점(<c>StatsPerLevel</c>)은 모두 위즈에 넣는다(회복량이 위즈에서 나온다 — 쿠로 = 위즈×8). 체력·마력은 가득.
        /// 마법은 <see cref="PriestSpells" /> 중 그 레벨까지 배울 수 있는 것만 남긴다. 옷은 성직자로 만들 때 입은 기본 옷 그대로다.
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

        /// <summary>봇을 사람 곁 빈칸(남·동·서·북 차례)으로. 빈칸이 없으면 서버가 가까운 빈터를 찾는다.</summary>
        private static void MoveBeside(Aisling bot, Aisling owner)
        {
            var map = owner.Map;
            if (map == null || bot.Client == null)
                return;

            var spot = new[] { (0, 1), (1, 0), (-1, 0), (0, -1) }
                .Select(d => new Position(owner.XPos + d.Item1, owner.YPos + d.Item2))
                .FirstOrDefault(p => p.X < map.Cols && p.Y < map.Rows && !map.IsWall(p.X, p.Y) &&
                                     !Finder.GetObjects(map, s => s.XPos == p.X && s.YPos == p.Y,
                                         Get.Aislings | Get.Monsters | Get.Mundanes).Any())
                       ?? owner.Position;

            bot.Client.TransitionToMap(map, spot);
        }

        /// <summary>짝을 푼다: 파티에서 빼고, 봇이 접속해 있으면 마을로 보내고, 둘에게 끝났다고 알린다.</summary>
        private static void Release(string botName, Aisling bot, Aisling owner)
        {
            lock (Gate)
            {
                OwnerOf.Remove(botName);
            }

            owner?.Client?.Send(new ServerFormat5E(ServerFormat5E.Companion, 0, string.Empty));

            if (bot?.Client == null)
                return;

            if (bot.GroupParty != null)
                Party.RemovePartyMember(bot);

            var homeMap = ServerContext.Config.CompanionHomeMap > 0
                ? ServerContext.Config.CompanionHomeMap
                : ServerContext.Config.StartingMap;
            var home = ServerContext.Config.CompanionHomePosition ?? ServerContext.Config.StartingPosition;

            bot.Client.TransitionToMap(homeMap, new Position(home.X, home.Y));
            bot.Client.Send(new ServerFormat5E(ServerFormat5E.Master, 0, string.Empty));
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

            var place = item.Template.EquipmentSlot;
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

            if (n >= have)
            {
                owner.Inventory.Remove(owner.Client, item);
                owner.CurrentWeight = Math.Max(0, owner.CurrentWeight - item.Template.CarryWeight);
                owner.Client.SendStats(StatusFlags.StructA);
                item.GiveTo(bot, false);
            }
            else
            {
                owner.Inventory.RemoveRange(owner.Client, item, n);
                var part = Item.Create(bot, item.Template);
                part.Stacks = (ushort) n;
                part.GiveTo(bot, false);
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

        // ── 알림 ────────────────────────────────────────────────────────────

        /// <summary>하데스 버프·디버프와 5.99 시간 상태 — 이름·남은 초·해로움.</summary>
        private static List<(string Name, int Seconds, bool Harmful)> StatusesOf(Sprite who)
        {
            var listed = new List<(string, int, bool)>();
            listed.AddRange(who.Buffs.Values.Where(b => b != null).Select(b => (b.Name, b.Length - b.Timer.Tick, false)));
            listed.AddRange(who.Debuffs.Values.Where(d => d != null).Select(d => (d.Name, d.Length - d.Timer.Tick, true)));
            listed.AddRange(TimedStates.Of(who).Select(s => (s.Name, s.Seconds, false)));
            return listed.Take(byte.MaxValue).ToList();
        }

        private static void SendKit(Aisling bot, Aisling owner)
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

        private static byte Percent(int value, int maximum) =>
            (byte) (maximum > 0 ? Math.Clamp(100L * value / maximum, 0, 100) : 0);

        private static Aisling FindOnline(string name) =>
            Finder.GetObjects<Aisling>(null, a => a != null && a.LoggedIn && a.Client != null &&
                                                  string.Equals(a.Username, name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
    }
}
