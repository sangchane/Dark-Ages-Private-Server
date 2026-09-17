using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 배틀장퇴장 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_배틀장퇴장", "5.99표")]
    public class NpcBC30D2C0C7A5D1F4C7A5 : PackNpc
    {
        public NpcBC30D2C0C7A5D1F4C7A5(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            yield return Mes((V)1L, (V)"배틀장을 퇴장하여 빛의신전으로 가고싶습니까?");
            L_re: ;
            yield return Menu((V)"빛의신전으로 가시겠습니까?", (V)"이동한다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                p.Call("warp", (V)"빛의신전", (V)19L, (V)17L);
                yield break;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
