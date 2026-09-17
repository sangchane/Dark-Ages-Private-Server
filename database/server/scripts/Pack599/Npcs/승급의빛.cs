using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 승급의빛 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_승급의빛", "5.99표")]
    public class NpcC2B9AE09C758BE5B : PackNpc
    {
        public NpcC2B9AE09C758BE5B(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            p["#clsck"] = (V)2L;
            p.Call("set_delstrabismus", v_myid);
            p.Call("effect", v_myid, (V)280L, (V)0L, (V)75L);
            p.Call("mob_clear", p.Call("get_mapname", v_myid));
            p.Call("set_delstrabismus", v_myid);
            yield return Mes((V)0L, (V)"빛이 확장되더니 이내 모든공간을 밝혔다. 이곳에서 빠져나가보자.");
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
