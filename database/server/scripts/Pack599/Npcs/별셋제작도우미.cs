using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 별셋제작도우미 — 5.99 `Npc_Making.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_별셋제작도우미", "5.99표")]
    public class NpcBCC4C14BC81CC791B3C4C6B0BBF8 : PackNpc
    {
        public NpcBCC4C14BC81CC791B3C4C6B0BBF8(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;
            V v_select2 = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            v_select2 = (V)0L;
            yield return Mes((V)1L, (V)"어서오시오, 별의심장을 유일하게 가공할줄아는 대장장이라오.");
            L_re: ;
            yield return Menu((V)"별의심장으로 가공하고 싶은것이 있소?", (V)"별귀걸이", (V)"별각반", (V)"별벨트", (V)"별목걸이", (V)"별장갑", (V)"별반지");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"별의심장으로 별귀걸이를 가공할려면 별의심장 5개, 150000골드가 필요하오.");
                yield return Menu((V)"별귀걸이를 제작하겠소?", (V)"제작한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("item_exist", v_myid, (V)"별의심장")) < (V)((V)5L))))
                    {
                        yield return Mes((V)0L, (V)"별의심장이 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)150000L))))
                    {
                        yield return Mes((V)0L, (V)"가공비가 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    yield return Mes((V)1L, (V)"별의심장, 가공비가 충분하여 가공을 해드리겠소.");
                    p.Call("item_del", (V)"별의심장", (V)5L);
                    p.Call("money_del", (V)150000L);
                    p.Call("item_add", (V)"별귀걸이", (V)1L);
                    yield return Mes((V)0L, (V)"자! 가공을 성공적으로 끝맞췄소. 확인해 보시오.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"별의심장으로 별각반을 가공할려면 별의심장 10개, 1000000골드가 필요하오.");
                yield return Menu((V)"별각반을 제작하겠소?", (V)"제작한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("item_exist", v_myid, (V)"별의심장")) < (V)((V)10L))))
                    {
                        yield return Mes((V)0L, (V)"별의심장이 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)1000000L))))
                    {
                        yield return Mes((V)0L, (V)"가공비가 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    yield return Mes((V)1L, (V)"별의심장, 가공비가 충분하여 가공을 해드리겠소.");
                    p.Call("item_del", (V)"별의심장", (V)10L);
                    p.Call("money_del", (V)1000000L);
                    p.Call("item_add", (V)"별각반", (V)1L);
                    yield return Mes((V)0L, (V)"자! 가공을 성공적으로 끝맞췄소. 확인해 보시오.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"별의심장으로 별벨트을 가공할려면 별의심장 10개, 1000000골드가 필요하오.");
                yield return Menu((V)"별벨트를 제작하겠소?", (V)"제작한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("item_exist", v_myid, (V)"별의심장")) < (V)((V)10L))))
                    {
                        yield return Mes((V)0L, (V)"별의심장이 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)1000000L))))
                    {
                        yield return Mes((V)0L, (V)"가공비가 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    yield return Mes((V)1L, (V)"별의심장, 가공비가 충분하여 가공을 해드리겠소.");
                    p.Call("item_del", (V)"별의심장", (V)10L);
                    p.Call("money_del", (V)1000000L);
                    p.Call("item_add", (V)"별벨트", (V)1L);
                    yield return Mes((V)0L, (V)"자! 가공을 성공적으로 끝맞췄소. 확인해 보시오.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"별의심장으로 별목걸이을 가공할려면 별의심장 10개, 1000000골드가 필요하오.");
                yield return Menu((V)"별목걸이를 제작하겠소?", (V)"제작한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("item_exist", v_myid, (V)"별의심장")) < (V)((V)10L))))
                    {
                        yield return Mes((V)0L, (V)"별의심장이 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)1000000L))))
                    {
                        yield return Mes((V)0L, (V)"가공비가 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    yield return Mes((V)1L, (V)"별의심장, 가공비가 충분하여 가공을 해드리겠소.");
                    p.Call("item_del", (V)"별의심장", (V)10L);
                    p.Call("money_del", (V)1000000L);
                    p.Call("item_add", (V)"별목걸이", (V)1L);
                    yield return Mes((V)0L, (V)"자! 가공을 성공적으로 끝맞췄소. 확인해 보시오.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"별의심장으로 별장갑을 가공할려면 별의심장 10개, 1000000골드가 필요하오.");
                yield return Menu((V)"별장갑를 제작하겠소?", (V)"제작한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("item_exist", v_myid, (V)"별의심장")) < (V)((V)10L))))
                    {
                        yield return Mes((V)0L, (V)"별의심장이 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)1000000L))))
                    {
                        yield return Mes((V)0L, (V)"가공비가 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    yield return Mes((V)1L, (V)"별의심장, 가공비가 충분하여 가공을 해드리겠소.");
                    p.Call("item_del", (V)"별의심장", (V)10L);
                    p.Call("money_del", (V)1000000L);
                    p.Call("item_add", (V)"별장갑", (V)1L);
                    yield return Mes((V)0L, (V)"자! 가공을 성공적으로 끝맞췄소. 확인해 보시오.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                yield return Mes((V)1L, (V)"별의심장으로 별반지을 가공할려면 별의심장 10개, 1000000골드가 필요하오.");
                yield return Menu((V)"별반지를 제작하겠소?", (V)"제작한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("item_exist", v_myid, (V)"별의심장")) < (V)((V)10L))))
                    {
                        yield return Mes((V)0L, (V)"별의심장이 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)1000000L))))
                    {
                        yield return Mes((V)0L, (V)"가공비가 부족하여 가공할수 없소이다.");
                        yield break;
                    }
                    yield return Mes((V)1L, (V)"별의심장, 가공비가 충분하여 가공을 해드리겠소.");
                    p.Call("item_del", (V)"별의심장", (V)10L);
                    p.Call("money_del", (V)1000000L);
                    p.Call("item_add", (V)"별반지", (V)1L);
                    yield return Mes((V)0L, (V)"자! 가공을 성공적으로 끝맞췄소. 확인해 보시오.");
                }
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
