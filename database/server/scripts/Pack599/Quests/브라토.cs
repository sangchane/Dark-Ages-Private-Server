using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 브라토 — 아벨신해안외딴곳(아벨해안3-C 숨은 칸 (20,37)으로 간다)에 사는 사내. 해저의팬던트를 주고 아벨해안대기실로 보낸다(원작 공략 그림).
    /// 원작은 붉은진주가 있어야 숨은 방에 들었다 — 사용자 「앞부분 대화형 부분은 패싱」으로 진주·포스파늄 단계는 없앴다. 팬던트는 열쇠라 하나만.
    /// </summary>
    [Script("NPC_브라토", "킹아크퍼스퀘스트")]
    public class NpcBrato : PackNpc
    {
        public NpcBrato(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = p.Call("get_myid");
            if (!V.T(p.Call("item_exist", v_myid, (V)"해저의팬던트")))
            {
                if (!V.T(p.Call("item_room", (V)"해저의팬던트")))
                {
                    yield return Mes((V)0L, (V)"브라토: 줄 것이 있는데… 가방에 빈 자리를 만들고 다시 오게.");
                    yield break;
                }
                yield return Mes((V)1L, (V)"브라토: 여기까지 찾아오다니 대단하군. 바다 밑에서 건진 해저의팬던트일세. 킹아크퍼스를 만나려면 이것과 대지의인장이 있어야 하지.");
                p.Call("item_add", (V)"해저의팬던트", (V)1L);
            }
            yield return Mes((V)1L, (V)"브라토: 그럼 잘 가게.");
            p.Call("warp", (V)NpcKingArcfusReef.Waiting, (V)NpcKingArcfusReef.WaitX, (V)NpcKingArcfusReef.WaitY);
            yield break;
        }
    }
}
