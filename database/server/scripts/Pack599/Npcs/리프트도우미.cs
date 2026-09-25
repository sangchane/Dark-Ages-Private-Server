using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 리프트도우미 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_리프트도우미", "5.99표")]
    public class NpcB9ACD504D2B8B3C4C6B0BBF8 : PackNpc
    {
        public NpcB9ACD504D2B8B3C4C6B0BBF8(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            L_re: ;
            yield return Menu((V)"무엇을 도와드릴까요!?", (V)"마을로귀환", (V)"스키장이동");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"다음을 누르시면 마을로 귀환시켜 드리겠습니다!");
                p.Call("warp", (V)"밀레스마을", (V)49L, (V)45L);
                yield return Mes((V)0L, (V)"이동이 완료되었습니다.");
                yield break;
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)0L, (V)"현재 입장이 불가능합니다.");
                    yield break;
                }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
