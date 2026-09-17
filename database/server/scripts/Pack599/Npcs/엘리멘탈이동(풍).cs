using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 엘리멘탈이동(풍) — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_엘리멘탈이동(풍)", "5.99표")]
    public class NpcC5D8B9ACBA58D0C8C774B3D90028D48D0029 : PackNpc
    {
        public NpcC5D8B9ACBA58D0C8C774B3D90028D48D0029(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_name9_s = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("map_objmob", v_myid)) >= (V)((V)1L))))
            {
                p.Call("message", (V)3L, (V)"몬스터가 남아있습니다.");
                yield break;
            }
            v_name9_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"엘리멘탈(풍)"));
            p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
            p.Call("map_create", p["$map_num"], v_name9_s, (V)"엘리멘탈(풍)", (V)25L, (V)25L, (V)191L, (V)9L, (V)7L, (V)"db/maps/직업퀘스트/lod7007.map");
            p.Call("warp_create", (V)3L, v_name9_s, (V)1L, (V)9L, (V)"엘리멘탈이동(화)", (V)0L, (V)0L, (V)99L, (V)99L, (V)0L);
            p.Call("mob_clear", v_name9_s);
            p.Call("item_clear", v_name9_s);
            p.Call("group_warp", v_name9_s, (V)13L, (V)9L);
            p.Call("mob_spawn2", (V)"바람의엘리멘탈", (V)3L, (V)9L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)25L)));
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
