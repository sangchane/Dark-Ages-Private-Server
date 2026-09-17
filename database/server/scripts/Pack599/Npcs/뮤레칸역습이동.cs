using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 뮤레칸역습이동 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_뮤레칸역습이동", "5.99표")]
    public class NpcBBA4B808CE78C5EDC2B5C774B3D9 : PackNpc
    {
        public NpcBBA4B808CE78C5EDC2B5C774B3D9(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_selectz = 0;

            v_myid = p.Call("get_myid");
            v_selectz = (V)0L;
            L_re: ;
            if (V.T(V.B(V.T(((V)(p.Call("get_basevita", v_myid)) < (V)((V)400000L))) && V.T(((V)(p.Call("get_basemana", v_myid)) < (V)((V)200000L))))))
            {
                yield return Mes((V)0L, (V)"오직 강한자만...");
            }
            yield return Menu((V)"뮤레칸의역습맵으로 이동하시겠습니까?\\n[격수 체력 40만이상 || 비격 마력 20만이상입장가능]", (V)"아벨");
            v_selectz = reply.Choice;
            if (V.T(V.B(V.T(((V)(v_selectz) == (V)((V)0L))) || V.T(V.B(!V.T(v_selectz))))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_selectz) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("item_exist", v_myid, (V)"뮤레칸입장티켓")) == (V)((V)0L))))
                {
                    yield return Mes((V)0L, (V)"뮤레칸입장티켓이 없습니다.");
                }
                p.Call("item_del", (V)"뮤레칸입장티켓", (V)1L);
                p.Call("warp", (V)"뮤레칸의역습::아벨", (V)67L, (V)23L);
                yield return Mes((V)1L, (V)"뮤레칸입장티켓을 받았습니다.\\n[이동이 완료되었습니다.]");
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
