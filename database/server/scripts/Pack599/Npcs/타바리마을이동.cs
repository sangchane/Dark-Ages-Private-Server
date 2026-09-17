using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 타바리마을이동 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_타바리마을이동", "5.99표")]
    public class NpcD0C0BC14B9ACB9C8C744C774B3D9 : PackNpc
    {
        public NpcD0C0BC14B9ACB9C8C744C774B3D9(GameServer server, Mundane mundane) : base(server, mundane)
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
            L_re: ;
            yield return Menu((V)"용건이 무엇인가?", (V)"타바리마을 이동!");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"강화리젠트 를 주는 셀라임보스 등 이쁜마을 ^_^ 요금은 30만Gold라네.");
                L_re2: ;
                yield return Menu((V)"요금을 내고 타바리마을로 이동하겠나?", (V)"이동한다.", (V)"이동하지 않는다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)0L))))
                {
                    goto L_re2;
                }
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)300000L))))
                    {
                        yield return Mes((V)0L, (V)"골드가 부족하다네.");
                        yield break;
                    }
                    p.Call("warp", (V)"타바리마을", (V)67L, (V)92L);
                    p.Call("money_del", (V)300000L);
                    yield return Mes((V)0L, (V)"이동이 완료되었다네.");
                    yield break;
                }
                else
                    if (V.T(((V)(v_select2) == (V)((V)2L))))
                    {
                        yield break;
                    }
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
