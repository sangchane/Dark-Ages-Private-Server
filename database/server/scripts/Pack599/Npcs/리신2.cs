using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 리신2 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_리신2", "5.99표")]
    public class NpcB9ACC2E00032 : PackNpc
    {
        public NpcB9ACC2E00032(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_select = (V)0L;
            yield return Mes((V)1L, (V)"저는 무도가 스킬사범 리신입니다.");
            yield return Mes((V)1L, (V)"당신이 무도가라면, 저의 가르침을 받으시길 바랍니다. 심안의 힘으로!");
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)5L))))
            {
                yield return Mes((V)0L, (V)"당신은 무도가가 아니라서 저의 가르침을 받을수 없습니다.");
                yield break;
            }
            L_re: ;
            v_myid = p.Call("get_myid");
            yield return Menu((V)"무도가님, 어느 스킬을 원하십니까?", (V)"양의신권[21]", (V)"단각[31]", (V)"장풍[31]", (V)"금강불괴[41]", (V)"구양신공[50]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"양의신권은 일정확률로 기본공격시 2번 공격하는 패시브기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"양의신권")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"양의신권");
                yield return Mes((V)1L, (V)"양의신권를 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 하위스킬인 슬레쉬를 소유하고있어야하며, 좀비의살 20개, 좀비의막대기 10개가 필요합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의막대기")) < (V)((V)10L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"블로우")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(V.B(!V.T(p.Call("skill_exist", (V)"슬레쉬")))))
                {
                    yield return Mes((V)1L, (V)"이스킬의 하위스킬이 없어 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)50000L))))
                {
                    yield return Mes((V)1L, (V)"골드가 부족합니다.");
                    goto L_re;
                }
                p.Call("money_del", (V)50000L);
                p.Call("item_del", (V)"좀비의살", (V)20L);
                p.Call("item_del", (V)"좀비의막대기", (V)10L);
                p.Call("skill_add", (V)"블로우");
                p.Call("skill_del", (V)"슬레쉬");
                yield return Mes((V)1L, (V)"블로우를 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                if (V.T(((V)(p.Call("get_son")) == (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"순수가 아닌자는 배울수없습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 하위스킬인 습격을 소유하고있어야하며, 좀비의살 20개, 좀비의막대기 10개, 고사목뿌리 5개가 필요합니다.");
                if (V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의막대기")) < (V)((V)10L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"고사목뿌리")) < (V)((V)5L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"습격진")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(V.B(!V.T(p.Call("skill_exist", (V)"습격")))))
                {
                    yield return Mes((V)1L, (V)"이스킬의 하위스킬이 없어 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)50000L))))
                {
                    yield return Mes((V)1L, (V)"골드가 부족합니다.");
                    goto L_re;
                }
                p.Call("money_del", (V)50000L);
                p.Call("item_del", (V)"좀비의살", (V)20L);
                p.Call("item_del", (V)"좀비의막대기", (V)10L);
                p.Call("item_del", (V)"고사목뿌리", (V)5L);
                p.Call("skill_add", (V)"습격진");
                p.Call("skill_del", (V)"습격");
                yield return Mes((V)1L, (V)"습격진을 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                if (V.T(((V)(p.Call("get_son")) == (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"순수가 아닌자는 배울수없습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 좀비의살 20개, 좀비의막대기 10개, 고사목뿌리 8개가 필요합니다.");
                if (V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의막대기")) < (V)((V)10L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"고사목뿌리")) < (V)((V)8L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"백스텝")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)50000L))))
                {
                    yield return Mes((V)1L, (V)"골드가 부족합니다.");
                    goto L_re;
                }
                p.Call("money_del", (V)50000L);
                p.Call("item_del", (V)"좀비의살", (V)20L);
                p.Call("item_del", (V)"좀비의막대기", (V)10L);
                p.Call("item_del", (V)"고사목뿌리", (V)8L);
                p.Call("skill_add", (V)"백스텝");
                yield return Mes((V)1L, (V)"백스텝을 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                if (V.T(((V)(p.Call("get_son")) == (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"순수가 아닌자는 배울수없습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 좀비의살 20개, 좀비의막대기 10개, 고사목뿌리 8개가 필요합니다.");
                if (V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의막대기")) < (V)((V)10L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"고사목뿌리")) < (V)((V)8L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"파이어트랩")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)50000L))))
                {
                    yield return Mes((V)1L, (V)"골드가 부족합니다.");
                    goto L_re;
                }
                p.Call("money_del", (V)50000L);
                p.Call("item_del", (V)"좀비의살", (V)20L);
                p.Call("item_del", (V)"좀비의막대기", (V)10L);
                p.Call("item_del", (V)"고사목뿌리", (V)8L);
                p.Call("skill_add2", (V)"파이어트랩");
                yield return Mes((V)1L, (V)"파이어트랩을 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                if (V.T(((V)(p.Call("get_son")) == (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"순수가 아닌자는 배울수없습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 하위스킬인 암살격을 소유하고있어야하며, 좀비의살 20개, 좀비의막대기 10개, 고사목뿌리 5개가 필요합니다.");
                if (V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의막대기")) < (V)((V)10L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"고사목뿌리")) < (V)((V)5L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"암살격진")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(V.B(!V.T(p.Call("skill_exist", (V)"암살격")))))
                {
                    yield return Mes((V)1L, (V)"이스킬의 하위스킬이 없어 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)50000L))))
                {
                    yield return Mes((V)1L, (V)"골드가 부족합니다.");
                    goto L_re;
                }
                p.Call("money_del", (V)50000L);
                p.Call("item_del", (V)"좀비의살", (V)20L);
                p.Call("item_del", (V)"좀비의막대기", (V)10L);
                p.Call("item_del", (V)"고사목뿌리", (V)5L);
                p.Call("skill_add", (V)"암살격진");
                p.Call("skill_del", (V)"암살격");
                yield return Mes((V)1L, (V)"암살격진을 습득하셧습니다.");
                goto L_re;
            }
        }
    }
}
