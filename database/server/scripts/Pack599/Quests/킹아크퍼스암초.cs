using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 킹아크퍼스암초 — 아벨해안 킹아크퍼스 퀘스트(3서클 이벤트, 사용자 2026-10-11, <c>autopilot/king-arcfus-quest/SPEC.md</c>). 아벨해안4-C 바위 틈
    /// (10,39)을 밟으면 도는 대본이다(워프 <c>ScriptNpc</c>, 생성기 <c>scripts/gen/world/build-king-arcfus-quest.py</c>). 밟은 사람이 대지의인장·
    /// 해저의팬던트를 가졌으면 그룹(같은 맵)을 전용 사본으로 보낸다: 외딴길(코마밭) → 숨겨진길A → 숨겨진길B → 본거지.
    /// 원한의에센스가 있으면 진짜 킹아크퍼스의본거지(미레 — 누를 때 에센스를 쓴다), 없으면 가짜 거대문어의본거지(문어다리).
    /// 원작은 무도가가 자리에 서야 하고 5직업이 자리를 맞췄다 — 사용자 「5인 아니어도 진행 가능하도록」으로 직업·인원을 보지 않는다.
    /// </summary>
    /// <remarks>사본 단계 40 — 던전 스크립트(Dungeon__Script)는 0~23 단계만 다뤄 손님을 내보내지 않는다(에리얼과 같음).</remarks>
    [Script("NPC_킹아크퍼스암초", "킹아크퍼스퀘스트")]
    public class NpcKingArcfusReef : PackNpc
    {
        public const string Reef = "아벨해안4-C";
        public const string Waiting = "아벨해안대기실";
        public const long WaitX = 20, WaitY = 17;
        public const long Stage = 40;
        public const long Music = 142;

        // 코마밭 — 외딴길(원작 lod4747) 모래길 위 28칸, 줄마다 한두 칸. 밟으면 체력이 1이 된다(킹아크퍼스코마.cs).
        public static readonly (long X, long Y)[] Coma =
        {
            (10, 5), (14, 7), (11, 9), (10, 11), (12, 13), (9, 15), (10, 17), (13, 19), (17, 21), (10, 21), (10, 23), (18, 23), (9, 25), (23, 25),
            (25, 27), (20, 29), (20, 31), (20, 33), (20, 35), (25, 37), (25, 39), (24, 41), (16, 43), (23, 45), (22, 47), (24, 49), (21, 51), (26, 53)
        };

        public static readonly string[] Minions = { "문어", "애스코모이드", "슬러그2", "일2" };
        public const long MinionsEach = 2;     // 방마다 종마다 — 8마리

        public NpcKingArcfusReef(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = p.Call("get_myid");
            if (V.T((V)(p.Call("get_mapname", v_myid)) != (V)Reef))
                yield break;
            if (!V.T(p.Call("item_exist", v_myid, (V)"대지의인장")) || !V.T(p.Call("item_exist", v_myid, (V)"해저의팬던트")))
            {
                p.Call("message", (V)3L, (V)"{=c※킹아크퍼스※ 바위 틈이 희미하게 울립니다. 대지의인장과 해저의팬던트가 있어야 길이 열립니다.");
                yield break;
            }

            var real = V.T(p.Call("item_exist", v_myid, (V)"원한의에센스"));
            V trail = (V)"킹아크퍼스" + p.Call("get_name") + (V)"외딴길";
            V roadA = (V)"킹아크퍼스" + p.Call("get_name") + (V)"숨겨진길A";
            V roadB = (V)"킹아크퍼스" + p.Call("get_name") + (V)"숨겨진길B";
            // 가짜·진짜 본거지는 원본 맵이 달라 사본 이름도 나눈다 — 같은 이름이면 Instances.Create 가 있던 사본을 그대로 돌려준다.
            V lair = (V)"킹아크퍼스" + p.Call("get_name") + (V)(real ? "킹아크퍼스의본거지" : "거대문어의본거지");
            p.Call("map_create", (V)0L, trail, (V)"아벨신해안외딴길", (V)40L, (V)60L, (V)Music, (V)Stage, (V)1L, (V)"db/maps/kingarcfus/lod4747.map");
            p.Call("map_create", (V)0L, roadA, (V)"신해안의숨겨진길A", (V)14L, (V)14L, (V)Music, (V)Stage, (V)2L, (V)"db/maps/kingarcfus/lod4748.map");
            p.Call("map_create", (V)0L, roadB, (V)"신해안의숨겨진길B", (V)13L, (V)13L, (V)Music, (V)Stage, (V)3L, (V)"db/maps/kingarcfus/lod4750.map");
            if (real)
                p.Call("map_create", (V)0L, lair, (V)"킹아크퍼스의본거지", (V)12L, (V)12L, (V)Music, (V)Stage, (V)4L, (V)"db/maps/kingarcfus/lod4749.map");
            else
                p.Call("map_create", (V)0L, lair, (V)"거대문어의본거지", (V)20L, (V)20L, (V)Music, (V)Stage, (V)4L, (V)"db/maps/kingarcfus/lod4758.map");

            // 외딴길 북쪽 끝 → 숨겨진길A · A 남동 끝 → B · B 남동 끝 → 본거지(잡몹을 다 잡아야 지나감).
            foreach (var x in new[] { 9L, 11L, 12L, 13L, 14L })
                p.Call("warp_create", (V)0L, trail, (V)x, (V)0L, roadA, (V)1L, (V)1L, (V)1L, (V)99L, (V)0L);
            foreach (var x in new[] { 11L, 12L, 13L })
                p.Call("warp_create", (V)0L, roadA, (V)x, (V)13L, roadB, (V)1L, (V)1L, (V)1L, (V)99L, (V)1L);
            foreach (var (x, y) in new[] { (11L, 12L), (12L, 12L), (12L, 11L) })
                p.Call("warp_create", (V)0L, roadB, (V)x, (V)y, lair, real ? (V)1L : (V)5L, real ? (V)9L : (V)5L, (V)1L, (V)99L, (V)1L);
            foreach (var (x, y) in Coma)
                p.Call("warp_create", (V)3L, trail, (V)x, (V)y, (V)"킹아크퍼스코마", (V)0L, (V)0L, (V)1L, (V)99L, (V)0L);

            foreach (var map in new[] { trail, roadA, roadB, lair })
            {
                p.Call("item_clear", map);
                p.Call("mob_clear", map);
            }
            foreach (var minion in Minions)
            {
                p.Call("mob_spawn3", (V)minion, roadA, (V)0L, (V)0L, (V)0L, (V)MinionsEach);
                p.Call("mob_spawn3", (V)minion, roadB, (V)0L, (V)0L, (V)0L, (V)MinionsEach);
            }
            if (real)
            {
                // 에센스는 미레를 누를 때 쓴다(SPEC) — 가는 길에 죽거나 끊겨도 잃지 않는다.
                p.Call("mob_spawn3", (V)"킹아크퍼스", lair, (V)0L, (V)0L, (V)0L, (V)1L, (V)8L, (V)4L);
                p.Call("npc_spawn", (V)"미레", lair, (V)10L, (V)10L);
            }
            else
            {
                p.Call("mob_spawn3", (V)"거대문어", lair, (V)0L, (V)0L, (V)0L, (V)1L, (V)14L, (V)14L);
                foreach (var (x, y) in new[] { (18L, 18L), (19L, 18L), (18L, 19L), (19L, 19L) })
                    p.Call("warp_create", (V)0L, lair, (V)x, (V)y, (V)Waiting, (V)WaitX, (V)WaitY, (V)1L, (V)99L, (V)1L);
            }
            p.Call("group_warp", trail, (V)21L, (V)57L);
            p.Call("group_message", (V)3L, (V)"{=c※킹아크퍼스※ 바위 틈이 열리며 외딴길로 빨려 들어갔습니다. 죽음의 그림자를 조심해 북쪽 끝으로 가세요.");
            yield break;
        }
    }
}
