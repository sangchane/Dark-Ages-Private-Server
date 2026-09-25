using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 워프할아버지 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_워프할아버지", "5.99표")]
    public class NpcC6CCD504D560C544BC84C9C0 : PackNpc
    {
        public NpcC6CCD504D560C544BC84C9C0(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_name1_s = 0;
            V v_name2_s = 0;
            V v_name3_s = 0;
            V v_name4_s = 0;
            V v_name5_s = 0;
            V v_name6_s = 0;
            V v_select = 0;
            V v_select2 = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            v_select2 = (V)0L;
            L_re: ;
            yield return Menu((V)"용건이 무엇인가?", (V)"적룡플라밋의굴대기실 이동", (V)"직업퀘스트진행(그룹이동)");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"적룡플라밋의굴대기실로 이동하겠나? 요금은 1만골드라네.");
                L_re2: ;
                yield return Menu((V)"요금을 내고 적룡플라밋의굴로 이동하겠나?", (V)"이동한다.", (V)"이동하지 않는다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)0L))))
                {
                    goto L_re2;
                }
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)10000L))))
                    {
                        yield return Mes((V)0L, (V)"골드가 부족하다네.");
                        yield break;
                    }
                    p.Call("warp", (V)"적룡플라밋의굴대기실", (V)15L, (V)15L);
                    p.Call("money_del", (V)10000L);
                    yield return Mes((V)0L, (V)"이동이 완료되었다네.");
                    yield break;
                }
                else
                    if (V.T(((V)(v_select2) == (V)((V)2L))))
                    {
                        yield break;
                    }
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"그룹원들과 함께하는 그룹퀘스트라네. 일명 결계라고도 하지.");
                yield return Mes((V)1L, (V)"입장시 그룹원들과 바로입장되며, 입장비로는 입장신청자가 7만골드를 내면 된다네.");
                L_re3: ;
                yield return Menu((V)"어떤가? 7만골드를 당신이 지불하고 입장하겠나?\\n(그룹원이 5명 이상이여야 입장가능)", (V)"입장한다.", (V)"입장하지 않는다.");
                v_select2 = reply.Choice;
                p.Call("money_del", (V)70000L);
                v_name1_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"직업퀘스트대기실"));
                v_name2_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"직업퀘스트(전사)"));
                v_name3_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"직업퀘스트(도적)"));
                v_name4_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"직업퀘스트(무도가)"));
                v_name5_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"직업퀘스트(마법사)"));
                v_name6_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"직업퀘스트(성직자)"));
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name1_s, (V)"직업퀘스트대기실", (V)35L, (V)20L, (V)185L, (V)9L, (V)1L, (V)"db/maps/직업퀘스트/lod7000.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name2_s, (V)"직업퀘스트(전사)", (V)15L, (V)15L, (V)185L, (V)9L, (V)2L, (V)"db/maps/직업퀘스트/lod7001.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name3_s, (V)"직업퀘스트(도적)", (V)15L, (V)15L, (V)185L, (V)9L, (V)3L, (V)"db/maps/직업퀘스트/lod7001.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name4_s, (V)"직업퀘스트(무도가)", (V)15L, (V)15L, (V)185L, (V)9L, (V)4L, (V)"db/maps/직업퀘스트/lod7001.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name5_s, (V)"직업퀘스트(마법사)", (V)15L, (V)15L, (V)185L, (V)9L, (V)5L, (V)"db/maps/직업퀘스트/lod7002.map");
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name6_s, (V)"직업퀘스트(성직자)", (V)15L, (V)15L, (V)185L, (V)9L, (V)6L, (V)"db/maps/직업퀘스트/lod7002.map");
                p.Call("warp_create", (V)0L, v_name1_s, (V)4L, (V)0L, v_name2_s, (V)4L, (V)13L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name1_s, (V)23L, (V)0L, v_name3_s, (V)4L, (V)13L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name1_s, (V)13L, (V)0L, v_name4_s, (V)4L, (V)13L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name1_s, (V)0L, (V)4L, v_name5_s, (V)13L, (V)5L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name1_s, (V)0L, (V)13L, v_name6_s, (V)13L, (V)5L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name2_s, (V)4L, (V)14L, v_name1_s, (V)4L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name2_s, (V)5L, (V)14L, v_name1_s, (V)4L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name2_s, (V)6L, (V)14L, v_name1_s, (V)4L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name3_s, (V)4L, (V)14L, v_name1_s, (V)23L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name3_s, (V)5L, (V)14L, v_name1_s, (V)23L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name3_s, (V)6L, (V)14L, v_name1_s, (V)23L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name4_s, (V)4L, (V)14L, v_name1_s, (V)13L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name4_s, (V)5L, (V)14L, v_name1_s, (V)13L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name4_s, (V)6L, (V)14L, v_name1_s, (V)13L, (V)3L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name5_s, (V)14L, (V)4L, v_name1_s, (V)3L, (V)4L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name5_s, (V)14L, (V)5L, v_name1_s, (V)3L, (V)4L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name5_s, (V)14L, (V)6L, v_name1_s, (V)3L, (V)4L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name6_s, (V)14L, (V)4L, v_name1_s, (V)3L, (V)13L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name6_s, (V)14L, (V)5L, v_name1_s, (V)3L, (V)13L, (V)99L, (V)99L, (V)1L);
                p.Call("warp_create", (V)0L, v_name6_s, (V)14L, (V)6L, v_name1_s, (V)3L, (V)13L, (V)99L, (V)99L, (V)1L);
                p.Call("mob_clear", v_name2_s);
                p.Call("item_clear", v_name2_s);
                p.Call("mob_clear", v_name3_s);
                p.Call("item_clear", v_name3_s);
                p.Call("mob_clear", v_name4_s);
                p.Call("item_clear", v_name4_s);
                p.Call("mob_clear", v_name5_s);
                p.Call("item_clear", v_name5_s);
                p.Call("mob_clear", v_name6_s);
                p.Call("item_clear", v_name6_s);
                p.Call("warp_create", (V)3L, v_name1_s, (V)19L, (V)12L, (V)"엘리멘탈이동(수)", (V)0L, (V)0L, (V)99L, (V)99L, (V)0L);
                p.Call("warp_create", (V)3L, v_name1_s, (V)19L, (V)13L, (V)"엘리멘탈이동(수)", (V)0L, (V)0L, (V)99L, (V)99L, (V)0L);
                p.Call("warp_create", (V)3L, v_name1_s, (V)20L, (V)12L, (V)"엘리멘탈이동(수)", (V)0L, (V)0L, (V)99L, (V)99L, (V)0L);
                p.Call("warp_create", (V)3L, v_name1_s, (V)20L, (V)13L, (V)"엘리멘탈이동(수)", (V)0L, (V)0L, (V)99L, (V)99L, (V)0L);
                p.Call("group_val", (V)"#jobquest1", (V)0L, (V)1L);
                p.Call("group_val", (V)"#jobquest2", (V)0L, (V)1L);
                p.Call("group_val", (V)"#jobquest3", (V)0L, (V)1L);
                p.Call("group_val", (V)"#jobquest4", (V)0L, (V)1L);
                p.Call("group_val", (V)"#jobquest5", (V)0L, (V)1L);
                p.Call("group_val", (V)"#jobquest6", (V)0L, (V)1L);
                p.Call("group_warp", v_name1_s, (V)18L, (V)7L);
            }
            else
                if (V.T(((V)(v_select2) == (V)((V)2L))))
                {
                    yield break;
                }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
