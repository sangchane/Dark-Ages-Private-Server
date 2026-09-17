using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 화론도가 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_화론도가", "5.99표")]
    public class NpcD654B860B3C4AC00 : PackNpc
    {
        public NpcD654B860B3C4AC00(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            L_re: ;
            v_myid = p.Call("get_myid");
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)5L))) || V.T(((V)(p.Call("get_class_sub")) != (V)((V)2L))))))
            {
                yield return Mes((V)1L, (V)"수인이 아닙니다.");
                yield break;
            }
            yield return Menu((V)"2차승급기술을 가르쳐드립니다.", (V)"비스트어택(Lev1)", (V)"깃털날리기(Lev1)", (V)"마구때리기", (V)"늑대의위상", (V)"할퀴기(Lev1)");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"비스트어택(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 비스트어택(Lev1)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"비스트어택(Lev2)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 비스트어택(Lev2)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"비스트어택(Lev3)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 비스트어택(Lev3)를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"비스트어택(Lev1) 조건 : 2차승급");
                if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))))
                {
                    p.Call("skill_add", (V)"비스트어택(Lev1)");
                    yield return Mes((V)1L, (V)"비스트어택(Lev1) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"비스트어택(Lev1)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                if (V.T(((V)(p.Call("spell_exist", (V)"깃털날리기(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 깃털날리기(Lev1)을 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"깃털날리기(Lev1) 조건 : 2차승급 어빌리티레벨 15 재료 : 블루토닉10개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)15L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"블루토닉")) >= (V)((V)10L))))))
                {
                    p.Call("spell_add2", (V)"깃털날리기(Lev1)");
                    p.Call("item_del", (V)"블루토닉", (V)10L);
                    yield return Mes((V)1L, (V)"깃털날리기(Lev1) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"깃털날리기(Lev1)을 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"마구때리기")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 마구때리기을 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"마구때리기 조건 : 2차승급 어빌리티레벨 18 재료 : 옐로토닉10개, 그린토닉7개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)18L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"옐로토닉")) >= (V)((V)10L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"그린토닉")) >= (V)((V)7L))))))
                {
                    p.Call("skill_add2", (V)"마구때리기");
                    p.Call("item_del", (V)"옐로토닉", (V)10L);
                    p.Call("item_del", (V)"그린토닉", (V)7L);
                    yield return Mes((V)1L, (V)"마구때리기 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"마구때리기을 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"늑대의위상")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 늑대의위상을 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"늑대의위상 조건 : 2차승급 어빌리티레벨 22 재료 : 레드토닉12개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)22L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"레드토닉")) >= (V)((V)12L))))))
                {
                    p.Call("skill_add2", (V)"늑대의위상");
                    p.Call("item_del", (V)"레드토닉", (V)12L);
                    yield return Mes((V)1L, (V)"늑대의위상 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"늑대의위상을 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"할퀴기(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 할퀴기(Lev1)을 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"할퀴기(Lev1) 조건 : 2차승급 어빌리티레벨 30 재료 : 메가토닉5개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)30L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"메가토닉")) >= (V)((V)5L))))))
                {
                    p.Call("skill_add2", (V)"할퀴기(Lev1)");
                    p.Call("item_del", (V)"메가토닉", (V)5L);
                    yield return Mes((V)1L, (V)"할퀴기(Lev1) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"할퀴기(Lev1)을 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
