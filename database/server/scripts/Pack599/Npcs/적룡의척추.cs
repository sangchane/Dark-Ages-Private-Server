using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 적룡의척추 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_적룡의척추", "5.99표")]
    public class NpcC801B8E1C758CC99CD94 : PackNpc
    {
        public NpcC801B8E1C758CC99CD94(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_name11_s = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("map_objmob", v_myid)) >= (V)((V)1L))))
            {
                p.Call("message", (V)3L, (V)"몬스터가 남아있습니다.");
                yield break;
            }
            v_name11_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"적룡의척추"));
            p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
            p.Call("map_create", p["$map_num"], v_name11_s, (V)"적룡의척추", (V)50L, (V)150L, (V)191L, (V)0L, (V)0L, (V)"db/maps/직업퀘스트/lod7011.map");
            p.Call("warp_create", (V)3L, v_name11_s, (V)23L, (V)3L, (V)"적룡의결계", (V)0L, (V)0L, (V)99L, (V)99L, (V)0L);
            p.Call("warp_create", (V)3L, v_name11_s, (V)24L, (V)3L, (V)"적룡의결계", (V)0L, (V)0L, (V)99L, (V)99L, (V)0L);
            p.Call("group_warp", v_name11_s, (V)27L, (V)145L);
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
