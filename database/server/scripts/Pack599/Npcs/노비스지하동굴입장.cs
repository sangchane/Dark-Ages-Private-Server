using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 노비스지하동굴입장 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_노비스지하동굴입장", "5.99표")]
    public class NpcB178BE44C2A4C9C0D558B3D9AD74C785C7A5 : PackNpc
    {
        public NpcB178BE44C2A4C9C0D558B3D9AD74C785C7A5(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_name1_s = 0;
            V v_name2_s = 0;
            V v_name3_s = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            L_re: ;
            if (V.T(((V)(p.Call("get_level", v_myid)) >= (V)((V)22L))))
            {
                yield return Mes((V)0L, (V)"강한힘이 입장을 거부하고 있다.");
                yield break;
            }
            yield return Menu((V)"※던전※\\n노비스 지하동굴로 입장 하시겠습니까?\\n그룹원이 있다면 같이 이동됩니다.", (V)"입장한다", (V)"입장하지 않는다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("get_mapname", v_myid)) != (V)((V)"노비스지하던전B2"))))
                {
                    yield return Mes((V)0L, (V)"알수 없는 에러!");
                    yield break;
                }
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                v_name1_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"노비스지하동굴1"));
                v_name2_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"노비스지하동굴2"));
                v_name3_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"노비스지하동굴3"));
                p.Call("map_create", p["$map_num"], v_name1_s, (V)"노비스지하동굴1", (V)21L, (V)22L, (V)140L, (V)2L, (V)1L, (V)"db/maps/던전/lod001.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name2_s, (V)"노비스지하동굴2", (V)22L, (V)20L, (V)140L, (V)2L, (V)2L, (V)"db/maps/던전/lod002.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name3_s, (V)"노비스지하동굴3", (V)22L, (V)25L, (V)140L, (V)2L, (V)3L, (V)"db/maps/던전/lod004.map");
                p.Call("warp_create", (V)0L, v_name1_s, (V)20L, (V)12L, v_name2_s, (V)3L, (V)5L, (V)11L, (V)22L, (V)0L);
                p.Call("warp_create", (V)0L, v_name2_s, (V)20L, (V)4L, v_name3_s, (V)3L, (V)19L, (V)11L, (V)22L, (V)0L);
                p.Call("item_clear", v_name1_s);
                p.Call("mob_clear", v_name1_s);
                p.Call("item_clear", v_name2_s);
                p.Call("mob_clear", v_name2_s);
                p.Call("item_clear", v_name3_s);
                p.Call("mob_clear", v_name3_s);
                p.Call("group_warp", v_name1_s, (V)4L, (V)13L);
                p.Call("group_message", (V)3L, (V)"{=c※던전※ 노비스지하동굴에 입장하셧습니다.");
                p.Call("message", (V)3L, (V)"{=c※던전※ 노비스지하동굴에 입장하셧습니다.");
                p.Call("mob_spawn3", (V)"동굴지네", v_name1_s, (V)40L, (V)48L, (V)1600L, (V)7L);
                p.Call("mob_spawn3", (V)"동굴지네", v_name2_s, (V)40L, (V)48L, (V)1600L, (V)7L);
                p.Call("mob_spawn3", (V)"동굴지네", v_name3_s, (V)40L, (V)48L, (V)1600L, (V)5L);
                p.Call("mob_spawn3", (V)"동굴쥐", v_name1_s, (V)38L, (V)45L, (V)1800L, (V)7L);
                p.Call("mob_spawn3", (V)"동굴쥐", v_name2_s, (V)38L, (V)45L, (V)1800L, (V)7L);
                p.Call("mob_spawn3", (V)"동굴쥐", v_name3_s, (V)38L, (V)45L, (V)1800L, (V)5L);
                p.Call("mob_spawn3", (V)"고대의좀비", v_name3_s, (V)50L, (V)70L, (V)5000L, (V)1L);
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
