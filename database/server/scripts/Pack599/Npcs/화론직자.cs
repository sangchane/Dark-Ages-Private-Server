using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 화론직자 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_화론직자", "5.99표")]
    public class NpcD654B860C9C1C790 : PackNpc
    {
        public NpcD654B860C9C1C790(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            L_re: ;
            v_myid = p.Call("get_myid");
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)4L))) || V.T(((V)(p.Call("get_class_sub")) != (V)((V)2L))))))
            {
                yield return Mes((V)1L, (V)"바드가 아닙니다.");
                yield break;
            }
            yield return Menu((V)"2차승급기술을 가르쳐드립니다.", (V)"연주공격(Lev1)", (V)"연주공격(Lev2)", (V)"리젠(Lev1)", (V)"슈페이아움(Lev1)", (V)"메가홀리쿠라노");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"연주공격(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 연주공격(Lev1)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"연주공격(Lev2)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 연주공격(Lev2)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"연주공격(Lev3)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 연주공격(Lev3)를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"연주공격(Lev1) 조건 : 2차승급");
                if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))))
                {
                    p.Call("skill_add2", (V)"연주공격(Lev1)");
                    yield return Mes((V)1L, (V)"연주공격(Lev1) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"연주공격(Lev1)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"연주공격(Lev2)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 연주공격(Lev2)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"연주공격(Lev3)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 연주공격(Lev3)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"연주공격(Lev1)")) != (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"연주공격(Lev1)를 먼저 익혀야합니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"연주공격(Lev2) 조건 : 2차승급 어빌리티레벨 18 재료 : 레드토닉 10개 그린토닉 10개  블루토닉10개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)18L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"레드토닉")) >= (V)((V)10L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"그린토닉")) >= (V)((V)10L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"블루토닉")) >= (V)((V)10L))))))
                {
                    p.Call("skill_add2", (V)"연주공격(Lev2)");
                    p.Call("skill_del2", (V)"연주공격(Lev1)");
                    p.Call("item_del", (V)"레드토닉", (V)10L);
                    p.Call("item_del", (V)"그린토닉", (V)10L);
                    p.Call("item_del", (V)"블루토닉", (V)10L);
                    yield return Mes((V)1L, (V)"연주공격(Lev2) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"연주공격(Lev2)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                if (V.T(((V)(p.Call("spell_exist", (V)"리젠(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 리젠(Lev1)를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"리젠(Lev1) 조건 : 2차승급 어빌리티레벨 22 재료 : 옐로토닉20개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)22L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"옐로토닉")) >= (V)((V)20L))))))
                {
                    p.Call("spell_add2", (V)"리젠(Lev1)");
                    p.Call("item_del", (V)"옐로토닉", (V)20L);
                    yield return Mes((V)1L, (V)"리젠(Lev1) 스펠을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"리젠(Lev1)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"슈페이아움(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 슈페이아움(Lev1)를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"슈페이아움(Lev1) 조건 : 2차승급 어빌리티레벨 20 재료 : 구현中");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)20L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"다이아몬드")) >= (V)((V)20L))))))
                {
                    p.Call("skill_add2", (V)"슈페이아움(Lev1)");
                    p.Call("item_del", (V)"다이아몬드", (V)20L);
                    yield return Mes((V)1L, (V)"슈페이아움(Lev1) 스펠을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"슈페이아움(Lev1)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                if (V.T(((V)(p.Call("spell_exist", (V)"메가홀리쿠라노")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 메가홀리쿠라노를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"메가홀리쿠라노 조건 : 2차승급 어빌리티레벨 18 재료 : 메가토닉15개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)18L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"메가토닉")) >= (V)((V)15L))))))
                {
                    p.Call("spell_add", (V)"메가홀리쿠라노");
                    p.Call("item_del", (V)"메가토닉", (V)15L);
                    yield return Mes((V)1L, (V)"메가홀리쿠라노 스펠을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"메가홀리쿠라노를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
        }
    }
}
