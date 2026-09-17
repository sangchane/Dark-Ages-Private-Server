using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 화론전사 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_화론전사", "5.99표")]
    public class NpcD654B860C804C0AC : PackNpc
    {
        public NpcD654B860C804C0AC(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            L_re: ;
            v_myid = p.Call("get_myid");
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)1L))) || V.T(((V)(p.Call("get_class_sub")) != (V)((V)2L))))))
            {
                yield return Mes((V)1L, (V)"검투사가 아닙니다.");
                yield break;
            }
            yield return Menu((V)"2차승급기술을 가르쳐드립니다.", (V)"검투술(Lev1)", (V)"검투술(Lev2)", (V)"퓨리소월루(Lev1)", (V)"적무기해체");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"검투술(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 검투술(Lev1)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"검투술(Lev2)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 검투술(Lev2)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"검투술(Lev3)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 검투술(Lev3)를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"검투술(Lev1) 조건 : 2차승급");
                if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))))
                {
                    p.Call("skill_add2", (V)"검투술(Lev1)");
                    yield return Mes((V)1L, (V)"검투술(Lev1) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"검투술(Lev1)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"검투술(Lev2)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 검투술(Lev2)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"검투술(Lev3)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 검투술(Lev3)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("skill_exist", (V)"검투술(Lev1)")) != (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"검투술(Lev1)를 먼저 익혀야합니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"검투술(Lev2) 조건 : 2차승급 어빌리티레벨 8 재료 : 보리임5개 판티카5개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)8L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"보리임")) >= (V)((V)5L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"판티카")) >= (V)((V)5L))))))
                {
                    p.Call("skill_add2", (V)"검투술(Lev2)");
                    p.Call("skill_del2", (V)"검투술(Lev1)");
                    p.Call("item_del", (V)"보리임", (V)5L);
                    p.Call("item_del", (V)"판티카", (V)5L);
                    yield return Mes((V)1L, (V)"검투술(Lev2) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"검투술(Lev2)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"퓨리소월루(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 퓨리소월루(Lev1)를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"퓨리소월루(Lev1) 조건 : 2차승급 어빌리티레벨 25 재료 : 그린토닉5개, 옐로토닉5개, 레드토닉5개 블루토닉5개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)25L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"그린토닉")) >= (V)((V)5L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"옐로토닉")) >= (V)((V)5L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"레드토닉")) >= (V)((V)5L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"블루토닉")) >= (V)((V)5L))))))
                {
                    p.Call("skill_add2", (V)"퓨리소월루(Lev1)");
                    p.Call("item_del", (V)"그린토닉", (V)5L);
                    p.Call("item_del", (V)"옐로토닉", (V)5L);
                    p.Call("item_del", (V)"레드토닉", (V)5L);
                    p.Call("item_del", (V)"블루토닉", (V)5L);
                    yield return Mes((V)1L, (V)"퓨리소월루(Lev1) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"퓨리소월루(Lev1)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                if (V.T(((V)(p.Call("skill_exist", (V)"적무기해체")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 적무기해체을 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"적무기해체 조건 : 2차승급 어빌리티레벨 10 재료 : 그린토닉3개 3개 레드토닉 1개 돈 50만");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)10L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"그린토닉")) >= (V)((V)3L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"레드토닉")) >= (V)((V)3L))))) && V.T(((V)(p.Call("get_money", v_myid)) >= (V)((V)500000L))))))
                {
                    p.Call("skill_add", (V)"적무기해체");
                    p.Call("money_del", (V)500000L);
                    p.Call("item_del", (V)"그린토닉", (V)3L);
                    p.Call("item_del", (V)"레드토닉", (V)3L);
                    yield return Mes((V)1L, (V)"적무기해체 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"적무기해체를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
        }
    }
}
