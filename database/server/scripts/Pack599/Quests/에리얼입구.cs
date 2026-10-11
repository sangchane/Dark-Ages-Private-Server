using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 에리얼입구 — 뤼케시온해안 에리얼 퀘스트(4서클 이벤트, 사용자 2026-10-11, <c>autopilot/aerial-quest/SPEC.md</c>). 손으로 쓴 대본이다
    /// (5.99 팩에는 없다 — 원작 블로그를 보고 포테의숲오솔길입장 틀로 짰다). 말 건 사람의 그룹(같은 맵)을 전용 사본 셋으로 보낸다:
    /// 게길(원작 lod6627) → 대기실 → 에리얼의방(둘 다 3-D 사본). 게는 <c>Monsters/AerialCrab.cs</c>, 보상은 <c>마레.cs</c>.
    /// </summary>
    /// <remarks>
    /// 사본 단계 40 — 던전 스크립트(Dungeon__Script)는 0~23 단계만 다뤄, 에리얼을 잡은 뒤 마레를 누르기 전에 내보내지 않는다.
    /// </remarks>
    [Script("NPC_에리얼입구", "에리얼퀘스트")]
    public class NpcAerialEntrance : PackNpc
    {
        public const string Shore = "뤼케시온해안3-D";
        public const long Stage = 40;

        // 게 자리 — 게길 길(0~7줄) 위에 줄을 바꿔 가며 15마리. 아무 데나 세우면 시작 칸(1,6) 곁에 서서 들어서자마자 털리기도 했다(리뷰 2026-10-11).
        public static readonly (long X, long Y)[] Crabs = { (8, 2), (12, 5), (16, 7), (20, 3), (24, 6), (28, 1), (32, 4), (36, 2), (40, 5), (44, 7), (48, 3), (52, 6), (56, 1), (60, 4), (64, 2) };

        public NpcAerialEntrance(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = p.Call("get_myid");
            yield return Menu((V)"※에리얼※\\n해안의 에리얼이 날뛰어 마레가 붙잡혀 있습니다.\\n게길에서 게에게 닿으면 제물(블루하콘무기·레드하콘다리 한 벌)을 빼앗깁니다. 킹아크퍼스의팬던트가 있으면 제물이 필요 없습니다.\\n그룹원이 있다면 같이 이동됩니다.",
                (V)"입장한다", (V)"입장하지 않는다");
            if (reply.Choice != 1)
            {
                yield return Mes((V)0L, (V)"신중히 고려해 주세요.");
                yield break;
            }
            if (V.T((V)(p.Call("get_mapname", v_myid)) != (V)Shore))
            {
                yield return Mes((V)0L, (V)"알수 없는 에러!");
                yield break;
            }
            if (!V.T(p.Call("item_exist", v_myid, (V)"킹아크퍼스의팬던트"))
                && (!V.T(p.Call("item_exist", v_myid, (V)"블루하콘무기")) || !V.T(p.Call("item_exist", v_myid, (V)"레드하콘다리"))))
            {
                yield return Mes((V)0L, (V)"제물(블루하콘무기·레드하콘다리)이 한 벌도 없습니다. 뤼케시온해안의 하콘에게서 구해 오세요.");
                yield break;
            }

            V trail = (V)"에리얼" + p.Call("get_name") + (V)"게길";
            V lobby = (V)"에리얼" + p.Call("get_name") + (V)"대기실";
            V room = (V)"에리얼" + p.Call("get_name") + (V)"의방";
            p.Call("map_create", (V)0L, trail, (V)"게길", (V)70L, (V)30L, (V)173L, (V)Stage, (V)1L, (V)"db/maps/aerial/lod6627.map");
            p.Call("map_create", (V)0L, lobby, (V)"에리얼대기실", (V)80L, (V)50L, (V)173L, (V)Stage, (V)2L, (V)"db/maps/aerial/lod6625.map");
            p.Call("map_create", (V)0L, room, (V)"에리얼의방", (V)80L, (V)50L, (V)173L, (V)Stage, (V)3L, (V)"db/maps/aerial/lod6625.map");
            // 게길 동쪽 끝 → 대기실 · 대기실 동쪽 문 → 에리얼의방.
            for (var y = 5L; y <= 8; y++)
                p.Call("warp_create", (V)0L, trail, (V)69L, (V)y, lobby, (V)20L, (V)30L, (V)1L, (V)99L, (V)0L);
            for (var y = 28L; y <= 31; y++)
                p.Call("warp_create", (V)0L, lobby, (V)45L, (V)y, room, (V)20L, (V)30L, (V)1L, (V)99L, (V)0L);
            foreach (var map in new[] { trail, lobby, room })
            {
                p.Call("item_clear", map);
                p.Call("mob_clear", map);
            }
            foreach (var (x, y) in Crabs)
                p.Call("mob_spawn3", (V)"게", trail, (V)0L, (V)0L, (V)0L, (V)1L, (V)x, (V)y);
            p.Call("mob_spawn3", (V)"에리얼", room, (V)0L, (V)0L, (V)0L, (V)1L, (V)45L, (V)30L);
            p.Call("npc_spawn", (V)"마레", room, (V)48L, (V)27L);
            p.Call("group_warp", trail, (V)1L, (V)6L);
            p.Call("group_message", (V)3L, (V)"{=c※에리얼※ 게길에 들어섰습니다. 게를 피해 동쪽 끝으로 가세요.");
            yield break;
        }
    }
}
