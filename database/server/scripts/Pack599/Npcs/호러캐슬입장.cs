using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 호러캐슬입장 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_호러캐슬입장", "5.99표")]
    public class NpcD638B7ECCE90C2ACC785C7A5 : PackNpc
    {
        public NpcD638B7ECCE90C2ACC785C7A5(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            yield return Mes((V)1L, (V)"그래 .. 자네도 호러캐슬에 가려고 하는겐가? 생각보다 만만치 않은 곳이라는걸 알아둬야 할텐데 ..");
            if (V.T(((V)(p.Call("get_class_sub", v_myid)) != (V)((V)0L))))
            {
                yield return Mes((V)0L, (V)"승급자는 입장할수 없습니다.");
                yield break;
            }
            if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)99L))))
            {
                yield return Mes((V)0L, (V)"아직 입장할수 없습니다.");
                yield break;
            }
            L_re: ;
            yield return Menu((V)"호러캐슬에 입장하려면 기부금 조로 3천골드를 지급해야 한다네 .. 성 보수도 해야하고.. 의외로 돈들어가는 곳이 많지...", (V)"딴데간다", (V)"3천골드를 낸다");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)0L, (V)"그래 딴데를 가보도록 해보게..");
                yield break;
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)3000L))))
                    {
                        yield return Mes((V)0L, (V)"골드가 부족하다네, 입장할려면 3000골드가 필요하다네.");
                        yield break;
                    }
                    else
                    {
                        p.Call("money_del", (V)3000L);
                        p.Call("warp", (V)"호러캐슬메인홀", (V)46L, (V)15L);
                        yield return Mes((V)0L, (V)"기부금 3천골드 받았습니다. 이동시켜 드리겠습니다.");
                        yield break;
                    }
                }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
