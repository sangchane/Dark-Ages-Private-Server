using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 무기만들자 — 5.99 `Npc_Making.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_무기만들자", "5.99표")]
    public class NpcBB34AE30B9CCB4E4C790 : PackNpc
    {
        public NpcBB34AE30B9CCB4E4C790(GameServer server, Mundane mundane) : base(server, mundane)
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
            L_re: ;
            yield return Menu((V)"무엇때문에 말을 걸었지?", (V)"재료 구입", (V)"무기 교환");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                p.Call("shop", (V)0L, (V)"화론재료사기", (V)"돌괭이를 구매 하겠나?");
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)1L, (V)"화론재료를 일정수량모아온다면 특별한 아이템으로 교환해주겠네.");
                    L_re2: ;
                    yield return Menu((V)"무엇으로 교환해 줄까?", (V)"영월궁(보로껍50개)", (V)"영월도(보로껍50개)", (V)"돈파(보로껍50개)", (V)"비파(앤도르날개50개)", (V)"영월충(앤도르날개)", (V)"주작의서클릿(앤도르날개50개)");
                    v_menu = reply.Choice;
                    if (V.T(((V)(v_menu) == (V)((V)0L))))
                    {
                        goto L_re2;
                    }
                    if (V.T(((V)(v_menu) == (V)((V)1L))))
                    {
                        if (V.T(((V)(p.Call("item_exist", v_myid, (V)"보로보로껍질")) < (V)((V)50L))))
                        {
                            yield return Mes((V)1L, (V)"보로보로껍질이 부족합니다.");
                            goto L_re2;
                        }
                        p.Call("item_del", (V)"보로보로껍질", (V)50L);
                        p.Call("item_add", (V)"영월궁", (V)1L);
                        yield return Mes((V)1L, (V)"보로보로껍질을 영월궁으로 교환해 드렸습니다!");
                        goto L_re2;
                    }
                    if (V.T(((V)(v_menu) == (V)((V)2L))))
                    {
                        if (V.T(((V)(p.Call("item_exist", v_myid, (V)"보로보로껍질")) < (V)((V)50L))))
                        {
                            yield return Mes((V)1L, (V)"보로보로껍질이 부족합니다.");
                            goto L_re2;
                        }
                        p.Call("item_del", (V)"보로보로껍질", (V)50L);
                        p.Call("item_add", (V)"영월도", (V)1L);
                        yield return Mes((V)1L, (V)"보로보로껍질을 영월도로 교환해 드렸습니다!");
                        goto L_re2;
                    }
                    if (V.T(((V)(v_menu) == (V)((V)3L))))
                    {
                        if (V.T(((V)(p.Call("item_exist", v_myid, (V)"보로보로껍질")) < (V)((V)50L))))
                        {
                            yield return Mes((V)1L, (V)"보로보로껍질이 부족합니다.");
                            goto L_re2;
                        }
                        p.Call("item_del", (V)"보로보로껍질", (V)50L);
                        p.Call("item_add", (V)"돈파", (V)1L);
                        yield return Mes((V)1L, (V)"보로보로껍질을 돈파로 교환해 드렸습니다!");
                        goto L_re2;
                    }
                    if (V.T(((V)(v_menu) == (V)((V)4L))))
                    {
                        if (V.T(((V)(p.Call("item_exist", v_myid, (V)"앤도르날개")) < (V)((V)50L))))
                        {
                            yield return Mes((V)1L, (V)"앤도르날개이 부족합니다.");
                            goto L_re2;
                        }
                        p.Call("item_del", (V)"앤도르날개", (V)50L);
                        p.Call("item_add", (V)"비파", (V)1L);
                        yield return Mes((V)1L, (V)"앤도르날개을 비파로 교환해 드렸습니다!");
                        goto L_re2;
                    }
                    if (V.T(((V)(v_menu) == (V)((V)5L))))
                    {
                        if (V.T(((V)(p.Call("item_exist", v_myid, (V)"앤도르날개")) < (V)((V)50L))))
                        {
                            yield return Mes((V)1L, (V)"앤도르날개이 부족합니다.");
                            goto L_re2;
                        }
                        p.Call("item_del", (V)"앤도르날개", (V)50L);
                        p.Call("item_add", (V)"영월충", (V)1L);
                        yield return Mes((V)1L, (V)"앤도르날개을 영월충으로 교환해 드렸습니다!");
                        goto L_re2;
                    }
                    if (V.T(((V)(v_menu) == (V)((V)6L))))
                    {
                        if (V.T(((V)(p.Call("item_exist", v_myid, (V)"앤도르날개")) < (V)((V)50L))))
                        {
                            yield return Mes((V)1L, (V)"앤도르날개이 부족합니다.");
                            goto L_re2;
                        }
                        p.Call("item_del", (V)"앤도르날개", (V)50L);
                        p.Call("item_add", (V)"주작의서클릿", (V)1L);
                        yield return Mes((V)1L, (V)"앤도르날개을 주작의서클릿으로 교환해 드렸습니다!");
                        goto L_re2;
                    }
                    yield break;
                }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
