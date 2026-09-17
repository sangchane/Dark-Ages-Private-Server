using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 1차이아 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_1차이아", "5.99표")]
    public class Npc0031CC28C774C544 : PackNpc
    {
        public Npc0031CC28C774C544(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_menu = 0;
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            v_menu = (V)0L;
            yield return Mes((V)1L, (V)"강력한 기술을 배우고 싶습니까? 재물만 가져와준다면 가르쳐 드리겠습니다.");
            L_re: ;
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)4L))))
            {
                yield return Mes((V)0L, (V)"당신은 성직자 마스터가 아니라서 저의 가르침을 받을수 없습니다.");
                yield break;
            }
            if (V.T(((V)(p.Call("get_class_sub", v_myid)) == (V)((V)0L))))
            {
                yield return Mes((V)1L, (V)"당신은 승급자가 아니군요..");
                yield break;
            }
            yield return Menu((V)"어느것의 가르침을 받겠습니까?\\n기본제물 : 5만골드", (V)"홀리쿠라네라", (V)"수페라에나르마", (V)"신의축복", (V)"디나르콜룸(순수)", (V)"디소루미아(순수)", (V)"홀리드래곤(순수)");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 좀비의살 20개, 좀비지팡이 10개가 필요합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비지팡이")) < (V)((V)10L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"홀리쿠라네라")))
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
                p.Call("item_del", (V)"좀비지팡이", (V)10L);
                p.Call("spell_add", (V)"홀리쿠라네라");
                yield return Mes((V)1L, (V)"홀리쿠라네라를 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 좀비의살 20개, 좀비지팡이 10개가 필요합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비지팡이")) < (V)((V)10L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"수페라에나르마")))
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
                p.Call("item_del", (V)"좀비지팡이", (V)10L);
                p.Call("spell_add", (V)"수페라에나르마");
                yield return Mes((V)1L, (V)"수페라에나르마를 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 좀비의살 20개, 좀비지팡이 10개가 필요합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비지팡이")) < (V)((V)10L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"신의축복")))
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
                p.Call("item_del", (V)"좀비지팡이", (V)10L);
                p.Call("spell_add", (V)"신의축복");
                yield return Mes((V)1L, (V)"신의축복을 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                if (V.T(((V)(p.Call("get_son")) == (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"순수가 아닌자는 배울수없습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 좀비의살 20개, 좀비지팡이 10개, 고사목뿌리 5개가 필요합니다.");
                if (V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비지팡이")) < (V)((V)10L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"고사목뿌리")) < (V)((V)5L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"디나르콜룸")))
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
                p.Call("item_del", (V)"좀비지팡이", (V)10L);
                p.Call("item_del", (V)"고사목뿌리", (V)5L);
                p.Call("spell_add", (V)"디나르콜룸");
                yield return Mes((V)1L, (V)"디나르콜룸을 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                if (V.T(((V)(p.Call("get_son")) == (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"순수가 아닌자는 배울수없습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 좀비의살 20개, 좀비지팡이 10개, 고사목뿌리 5개가 필요합니다.");
                if (V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비지팡이")) < (V)((V)10L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"고사목뿌리")) < (V)((V)5L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"디소루미아")))
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
                p.Call("item_del", (V)"좀비지팡이", (V)10L);
                p.Call("item_del", (V)"고사목뿌리", (V)5L);
                p.Call("spell_add", (V)"디소루미아");
                yield return Mes((V)1L, (V)"디소루미아을 습득하셧습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                if (V.T(((V)(p.Call("get_son")) == (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"순수가 아닌자는 배울수없습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"이 스킬을 배울려면 좀비의살 20개, 좀비지팡이 10개, 고사목뿌리 8개가 필요합니다.");
                if (V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비지팡이")) < (V)((V)10L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"고사목뿌리")) < (V)((V)8L))))))
                {
                    yield return Mes((V)1L, (V)"재물이 부족하여 배울수 없습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"홀리드래곤")))
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
                p.Call("item_del", (V)"좀비지팡이", (V)10L);
                p.Call("item_del", (V)"고사목뿌리", (V)8L);
                p.Call("spell_add", (V)"홀리드래곤");
                yield return Mes((V)1L, (V)"홀리드래곤을 습득하셧습니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
