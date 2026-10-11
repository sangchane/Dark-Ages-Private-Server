using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 마레 — 에리얼의방(사본)에 붙잡혀 있는 아이. 입구 대본(<c>에리얼입구.cs</c>)이 `npc_spawn` 으로 세운다. 에리얼을 진정시킨 뒤 누르면
    /// 누른 사람에게 파파야방패와 전설 「뤼케시온 해안의 에리얼을 진정시키다」(원작 블로그 스크린샷 문구)를 주고 3-D 입구로 보낸다.
    /// 사용자 2026-10-11 「깰 때마다」 — 다시 들어오면 새 에리얼이 서므로 또 받는다. 한 사본에서는 누르면 곧 나가므로 한 번.
    /// </summary>
    [Script("NPC_마레", "에리얼퀘스트")]
    public class NpcMare : PackNpc
    {
        public NpcMare(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = p.Call("get_myid");
            if (V.T((V)(p.Call("map_objmob", v_myid)) > (V)0L))
            {
                yield return Mes((V)0L, (V)"마레: 에리얼이 아직 날뛰고 있어요! 먼저 진정시켜 주세요.");
                yield break;
            }
            // item_add 는 가방이 차면 말없이 버린다 — 방패를 잃고 에리얼부터 다시 잡지 않게 먼저 묻는다(리뷰 2026-10-11).
            if (!V.T(p.Call("item_room", (V)"파파야방패")))
            {
                yield return Mes((V)0L, (V)"마레: 고마워요! 드릴 방패가 있는데… 가방에 빈 자리를 만들고 다시 말을 걸어 주세요.");
                yield break;
            }
            yield return Mes((V)1L, (V)"마레: 살았다… 고마워요! 제가 지니고 있던 방패예요. 받아 주세요.");
            p.Call("item_add", (V)"파파야방패", (V)1L);
            p.Call("legend_add", (V)7L, (V)68L, (V)"뤼케시온 해안의 에리얼을 진정시키다");
            p.Call("message", (V)3L, (V)"{=c※에리얼※ 마레를 구하고 파파야방패를 받았습니다.");
            p.Call("warp", (V)NpcAerialEntrance.Shore, (V)55L, (V)25L);
            yield break;
        }
    }
}
