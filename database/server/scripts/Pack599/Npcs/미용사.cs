using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 미용사 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_미용사", "5.99표")]
    public class NpcBBF8C6A9C0AC : PackNpc
    {
        public NpcBBF8C6A9C0AC(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_hair = 0;
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            L_re: ;
            yield return Menu((V)"안녕하십니까? 멋진 헤어스타일을 원하십니까??\\n어떤 가위로 자르시겠습니까?\\n(요금5000GOLD)", (V)"가위", (V)"금가위", (V)"가위머리보기(새창)");
            v_select = reply.Choice;
            if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)5000L))))
            {
                yield return Mes((V)0L, (V)"요금이 부족합니다.");
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(V.B(!V.T(p.Call("item_exist", v_myid, (V)"가위")))))
                {
                    yield return Mes((V)0L, (V)"소지하신 가위가 없으십니다.");
                    yield break;
                }
                L_re2: ;
                yield return Input((V)"어떤 헤어스타일로 해드릴까요?\\n(0~25번중, 0은 빠박이)");
                v_hair = reply.Words;
                if (V.T(V.B(V.T(((V)(v_hair) < (V)((V)1L))) || V.T(((V)(v_hair) > (V)((V)25L))))))
                {
                    yield return Mes((V)1L, (V)"일반 가위로 해드릴수 있는 머리범위 밖입니다.");
                    goto L_re2;
                }
                p.Call("set_hair", v_hair);
                p.Call("money_del", (V)5000L);
                p.Call("item_del", (V)"가위", (V)1L);
                yield return Mes((V)0L, (V)"자, 머리가 완성되었습니다!? 어떠십니까!");
                yield break;
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    if (V.T(V.B(!V.T(p.Call("item_exist", v_myid, (V)"금가위")))))
                    {
                        yield return Mes((V)0L, (V)"소지하신 금가위가 없으십니다.");
                        yield break;
                    }
                    L_re3: ;
                    yield return Input((V)"어떤 헤어스타일로 해드릴까요?\\n(27~47번중)");
                    v_hair = reply.Words;
                    if (V.T(V.B(V.T(((V)(v_hair) < (V)((V)27L))) || V.T(((V)(v_hair) > (V)((V)47L))))))
                    {
                        yield return Mes((V)1L, (V)"금가위로 해드릴수 있는 머리범위 밖입니다.");
                        goto L_re3;
                    }
                    p.Call("set_hair", v_hair);
                    p.Call("money_del", (V)5000L);
                    p.Call("item_del", (V)"금가위", (V)1L);
                    yield return Mes((V)0L, (V)"자, 머리가 완성되었습니다!? 어떠십니까!");
                    yield break;
                }
                else
                    if (V.T(((V)(v_select) == (V)((V)3L))))
                    {
                        yield return Mes((V)1L, (V)"다음을 누르시면 새로운 웹창으로 이미지를 보여드립니다.");
                        p.Call("game_web", v_myid, (V)"http://novaonline.woobi.co.kr/Novaonline/img/scissors.gif");
                        yield break;
                    }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
