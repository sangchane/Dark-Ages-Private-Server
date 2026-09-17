using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 노엠마을 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_노엠마을", "5.99표")]
    public class NpcB178C5E0B9C8C744 : PackNpc
    {
        public NpcB178C5E0B9C8C744(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_ability")) < (V)((V)18L))))
            {
                p.Call("message", (V)3L, (V)"어빌리티 18이하는 입장하실수없습니다.");
                yield break;
            }
            yield return Mes((V)1L, (V)"안녕하세요 노엠마을로 이동시켜드립니다.");
            p.Call("warp", (V)"노엠마을", (V)35L, (V)27L);
            yield break;
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
