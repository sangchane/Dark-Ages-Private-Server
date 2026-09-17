using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 애교 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_애교", "5.99표")]
    public class NpcC560AD50 : PackNpc
    {
        public NpcC560AD50(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            yield return Mes((V)1L, (V)"어디든지 배를 타고갈수 있는곳이라면 대려다주지.");
            L_re: ;
            yield return Menu((V)"참고로 운항비는 옆에 적혀있으니깐, 착오없도록해!", (V)"오렌마을(5000Gold)");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)5000L))))
                {
                    yield return Mes((V)0L, (V)"운항비가 부족하잖아! 돈을벌어와.");
                    yield break;
                }
                p.Call("money_del", (V)"5000");
                p.Call("warp", (V)"오렌", (V)57L, (V)169L);
                yield return Mes((V)0L, (V)"자! 다도착했다구!");
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
