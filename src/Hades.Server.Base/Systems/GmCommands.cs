using Darkages.Network.Game;
using Darkages.Storage.locales.Buffs;
using Darkages.Types;
using Pyratron.Frameworks.Commands.Parser;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Darkages.Systems.CLI
{
    /// <summary>
    /// 한글 운영자 명령(사용자 2026-10-11 「아이템 생성은 /item 아이템이름 이런식으로 직관적으로 · /운영자 ? 로 도움말」 + 고른 것 캐릭터 조정·운영자 몸).
    /// 이름은 따옴표 없이 쳐도 되고, 꼭 맞는 이름이 없으면 그 글자가 든 이름이 하나뿐일 때 그것을, 여럿이면 후보를 알려 준다.
    /// 영어 명령(`/give` `/tp` …)은 <see cref="Commander" /> 에 그대로 있다. 운영자만 — 대화 처리(<c>Format0EHandler</c>)가 가른다.
    /// </summary>
    public static class GmCommands
    {
        /// <summary>무적·숨기를 「켜 둠」으로 거는 시간 — 다시 치면 푼다.</summary>
        private const int Forever = 24 * 60 * 60;

        public static readonly string Help = string.Join("\n",
            "운영자 명령 (이름에 따옴표는 안 붙여도 됩니다)",
            "/item 이름 [개수] — 아이템 만들기 (예: /item 세줄금반지 · /item 상급체력포션 50)",
            "/이동 맵이름 [x y] — 그 맵으로 (좌표를 빼면 가운데)",
            "/가기 캐릭터 — 그 사람 곁으로 · /소환 캐릭터 — 그 사람을 내 곁으로",
            "/괴물 이름 [마리] — 내 곁에 괴물 세우기",
            "/기술 이름 · /마법 이름 — 바로 배우기",
            "/회복 — 체력·마력 가득 · /레벨 숫자 — 내 레벨 바꾸기 · /돈 금액 — 금화 받기",
            "/무적 · /숨기 — 켜기, 다시 치면 끄기",
            "맵 이동 칸·월드맵의 레벨 제한은 운영자에게 걸리지 않습니다.");

        /// <summary>운영자인가 — 설정 `GameMasters`(캐릭터 이름) 또는 `GameMasterKakaoIds`(그 카카오에 묶인 캐릭터 모두).</summary>
        public static bool IsOperator(string username, string kakaoId) =>
            (username != null && (ServerContext.Config.GameMasters?.Any(n => string.Equals(n, username, StringComparison.OrdinalIgnoreCase)) ?? false))
            || (!string.IsNullOrEmpty(kakaoId) && (ServerContext.Config.GameMasterKakaoIds?.Contains(kakaoId) ?? false));

        public static void Register(CommandParser parser)
        {
            parser.AddCommand(Command.Create("Operator Help").AddAlias("운영자").AddAlias("도움말").SetAction(OnHelp)
                .AddArgument(Argument.Create("what").MakeOptional().SetDefault("?")));
            parser.AddCommand(Command.Create("Item").AddAlias("item").AddAlias("아이템").SetAction(OnItem)
                .AddArgument(Argument.Create("name"))
                .AddArgument(Argument.Create("amount").MakeOptional().SetDefault(1)));
            parser.AddCommand(Command.Create("Go To Map").AddAlias("이동").SetAction(OnMove)
                .AddArgument(Argument.Create("map"))
                .AddArgument(Argument.Create("x").MakeOptional().SetDefault(-1))
                .AddArgument(Argument.Create("y").MakeOptional().SetDefault(-1)));
            parser.AddCommand(Command.Create("Spawn Monster").AddAlias("괴물").SetAction(OnMonster)
                .AddArgument(Argument.Create("name"))
                .AddArgument(Argument.Create("count").MakeOptional().SetDefault(1)));
            parser.AddCommand(Command.Create("Heal").AddAlias("회복").SetAction(OnHeal));
            parser.AddCommand(Command.Create("Set Level").AddAlias("레벨").SetAction(OnLevel).AddArgument(Argument.Create("level")));
            parser.AddCommand(Command.Create("Give Gold").AddAlias("돈").SetAction(OnGold).AddArgument(Argument.Create("amount")));
            parser.AddCommand(Command.Create("Invincible").AddAlias("무적").SetAction(OnInvincible));
            parser.AddCommand(Command.Create("Hide").AddAlias("숨기").SetAction(OnHide));
        }

        /// <summary>
        /// 이름 고르기 — 꼭 맞는 것, 아니면 그 글자가 든 것이 하나뿐일 때 그것. 못 고르면 null 과 알릴 말(후보 다섯까지).
        /// </summary>
        public static (string Name, string Refusal) Pick(IEnumerable<string> names, string typed, string what)
        {
            var all = names.Where(n => !string.IsNullOrEmpty(n)).Distinct().ToList();
            if (all.Contains(typed))
                return (typed, null);
            var near = all.Where(n => n.Contains(typed, StringComparison.OrdinalIgnoreCase)).OrderBy(n => n.Length).ThenBy(n => n).ToList();
            if (near.Count == 1)
                return (near[0], null);
            return near.Count == 0
                ? (null, $"그런 {what}이(가) 없습니다: {typed}")
                : (null, $"{what} 후보 {near.Count}개 — {string.Join(", ", near.Take(5))}{(near.Count > 5 ? " …" : "")}");
        }

        private static string Text(Argument[] args, string name) => args.FromName(name)?.Replace("\"", "").Trim() ?? "";

        private static void OnHelp(Argument[] args, object arg) => (arg as GameClient)?.SendMessage(0x08, Help);

        private static void OnItem(Argument[] args, object arg)
        {
            if (arg is not GameClient client)
                return;
            var (name, refusal) = Pick(ServerContext.GlobalItemTemplateCache.Keys, Text(args, "name"), "아이템");
            if (name == null)
            {
                client.SendMessage(0x02, refusal);
                return;
            }
            if (!int.TryParse(Text(args, "amount"), out var count) || count < 1)
                count = 1;
            var given = Commander.GiveItem(client.Aisling, ServerContext.GlobalItemTemplateCache[name], Math.Min(count, 1000));
            client.SendMessage(0x02, given < Math.Min(count, 1000) ? $"{name} {given}개를 만들었습니다(가방이 찼습니다)." : $"{name} {given}개를 만들었습니다.");
        }

        private static void OnMove(Argument[] args, object arg)
        {
            if (arg is not GameClient client)
                return;
            var (name, refusal) = Pick(ServerContext.GlobalMapCache.Values.Select(m => m.Name), Text(args, "map"), "맵");
            var area = name == null ? null : ServerContext.GlobalMapCache.Values.FirstOrDefault(m => m.Name == name);
            if (area == null)
            {
                client.SendMessage(0x02, refusal);
                return;
            }
            int.TryParse(Text(args, "x"), out var x);
            int.TryParse(Text(args, "y"), out var y);
            var to = x >= 0 && y >= 0 && x < area.Cols && y < area.Rows ? new Position(x, y) : OpenNear(area, area.Cols / 2, area.Rows / 2);
            client.TransitionToMap(area, to);
        }

        /// <summary>(x,y)에서 가장 가까운 벽 아닌 칸 — 맵 가운데가 바다·바위일 때.</summary>
        private static Position OpenNear(Area area, int x, int y)
        {
            for (var ring = 0; ring < Math.Max(area.Cols, area.Rows); ring++)
                for (var dx = -ring; dx <= ring; dx++)
                for (var dy = -ring; dy <= ring; dy++)
                {
                    int cx = x + dx, cy = y + dy;
                    if (Math.Max(Math.Abs(dx), Math.Abs(dy)) == ring && cx >= 0 && cy >= 0 && cx < area.Cols && cy < area.Rows && !area.IsWall(cx, cy))
                        return new Position(cx, cy);
                }
            return new Position(x, y);
        }

        private static void OnMonster(Argument[] args, object arg)
        {
            if (arg is not GameClient client)
                return;
            var me = client.Aisling;
            var (name, refusal) = Pick(ServerContext.GlobalMonsterTemplateCache.Select(t => t.Name), Text(args, "name"), "괴물");
            if (name == null)
            {
                client.SendMessage(0x02, refusal);
                return;
            }
            var template = ServerContext.GlobalMonsterTemplateCache.First(t => t.Name == name);
            if (!int.TryParse(Text(args, "count"), out var count) || count < 1)
                count = 1;
            var made = 0;
            for (var i = 0; i < Math.Min(count, 50); i++)
            {
                Monster monster = null;
                for (var attempt = 0; attempt < 20 && monster == null; attempt++)
                    monster = Monster.Create(template, me.Map);
                if (monster == null)
                    continue;
                var spot = me.Map.FreeSpotNear(new Position(me.X, me.Y + 1));
                monster.XPos = spot.X;
                monster.YPos = spot.Y;
                me.AddObject(monster);
                made++;
            }
            client.SendMessage(0x02, $"{name} {made}마리를 세웠습니다.");
        }

        private static void OnHeal(Argument[] args, object arg)
        {
            if (arg is not GameClient client)
                return;
            client.Aisling.CurrentHp = client.Aisling.MaximumHp;
            client.Aisling.CurrentMp = client.Aisling.MaximumMp;
            client.SendStats(StatusFlags.StructB);
        }

        private static void OnLevel(Argument[] args, object arg)
        {
            if (arg is not GameClient client || !int.TryParse(Text(args, "level"), out var level))
                return;
            level = Math.Clamp(level, 1, 99);
            client.Aisling.ExpLevel = level;
            client.Aisling.ExpNext = level >= 99 ? 0 : Math.Max(1, ExperienceCurve.ToReach(level + 1));
            client.SendStats(StatusFlags.All);
            client.SendMessage(0x02, $"레벨을 {level}(으)로 바꿨습니다.");
        }

        private static void OnGold(Argument[] args, object arg)
        {
            if (arg is not GameClient client || !long.TryParse(Text(args, "amount"), out var amount) || amount <= 0)
                return;
            client.Aisling.GoldPoints = (int) Math.Max(client.Aisling.GoldPoints, Math.Min(ServerContext.Config.MaxCarryGold, (long) client.Aisling.GoldPoints + amount));
            client.SendStats(StatusFlags.StructC);
            client.SendMessage(0x02, $"금화 {client.Aisling.GoldPoints:N0}");
        }

        private static void OnInvincible(Argument[] args, object arg)
        {
            if (arg is GameClient client)
                Toggle(client, new buff_dion(), "무적");
        }

        private static void OnHide(Argument[] args, object arg)
        {
            if (arg is GameClient client)
                Toggle(client, new buff_hide(), "숨기");
        }

        /// <summary>
        /// 켜져 있으면 풀고, 아니면 하루짜리로 건다(디온 = 맞아도 안 죽음 · 하이드 = 남에게 안 보임, 기존 상태 그대로). 켜졌는지는 효과 목록이 아니라
        /// 캐릭터 값(<c>Immunity</c>·<c>Invisible</c>)으로 본다 — 효과 목록은 저장되지 않아, 값만 남은 채 들어오면 「켬」이 먼저 나왔다(리뷰).
        /// </summary>
        private static void Toggle(GameClient client, Buff fresh, string what)
        {
            var me = client.Aisling;
            var on = fresh is buff_dion ? me.Immunity : me.Invisible;
            if (on || me.Buffs.ContainsKey(fresh.Name))
            {
                if (me.Buffs.TryGetValue(fresh.Name, out var buff))
                    buff.OnEnded(me, buff);
                me.Immunity = fresh is buff_dion ? false : me.Immunity;
                me.Invisible = fresh is buff_hide ? false : me.Invisible;
                client.UpdateDisplay();
                client.SendMessage(0x02, $"{what} 끔");
                return;
            }
            fresh.Timer.Tick = fresh.Length - Forever;
            fresh.OnApplied(me, fresh);
            client.SendMessage(0x02, $"{what} 켬 — 다시 치면 끕니다.");
        }
    }
}
