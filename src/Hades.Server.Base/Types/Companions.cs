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
        /// 공격(홀리볼트)·해제(디나르콜리·디소루마)·무적(이모탈)은 동료의 일이 아니라 주지 않는다.
        /// </summary>
        public static readonly (int Level, string Name)[] PriestSpells =
        {
            (1, "쿠로"),
            (11, "신성력강화"),
            (11, "쿠러스"),
            (15, "호르라마"),
            (21, "에나르마"),
            (21, "쿠라노"),
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

        /// <summary>[동료 부르기]. 알림은 모두 부른 사람에게 한국어 한 줄로.</summary>
        public static void Call(Aisling caller)
        {
            if (caller?.Client == null || IsBot(caller.Username))
                return;

            Aisling bot;

            lock (Gate)
            {
                if (OwnerOf.Values.Any(o => string.Equals(o, caller.Username, StringComparison.OrdinalIgnoreCase)))
                {
                    caller.Client.SendMessage(0x02, "이미 동료가 함께 있습니다.");
                    return;
                }

                if (caller.GroupParty != null && !caller.LeaderPrivileges)
                {
                    caller.Client.SendMessage(0x02, "그룹장만 동료를 부를 수 있습니다.");
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
                        ? "동료가 모두 다른 분과 함께 있습니다."
                        : "지금 부를 수 있는 동료가 없습니다.");
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
            caller.Client.SendMessage(0x02, $"동료 {bot.Username}님이 왔습니다. (레벨 {bot.ExpLevel})");
        }

        /// <summary>[동료 보내기]. 파티에서 빼고 마을로.</summary>
        public static void Dismiss(Aisling caller)
        {
            if (caller?.Client == null)
                return;

            var name = CompanionOf(caller.Username);
            var bot = name == null ? null : FindOnline(name);

            if (name == null)
            {
                caller.Client.SendMessage(0x02, "함께 있는 동료가 없습니다.");
                return;
            }

            Release(name, bot, caller);
            caller.Client.SendMessage(0x02, $"동료 {name}님을 보냈습니다.");
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
                        owner.Client.SendMessage(0x02, $"동료 {botName}님이 떠났습니다.");

                    Release(botName, bot, owner);
                    continue;
                }

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

        private static Aisling FindOnline(string name) =>
            Finder.GetObjects<Aisling>(null, a => a != null && a.LoggedIn && a.Client != null &&
                                                  string.Equals(a.Username, name, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();
    }
}
