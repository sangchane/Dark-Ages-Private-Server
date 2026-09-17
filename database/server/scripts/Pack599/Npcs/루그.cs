using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 루그 — 5.99 `Npc_Making.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_루그", "5.99표")]
    public class NpcB8E8ADF8 : PackNpc
    {
        public NpcB8E8ADF8(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_rand = 0;
            V v_select = 0;

            L_RE: ;
            v_rand = p.Call("rand", (V)1L, (V)100L);
            if (V.T(((V)(v_rand) < (V)((V)0L))))
                goto L_RE;
            yield return Mes((V)1L, (V)"읭? 누구인가? 여긴 어떻게 들어온겐가 ?");
            yield return Mes((V)1L, (V)"하.. 그녀석이 데려다 줬다고? 참 쓸데없는짓을 했구만..");
            yield return Mes((V)1L, (V)"자오랜만에 실력발휘좀해볼까");
            yield return Menu((V)"제작할것을 선택하세요", (V)"생명의벨트", (V)"생명의목걸이", (V)"암흑의벨트", (V)"암흑의목걸이");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"생명의벨트");
                yield return Mes((V)1L, (V)"{=c생명의시약1개,뉴트의뿔1개");
                yield return Mes((V)1L, (V)"재료를 확인중입니다");
                v_myid = p.Call("get_myid");
                if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"생명의시약")) >= (V)((V)1L))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"뉴트의뿔")) >= (V)((V)1L))))))
                {
                    p.Call("item_del", (V)"생명의시약", (V)1L);
                    p.Call("item_del", (V)"뉴트의뿔", (V)1L);
                    yield return Mes((V)1L, (V)"제작중");
                    yield return Mes((V)1L, (V)"{=c제작을 성공하셨습니다!!");
                    p.Call("item_add", (V)"생명의벨트", (V)1L);
                }
                else
                {
                    yield return Mes((V)1L, (V)"{=q 재료가 부족하네요");
                }
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)1L, (V)"생명의목걸이를 제작합니다");
                    yield return Mes((V)1L, (V)"{=c홀리팬플롯1개,블루수염1개");
                    yield return Mes((V)1L, (V)"재료를 확인중입니다");
                    v_myid = p.Call("get_myid");
                    if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"홀리팬플롯")) >= (V)((V)1L))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"블루오피온의수염")) >= (V)((V)1L))))))
                    {
                        p.Call("item_del", (V)"홀리팬플롯", (V)1L);
                        p.Call("item_del", (V)"블루오피온의수염", (V)1L);
                        yield return Mes((V)1L, (V)"제작중");
                        yield return Mes((V)1L, (V)"{=c제작을 성공하셨습니다!!");
                        p.Call("item_add", (V)"생명의목걸이", (V)1L);
                    }
                    else
                    {
                        yield return Mes((V)1L, (V)"{=q 재료가 부족하네요");
                    }
                }
                else
                    if (V.T(((V)(v_select) == (V)((V)3L))))
                    {
                        yield return Mes((V)1L, (V)"암흑의벨트를 제작합니다");
                        yield return Mes((V)1L, (V)"{=c암흑의시약1개,뉴트의뿔1개");
                        yield return Mes((V)1L, (V)"재료를 확인중입니다");
                        v_myid = p.Call("get_myid");
                        if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"암흑의시약")) >= (V)((V)1L))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"뉴트의뿔")) >= (V)((V)1L))))))
                        {
                            p.Call("item_del", (V)"암흑의시약", (V)1L);
                            p.Call("item_del", (V)"뉴트의뿔", (V)1L);
                            yield return Mes((V)1L, (V)"제작중");
                            yield return Mes((V)1L, (V)"{=c제작을 성공하셨습니다!!");
                            p.Call("item_add", (V)"암흑의벨트", (V)1L);
                        }
                        else
                        {
                            yield return Mes((V)1L, (V)"{=q 재료가 부족하네요");
                        }
                    }
                    else
                        if (V.T(((V)(v_select) == (V)((V)4L))))
                        {
                            yield return Mes((V)1L, (V)"암흑의목걸이를 제작합니다");
                            yield return Mes((V)1L, (V)"{=c레드오피온의수염1개 여의주1개");
                            yield return Mes((V)1L, (V)"재료를 확인중입니다");
                            v_myid = p.Call("get_myid");
                            if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"레드오피온의수염")) >= (V)((V)1L))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"여의주")) >= (V)((V)1L))))))
                            {
                                p.Call("item_del", (V)"레드오피온의수염", (V)1L);
                                p.Call("item_del", (V)"여의주", (V)1L);
                                yield return Mes((V)1L, (V)"제작중");
                                yield return Mes((V)1L, (V)"{=c제작을 성공하셨습니다!!");
                                p.Call("item_add", (V)"암흑의목걸이", (V)1L);
                            }
                            else
                            {
                                yield return Mes((V)1L, (V)"{=q 재료가 부족하네요");
                            }
                        }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
