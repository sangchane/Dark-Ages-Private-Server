using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 봉인의주인 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_봉인의주인", "5.99표")]
    public class NpcBD09C778C758C8FCC778 : PackNpc
    {
        public NpcBD09C778C758C8FCC778(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            yield return Mes((V)1L, (V)"봉인의 주인을 깨울려면 4속성의 씰이 필요합니다.");
            if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"바다의씰")) == (V)((V)0L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"화염의씰")) == (V)((V)0L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"대지의씰")) == (V)((V)0L))))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"바람의씰")) == (V)((V)0L))))))
            {
                yield return Mes((V)0L, (V)"4속성의 씰을 보유하고 있지 않습니다.");
                yield break;
            }
            p.Call("item_del", (V)"바다의씰", (V)100L);
            p.Call("item_del", (V)"화염의씰", (V)100L);
            p.Call("item_del", (V)"대지의씰", (V)100L);
            p.Call("item_del", (V)"바람의씰", (V)100L);
            p.Call("mob_clear", p.Call("get_mapname", v_myid));
            p.Call("mob_spawn2", (V)"봉인의주인", (V)20L, (V)16L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)30L)));
            p.Call("group_val", (V)"#jobquest6", (V)1L, (V)1L);
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
