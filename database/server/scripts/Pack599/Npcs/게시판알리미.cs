using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 게시판알리미 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_게시판알리미", "5.99표")]
    public class NpcAC8CC2DCD310C54CB9ACBBF8 : PackNpc
    {
        public NpcAC8CC2DCD310C54CB9ACBBF8(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_name", v_myid)) == (V)((V)"순삭"))))
            {
                p.Call("message", (V)3L, (V)"게시판 차단당함.");
                yield break;
            }
            p.Call("call_func", v_myid, (V)"Board");
            p.Call("sleep", (V)100L);
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
