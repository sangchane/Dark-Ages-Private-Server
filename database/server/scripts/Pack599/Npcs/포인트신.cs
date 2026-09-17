using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 포인트신 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_포인트신", "5.99표")]
    public class NpcD3ECC778D2B8C2E0 : PackNpc
    {
        public NpcD3ECC778D2B8C2E0(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_point = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            v_point = (V)0L;
            if (V.T(((V)(p.Call("get_ac", v_myid)) != (V)((V)100L))))
            {
                yield return Mes((V)0L, (V)"가리지 않은 모습으로 말을 걸어주게나...");
                yield break;
            }
            if (V.T(((V)(p.Call("get_class_sub", v_myid)) == (V)((V)0L))))
            {
                yield return Mes((V)1L, (V)"당신은 아직 포인트를 구매할수 없습니다.");
                yield break;
            }
            L_re: ;
            yield return Menu((V)"어느 포인트를 올리겠습니까.. (포인트 1개당 400체력이 소모됩니다.)", (V)"힘(STR)", (V)"인트(INT)", (V)"위즈(WIS)", (V)"콘(CON)", (V)"덱스(DEX)");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                L_re2: ;
                yield return Input((V)"힘을 몇개 올리시겠습니까??");
                v_point = reply.Words;
                if (V.T(((V)(v_point) <= (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"똑바로 입력해 주게나...");
                    goto L_re2;
                }
                if (V.T(((V)(((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L)))))) < (V)((V)25000L))))
                {
                    yield return Mes((V)1L, (V)"당신이 살려는 포인트의 양과, 체력수치를 비교한결과 25000이하가 되버려서 사실수가 없습니다.");
                    goto L_re2;
                }
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(((V)(p.Call("get_str", v_myid)) + (V)(v_point))) > (V)((V)215L))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))) && V.T(((V)(((V)(p.Call("get_str", v_myid)) + (V)(v_point))) > (V)((V)180L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(((V)(p.Call("get_str", v_myid)) + (V)(v_point))) > (V)((V)120L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(((V)(p.Call("get_str", v_myid)) + (V)(v_point))) > (V)((V)120L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(((V)(p.Call("get_str", v_myid)) + (V)(v_point))) > (V)((V)180L))))))))
                {
                    yield return Mes((V)1L, (V)"사실려는 포인트가 올포를 넘어버립니다.");
                    goto L_re2;
                }
                p.Call("set_basevita", ((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L))))));
                p.Call("set_str", ((V)(p.Call("get_str", v_myid)) + (V)(v_point)));
                yield return Mes((V)0L, (V)"성공적으로 포인트구매가 이루어졌습니다.");
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                L_re3: ;
                yield return Input((V)"인트를 몇개 올리시겠습니까??");
                v_point = reply.Words;
                if (V.T(((V)(v_point) <= (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"똑바로 입력해 주게나...");
                    goto L_re3;
                }
                if (V.T(((V)(((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L)))))) < (V)((V)25000L))))
                {
                    yield return Mes((V)1L, (V)"당신이 살려는 포인트의 양과, 체력수치를 비교한결과 25000이하가 되버려서 사실수가 없습니다.");
                    goto L_re3;
                }
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(((V)(p.Call("get_int", v_myid)) + (V)(v_point))) > (V)((V)120L))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))) && V.T(((V)(((V)(p.Call("get_int", v_myid)) + (V)(v_point))) > (V)((V)120L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(((V)(p.Call("get_int", v_myid)) + (V)(v_point))) > (V)((V)215L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(((V)(p.Call("get_int", v_myid)) + (V)(v_point))) > (V)((V)180L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(((V)(p.Call("get_int", v_myid)) + (V)(v_point))) > (V)((V)120L))))))))
                {
                    yield return Mes((V)1L, (V)"사실려는 포인트가 올포를 넘어버립니다.");
                    goto L_re3;
                }
                p.Call("set_basevita", ((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L))))));
                p.Call("set_int", ((V)(p.Call("get_int", v_myid)) + (V)(v_point)));
                yield return Mes((V)0L, (V)"성공적으로 포인트구매가 이루어졌습니다.");
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                L_re4: ;
                yield return Input((V)"위즈를 몇개 올리시겠습니까??");
                v_point = reply.Words;
                if (V.T(((V)(v_point) <= (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"똑바로 입력해 주게나...");
                    goto L_re4;
                }
                if (V.T(((V)(((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L)))))) < (V)((V)25000L))))
                {
                    yield return Mes((V)1L, (V)"당신이 살려는 포인트의 양과, 체력수치를 비교한결과 25000이하가 되버려서 사실수가 없습니다.");
                    goto L_re4;
                }
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(((V)(p.Call("get_wis", v_myid)) + (V)(v_point))) > (V)((V)120L))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))) && V.T(((V)(((V)(p.Call("get_wis", v_myid)) + (V)(v_point))) > (V)((V)120L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(((V)(p.Call("get_wis", v_myid)) + (V)(v_point))) > (V)((V)180L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(((V)(p.Call("get_wis", v_myid)) + (V)(v_point))) > (V)((V)215L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(((V)(p.Call("get_wis", v_myid)) + (V)(v_point))) > (V)((V)120L))))))))
                {
                    yield return Mes((V)1L, (V)"사실려는 포인트가 올포를 넘어버립니다.");
                    goto L_re4;
                }
                p.Call("set_basevita", ((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L))))));
                p.Call("set_wis", ((V)(p.Call("get_wis", v_myid)) + (V)(v_point)));
                yield return Mes((V)0L, (V)"성공적으로 포인트구매가 이루어졌습니다.");
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                L_re5: ;
                yield return Input((V)"콘을 몇개 올리시겠습니까??");
                v_point = reply.Words;
                if (V.T(((V)(v_point) <= (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"똑바로 입력해 주게나...");
                    goto L_re5;
                }
                if (V.T(((V)(((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L)))))) < (V)((V)25000L))))
                {
                    yield return Mes((V)1L, (V)"당신이 살려는 포인트의 양과, 체력수치를 비교한결과 25000이하가 되버려서 사실수가 없습니다.");
                    goto L_re5;
                }
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(((V)(p.Call("get_con", v_myid)) + (V)(v_point))) > (V)((V)180L))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))) && V.T(((V)(((V)(p.Call("get_con", v_myid)) + (V)(v_point))) > (V)((V)180L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(((V)(p.Call("get_con", v_myid)) + (V)(v_point))) > (V)((V)120L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(((V)(p.Call("get_con", v_myid)) + (V)(v_point))) > (V)((V)120L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(((V)(p.Call("get_con", v_myid)) + (V)(v_point))) > (V)((V)215L))))))))
                {
                    yield return Mes((V)1L, (V)"사실려는 포인트가 올포를 넘어버립니다.");
                    goto L_re5;
                }
                p.Call("set_basevita", ((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L))))));
                p.Call("set_con", ((V)(p.Call("get_con", v_myid)) + (V)(v_point)));
                yield return Mes((V)0L, (V)"성공적으로 포인트구매가 이루어졌습니다.");
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                L_re6: ;
                yield return Input((V)"덱스를 몇개 올리시겠습니까??");
                v_point = reply.Words;
                if (V.T(((V)(v_point) <= (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"똑바로 입력해 주게나...");
                    goto L_re6;
                }
                if (V.T(((V)(((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L)))))) < (V)((V)25000L))))
                {
                    yield return Mes((V)1L, (V)"당신이 살려는 포인트의 양과, 체력수치를 비교한결과 25000이하가 되버려서 사실수가 없습니다.");
                    goto L_re6;
                }
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(((V)(p.Call("get_dex", v_myid)) + (V)(v_point))) > (V)((V)180L))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))) && V.T(((V)(((V)(p.Call("get_dex", v_myid)) + (V)(v_point))) > (V)((V)215L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(((V)(p.Call("get_dex", v_myid)) + (V)(v_point))) > (V)((V)120L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(((V)(p.Call("get_dex", v_myid)) + (V)(v_point))) > (V)((V)120L))))))) || V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(((V)(p.Call("get_dex", v_myid)) + (V)(v_point))) > (V)((V)180L))))))))
                {
                    yield return Mes((V)1L, (V)"사실려는 포인트가 올포를 넘어버립니다.");
                    goto L_re6;
                }
                p.Call("set_basevita", ((V)(p.Call("get_basevita2", v_myid)) - (V)((((V)(v_point) * (V)((V)400L))))));
                p.Call("set_dex", ((V)(p.Call("get_dex", v_myid)) + (V)(v_point)));
                yield return Mes((V)0L, (V)"성공적으로 포인트구매가 이루어졌습니다.");
                yield break;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
