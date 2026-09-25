using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 개인던전입장도우미 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_개인던전입장도우미", "5.99표")]
    public class NpcAC1CC778B358C804C785C7A5B3C4C6B0BBF8 : PackNpc
    {
        public NpcAC1CC778B358C804C785C7A5B3C4C6B0BBF8(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            L_re: ;
            yield return Menu((V)"솔로사냥터로 이동하시겠습니까?\\n요금은 1500골드입니다.", (V)"지하수로(지존)", (V)"승급던전");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)1500L))))
                {
                    yield return Mes((V)0L, (V)"솔로던전 입장요금이 부족합니다.");
                    yield break;
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_level", v_myid)) != (V)((V)99L))) || V.T(((V)(p.Call("get_class_sub", v_myid)) != (V)((V)0L))))))
                {
                    yield return Mes((V)0L, (V)"지존만 입장할수 있습니다.");
                    yield break;
                }
                p.Call("group_end", v_myid);
                p.Call("warp", (V)"지하수로D-1", (V)5L, (V)5L);
                p.Call("money_del", (V)"1500");
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)1500L))))
                    {
                        yield return Mes((V)0L, (V)"솔로던전 입장요금이 부족합니다.");
                        yield break;
                    }
                    if (V.T(V.B(V.T(((V)(p.Call("get_level", v_myid)) != (V)((V)99L))) || V.T(((V)(p.Call("get_class_sub", v_myid)) != (V)((V)2L))))))
                    {
                        yield return Mes((V)0L, (V)"승급이상만 입장할수 있습니다.");
                        yield break;
                    }
                    p.Call("group_end", v_myid);
                    p.Call("warp", (V)"승급던젼", (V)37L, (V)13L);
                    p.Call("money_del", (V)"1500");
                }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
