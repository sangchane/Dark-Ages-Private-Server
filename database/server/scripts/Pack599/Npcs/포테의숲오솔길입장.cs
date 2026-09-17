using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 포테의숲오솔길입장 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_포테의숲오솔길입장", "5.99표")]
    public class NpcD3ECD14CC758C232C624C194AE38C785C7A5 : PackNpc
    {
        public NpcD3ECD14CC758C232C624C194AE38C785C7A5(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_name4_s = 0;
            V v_name5_s = 0;
            V v_name6_s = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            L_re: ;
            if (V.T(((V)(p.Call("get_level", v_myid)) >= (V)((V)52L))))
            {
                p.Call("message", (V)3L, (V)"강한힘이 입장을 거부하고 있다.");
                yield break;
            }
            yield return Menu((V)"※던전※\\n포테의숲 오솔길로 입장 하시겠습니까?\\n그룹원이 있다면 같이 이동됩니다.", (V)"입장한다", (V)"입장하지 않는다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("get_mapname", v_myid)) != (V)((V)"포테의숲5존"))))
                {
                    yield return Mes((V)0L, (V)"알수 없는 에러!");
                    yield break;
                }
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                v_name4_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"포테의숲오솔길"));
                v_name5_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"포테의숲오솔길대기실"));
                v_name6_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"포테의숲오솔길보스존"));
                p.Call("map_create", p["$map_num"], v_name4_s, (V)"포테의숲오솔길", (V)20L, (V)40L, (V)145L, (V)3L, (V)1L, (V)"db/maps/default/maps/lod4612.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name5_s, (V)"포테의숲오솔길대기실", (V)10L, (V)10L, (V)145L, (V)3L, (V)2L, (V)"db/maps/default/maps/lod4613.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name6_s, (V)"포테의숲오솔길보스존", (V)20L, (V)20L, (V)145L, (V)3L, (V)3L, (V)"db/maps/default/maps/lod4615.map");
                p.Call("warp_create", (V)0L, v_name4_s, (V)9L, (V)0L, v_name5_s, (V)4L, (V)9L, (V)21L, (V)52L, (V)0L);
                p.Call("warp_create", (V)0L, v_name5_s, (V)4L, (V)0L, v_name6_s, (V)10L, (V)19L, (V)21L, (V)52L, (V)1L);
                p.Call("item_clear", v_name4_s);
                p.Call("mob_clear", v_name4_s);
                p.Call("item_clear", v_name5_s);
                p.Call("mob_clear", v_name5_s);
                p.Call("item_clear", v_name6_s);
                p.Call("mob_clear", v_name6_s);
                p.Call("group_warp", v_name4_s, (V)9L, (V)39L);
                p.Call("group_message", (V)3L, (V)"{=c※던전※ 포테의숲오솔길에 입장하셧습니다.");
                p.Call("message", (V)3L, (V)"{=c※던전※ 포테의숲오솔길에 입장하셧습니다.");
                p.Call("mob_spawn3", (V)"사나운은빛늑대", v_name5_s, (V)150L, (V)160L, (V)4500L, (V)6L);
                p.Call("mob_spawn3", (V)"사나운은빛늑대", v_name6_s, (V)150L, (V)160L, (V)4500L, (V)8L);
                p.Call("mob_spawn3", (V)"자이언트맨티스", v_name6_s, (V)220L, (V)240L, (V)15000L, (V)1L);
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)0L, (V)"신중히 고려해 주세요.");
                    yield break;
                }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
