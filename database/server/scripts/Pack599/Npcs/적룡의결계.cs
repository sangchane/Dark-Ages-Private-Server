using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 적룡의결계 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_적룡의결계", "5.99표")]
    public class NpcC801B8E1C758ACB0ACC4 : PackNpc
    {
        public NpcC801B8E1C758ACB0ACC4(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_name12_s = 0;

            v_myid = p.Call("get_myid");
            v_name12_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"적룡의결계"));
            p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
            p.Call("map_create", p["$map_num"], v_name12_s, (V)"적룡의결계", (V)40L, (V)40L, (V)191L, (V)9L, (V)8L, (V)"db/maps/직업퀘스트/lod7012.map");
            p.Call("group_warp", v_name12_s, (V)20L, (V)23L);
            p.Call("mob_clear", v_name12_s);
            p.Call("item_clear", v_name12_s);
            p.Call("mob_spawn2", (V)"봉인의주인(봉)", (V)18L, (V)13L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)1000L)));
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
