using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 선진 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_선진", "5.99표")]
    public class NpcC120C9C4 : PackNpc
    {
        public NpcC120C9C4(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_HP = 0;
            V v_MP = 0;
            V v_class = 0;
            V v_myid = 0;
            V v_select = 0;
            V v_select2 = 0;
            V v_select3 = 0;

            v_myid = p.Call("get_myid");
            v_class = p.Call("get_class", v_myid);
            v_HP = p.Call("get_basevita2", v_myid);
            v_MP = p.Call("get_basemana2", v_myid);
            v_select = (V)0L;
            v_select2 = (V)0L;
            v_select3 = (V)0L;
            yield return Mes((V)1L, (V)"신들과 통하는 공간인 신전에 오신것을 환영합니다.");
            L_re: ;
            yield return Menu((V)"어떤일을 하시겠습니까?", (V)"전직하기", (V)"신들의세계", (V)"승급투구", (V)"대장장이신전");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"당신이 지존에 도달하였다면, 한번의 전직 기회가 찾아옵니다.\\n(무도가는 환골탈퇴)");
                yield return Mes((V)1L, (V)"전직(환골탈퇴)을 할수있는 조건은 체력과 마력이 일정수준 이상으로 도달하였다면, 전직(환골탈퇴)을 할수 있습니다.");
                yield return Mes((V)1L, (V)"전직시에는 무장을 해체하실 필요가 없습니다. 착용하실 아이템이 있으시다면 착용하고 다시 말을 걸어주세요.");
                if (V.T(((V)(p.Call("get_class_sub")) != (V)((V)0L))))
                {
                    yield return Mes((V)0L, (V)"승급자는 전직 또는 환골탈퇴를 할수없습니다.");
                    yield break;
                }
                if (V.T(((V)(p.Call("get_level", p.Call("myid"))) < (V)((V)99L))))
                {
                    yield return Mes((V)0L, (V)"당신은 아직 전직을 할만큼의 모험을 해오지 않았군요.");
                    yield break;
                }
                if (V.T(((V)(p["#classchange"]) == (V)((V)1L))))
                {
                    yield return Mes((V)0L, (V)"당신은 이미 전직(환골탈퇴)하신 몸이시군요. 욕심은 신들께서 싫어하십니다.");
                    yield break;
                }
                if (V.T(((V)(v_class) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 전사로써 전직 조건은, 체력 16000 마력 5000입니다.");
                    if (V.T(V.B(V.T(((V)(v_HP) < (V)((V)16000L))) || V.T(((V)(v_MP) < (V)((V)5000L))))))
                    {
                        yield return Mes((V)0L, (V)"당신은 아직 전직을 하실수가 없습니다. 체력마력 기준치를 달성한뒤 와주세요.");
                        yield break;
                    }
                    goto L_go;
                }
                if (V.T(((V)(v_class) == (V)((V)2L))))
                {
                    yield return Mes((V)1L, (V)"당신은 도적으로써 전직 조건은, 체력 13000 마력 7500입니다.");
                    if (V.T(V.B(V.T(((V)(v_HP) < (V)((V)13000L))) || V.T(((V)(v_MP) < (V)((V)7500L))))))
                    {
                        yield return Mes((V)0L, (V)"당신은 아직 전직을 하실수가 없습니다. 체력마력 기준치를 달성한뒤 와주세요.");
                        yield break;
                    }
                    goto L_go;
                }
                if (V.T(((V)(v_class) == (V)((V)3L))))
                {
                    yield return Mes((V)1L, (V)"당신은 마법사로써 전직 조건은, 체력 12000 마력 12500입니다.");
                    if (V.T(V.B(V.T(((V)(v_HP) < (V)((V)12000L))) || V.T(((V)(v_MP) < (V)((V)12500L))))))
                    {
                        yield return Mes((V)0L, (V)"당신은 아직 전직을 하실수가 없습니다. 체력마력 기준치를 달성한뒤 와주세요.");
                        yield break;
                    }
                    goto L_go;
                }
                if (V.T(((V)(v_class) == (V)((V)4L))))
                {
                    yield return Mes((V)1L, (V)"당신은 성직자로써 전직 조건은, 체력 13000 마력 13500입니다.");
                    if (V.T(V.B(V.T(((V)(v_HP) < (V)((V)13000L))) || V.T(((V)(v_MP) < (V)((V)13500L))))))
                    {
                        yield return Mes((V)0L, (V)"당신은 아직 전직을 하실수가 없습니다. 체력마력 기준치를 달성한뒤 와주세요.");
                        yield break;
                    }
                    goto L_go;
                }
                if (V.T(((V)(v_class) == (V)((V)5L))))
                {
                    yield return Mes((V)1L, (V)"당신은 무도가로써 환골탈퇴 조건은, 체력 16000 마력 6000입니다.");
                    if (V.T(V.B(V.T(((V)(v_HP) < (V)((V)16000L))) || V.T(((V)(v_MP) < (V)((V)6000L))))))
                    {
                        yield return Mes((V)0L, (V)"당신은 아직 환골탈퇴를 하실수가 없습니다. 체력마력 기준치를 달성한뒤 와주세요.");
                        yield break;
                    }
                    goto L_go2;
                }
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)1L, (V)"신들의 세계에서는 당신의 체력과 마력 최대치를 경험치를 팔아 상승 시킬수 있습니다.");
                    yield return Mes((V)1L, (V)"신들의 세계로 가실려면, 신들에게 제물로 엑스쿠라눔 1개를 바치셔야 합니다.");
                    L_re2: ;
                    yield return Menu((V)"제물 1개를 바치시고 신들의 세계로 가시겠습니까?", (V)"이동한다.", (V)"이동하지 않는다.");
                    v_select2 = reply.Choice;
                    if (V.T(((V)(v_select2) == (V)((V)0L))))
                    {
                        goto L_re2;
                    }
                    if (V.T(((V)(v_select2) == (V)((V)1L))))
                    {
                        if (V.T(((V)(p.Call("item_exist", v_myid, (V)"엑스쿠라눔")) == (V)((V)0L))))
                        {
                            yield return Mes((V)0L, (V)"엑스쿠라눔이 없습니다.");
                        }
                        p.Call("item_del", (V)"엑스쿠라눔", (V)1L);
                        p.Call("warp", (V)"신들의세계", (V)23L, (V)21L);
                        yield return Mes((V)0L, (V)"성공적으로 신들의 세계로 이동되었습니다.");
                        yield break;
                    }
                    else
                    {
                        yield return Mes((V)0L, (V)"신들의세계로 가실때에는 신중히 생각하시고 결정해주세요.");
                        yield break;
                    }
                }
                else
                    if (V.T(((V)(v_select) == (V)((V)3L))))
                    {
                        if (V.T(((V)(p["#masterud"]) == (V)((V)1L))))
                        {
                            yield return Mes((V)0L, (V)"당신은 이미 승급투구를 지급받으셧습니다.");
                            yield break;
                        }
                        if (V.T(((V)(p.Call("get_class_sub")) == (V)((V)0L))))
                        {
                            yield return Mes((V)0L, (V)"비승급자는 투구를 받으실수 없습니다.");
                            yield break;
                        }
                        yield return Mes((V)1L, (V)"승급의 증표와도같은 승급투구를 구하시고 싶으십니까?");
                        yield return Mes((V)1L, (V)"승급투구를 받으실려면 그 자격을 증명하기위해 좀비의살 20개, 고사목뿌리10개를 가져오셔야합니다.");
                        L_masterud: ;
                        yield return Menu((V)"승급투구를 받겠습니까?", (V)"네, 자격을 증명할수 있습니다.", (V)"아직 재물이 부족합니다.");
                        v_select2 = reply.Choice;
                        if (V.T(((V)(v_select2) == (V)((V)0L))))
                        {
                            goto L_masterud;
                        }
                        if (V.T(((V)(v_select2) == (V)((V)1L))))
                        {
                            if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"좀비의살")) < (V)((V)20L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"고사목뿌리")) < (V)((V)10L))))))
                            {
                                yield return Mes((V)0L, (V)"당신은 아직 승급의 자격을 증명할수 없으시군요.");
                                yield break;
                            }
                            p.Call("item_del", (V)"좀비의살", (V)20L);
                            p.Call("item_del", (V)"고사목뿌리", (V)10L);
                            p["#masterud"] = (V)1L;
                            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
                            {
                                p.Call("item_add", (V)"마스터투구1", (V)1L);
                            }
                            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
                            {
                                p.Call("item_add", (V)"마스터투구2", (V)1L);
                            }
                            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))))
                            {
                                p.Call("item_add", (V)"웨스턴", (V)1L);
                            }
                            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
                            {
                                p.Call("item_add", (V)"다크홀", (V)1L);
                            }
                            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
                            {
                                p.Call("item_add", (V)"다크베일", (V)1L);
                            }
                            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
                            {
                                p.Call("item_add", (V)"세인트홀", (V)1L);
                            }
                            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
                            {
                                p.Call("item_add", (V)"세인트베일", (V)1L);
                            }
                            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
                            {
                                p.Call("item_add", (V)"무상관", (V)1L);
                            }
                            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
                            {
                                p.Call("item_add", (V)"월영관", (V)1L);
                            }
                            yield return Mes((V)0L, (V)"성공적으로 승급투구를 지급해 드렸습니다.");
                            yield break;
                        }
                        else
                        {
                            yield return Mes((V)0L, (V)"자격을 증명할수 있을때 와주세요.");
                            yield break;
                        }
                    }
                    else
                        if (V.T(((V)(v_select) == (V)((V)4L))))
                        {
                            yield return Mes((V)1L, (V)"대장장이의 신전으로 가시겠습니까? 다음을 누르면 보내드리도록 해드리죠.");
                            p.Call("warp", (V)"대장장이신전", (V)7L, (V)6L);
                            yield return Mes((V)0L, (V)"대장장이신전으로 이동이 완료되었습니다.");
                            yield break;
                        }
            yield break;
            L_go: ;
            yield return Mes((V)1L, (V)"당신은 충분히 전직할수 있는 체력과 마력을 지니셧군요.");
            yield return Mes((V)1L, (V)"다음을 누르시면 당신이 전직할수있는 직업 목록표를 보여드리도록 하겠습니다.");
            L_re3: ;
            yield return Menu((V)"어느 직업으로 전직 하시겠습니까?\\n(전직시에 보유기술, 스펠은 보존됩니다.)", (V)"전사", (V)"도적", (V)"마법사", (V)"성직자");
            v_select2 = reply.Choice;
            if (V.T(((V)(v_select2) == (V)((V)0L))))
            {
                goto L_re3;
            }
            if (V.T(((V)(v_select2) == (V)((V)1L))))
            {
                L_re4: ;
                yield return Menu((V)"강인한 체력과 강력함을 지닌 전사로서 다시 태워 나시겠습니까?", (V)"네, 전사로 선택했습니다.", (V)"다시 선택하겠습니다.");
                v_select3 = reply.Choice;
                if (V.T(((V)(v_select3) == (V)((V)0L))))
                {
                    goto L_re4;
                }
                if (V.T(((V)(v_select3) == (V)((V)1L))))
                {
                    if (V.T(((V)(v_class) == (V)((V)1L))))
                    {
                        p.Call("set_son", (V)1L);
                    }
                    else
                    {
                        p.Call("set_son", (V)0L);
                    }
                    p.Call("set_class", (V)1L);
                    p.Call("legend_add", (V)0L, (V)255L, (V)"어둠의전설, 전사로 전직하다.");
                    p.Call("message", (V)3L, ((V)(((V)((V)"{=q전사[") + (V)(p.Call("get_name")))) + (V)((V)"]님. 축하드립니다! 멋진 두번째 모험되시기를 바랍니다!")));
                    p.Call("skill_add", (V)"숏블레이드");
                    goto L_go3;
                }
                else
                    if (V.T(((V)(v_select3) == (V)((V)2L))))
                    {
                        yield return Mes((V)1L, (V)"신중히 골라주시길 바랍니다.");
                        goto L_re3;
                    }
            }
            if (V.T(((V)(v_select2) == (V)((V)2L))))
            {
                L_re5: ;
                yield return Menu((V)"뛰어난 스피드와 치명적인 매력을 지닌 도적으로서 다시 태워 나시겠습니까?", (V)"네, 도적으로 선택했습니다.", (V)"다시 선택하겠습니다.");
                v_select3 = reply.Choice;
                if (V.T(((V)(v_select3) == (V)((V)0L))))
                {
                    goto L_re5;
                }
                if (V.T(((V)(v_select3) == (V)((V)1L))))
                {
                    if (V.T(((V)(v_class) == (V)((V)2L))))
                    {
                        p.Call("set_son", (V)1L);
                    }
                    else
                    {
                        p.Call("set_son", (V)0L);
                    }
                    p.Call("set_class", (V)2L);
                    p.Call("legend_add", (V)0L, (V)255L, (V)"어둠의전설, 도적로 전직하다.");
                    p.Call("message", (V)3L, ((V)(((V)((V)"{=q도적[") + (V)(p.Call("get_name")))) + (V)((V)"]님. 축하드립니다! 멋진 두번째 모험되시기를 바랍니다!")));
                    p.Call("skill_add", (V)"찌르기");
                    goto L_go3;
                }
                else
                    if (V.T(((V)(v_select3) == (V)((V)2L))))
                    {
                        yield return Mes((V)1L, (V)"신중히 골라주시길 바랍니다.");
                        goto L_re3;
                    }
            }
            if (V.T(((V)(v_select2) == (V)((V)3L))))
            {
                L_re6: ;
                yield return Menu((V)"뛰어난정식력과 강력한 마법공격력을 지닌 마법사로서 다시 태워 나시겠습니까?", (V)"네, 마법사로 선택했습니다.", (V)"다시 선택하겠습니다.");
                v_select3 = reply.Choice;
                if (V.T(((V)(v_select3) == (V)((V)0L))))
                {
                    goto L_re6;
                }
                if (V.T(((V)(v_select3) == (V)((V)1L))))
                {
                    if (V.T(((V)(v_class) == (V)((V)3L))))
                    {
                        p.Call("set_son", (V)1L);
                    }
                    else
                    {
                        p.Call("set_son", (V)0L);
                    }
                    p.Call("set_class", (V)3L);
                    p.Call("legend_add", (V)0L, (V)255L, (V)"어둠의전설, 마법사로 전직하다.");
                    p.Call("message", (V)3L, ((V)(((V)((V)"{=q마법사[") + (V)(p.Call("get_name")))) + (V)((V)"]님. 축하드립니다! 멋진 두번째 모험되시기를 바랍니다!")));
                    p.Call("spell_add", (V)"마레노");
                    goto L_go3;
                }
                else
                    if (V.T(((V)(v_select3) == (V)((V)2L))))
                    {
                        yield return Mes((V)1L, (V)"신중히 골라주시길 바랍니다.");
                        goto L_re3;
                    }
            }
            if (V.T(((V)(v_select2) == (V)((V)4L))))
            {
                L_re7: ;
                yield return Menu((V)"뛰어난신성력과 강력한 회복력을 지닌 성직자로서 다시 태워 나시겠습니까?", (V)"네, 성직자로 선택했습니다.", (V)"다시 선택하겠습니다.");
                v_select3 = reply.Choice;
                if (V.T(((V)(v_select3) == (V)((V)0L))))
                {
                    goto L_re7;
                }
                if (V.T(((V)(v_select3) == (V)((V)1L))))
                {
                    if (V.T(((V)(v_class) == (V)((V)4L))))
                    {
                        p.Call("set_son", (V)1L);
                    }
                    else
                    {
                        p.Call("set_son", (V)0L);
                    }
                    p.Call("set_class", (V)4L);
                    p.Call("legend_add", (V)0L, (V)255L, (V)"어둠의전설, 성직자로 전직하다.");
                    p.Call("message", (V)3L, ((V)(((V)((V)"{=q성직자[") + (V)(p.Call("get_name")))) + (V)((V)"]님. 축하드립니다! 멋진 두번째 모험되시기를 바랍니다!")));
                    p.Call("spell_add", (V)"홀리볼트");
                    p.Call("spell_add", (V)"쿠로");
                    goto L_go3;
                }
                else
                    if (V.T(((V)(v_select3) == (V)((V)2L))))
                    {
                        yield return Mes((V)1L, (V)"신중히 골라주시길 바랍니다.");
                        goto L_re3;
                    }
            }
            yield break;
            L_go2: ;
            yield return Mes((V)1L, (V)"당신은 충분히 환골탈퇴를할수 있는 체력과 마력을 지니셧군요.");
            L_re8: ;
            yield return Menu((V)"환골탈퇴를 하시겠습니까? 신중히 선택해 주시길 바랍니다.", (V)"환골탈퇴를 한다.", (V)"환골탈퇴를 하지 않는다.");
            v_select2 = reply.Choice;
            if (V.T(((V)(v_select2) == (V)((V)0L))))
            {
                goto L_re8;
            }
            if (V.T(((V)(v_select2) == (V)((V)1L))))
            {
                p.Call("legend_add", (V)0L, (V)255L, (V)"어둠의전설, 환골탈퇴를 하다.");
                p.Call("message", (V)3L, ((V)(((V)((V)"{=q무도가[") + (V)(p.Call("get_name")))) + (V)((V)"]님. 축하드립니다! 멋진 두번째 모험되시기를 바랍니다!")));
                goto L_go3;
            }
            else
                if (V.T(((V)(v_select2) == (V)((V)2L))))
                {
                    yield return Mes((V)0L, (V)"환골탈퇴에 신중을 기울여주세요. 승급만 하지 않는다면 언재든지 환골탈퇴가 가능하답니다.");
                    yield break;
                }
            yield break;
            L_go3: ;
            p["#pointmap1$"] = (V)"지정된위치없음";
            p["#pointmap2$"] = (V)"지정된위치없음";
            p["#pointmap3$"] = (V)"지정된위치없음";
            p["#pointmap4$"] = (V)"지정된위치없음";
            p["#pointmap5$"] = (V)"지정된위치없음";
            p.Call("set_basevita", (V)100L);
            p.Call("set_basemana", (V)100L);
            p.Call("set_level", (V)"1");
            p.Call("set_point", (V)"0");
            p.Call("exp_del", (V)"4200000000");
            p.Call("set_str", (V)"4");
            p.Call("set_int", (V)"4");
            p.Call("set_wis", (V)"3");
            p.Call("set_dex", (V)"3");
            p.Call("set_con", (V)"3");
            p["#bounsexpadd"] = (V)0L;
            p["#classchange"] = (V)1L;
            p.Call("warp", (V)"노비스마을", (V)31L, (V)9L);
            yield break;
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
