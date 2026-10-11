using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 킹아크퍼스코마 — 외딴길(코마밭) 사본의 정한 칸(<see cref="NpcKingArcfusReef.Coma" />)을 밟으면 도는 대본(<c>warp_create 3</c>).
    /// 원작 공략 그림: 코마밭을 밟으면 「죽음의 그림자가 드리웁니다.」와 함께 체력이 1이 된다. 원작은 서로 살려 주며 건넜다 — 혼자서도 되게
    /// 죽이지는 않는다(체력 1, 물약·회복으로 버틴다).
    /// </summary>
    [Script("NPC_킹아크퍼스코마", "킹아크퍼스퀘스트")]
    public class NpcKingArcfusComa : PackNpc
    {
        public NpcKingArcfusComa(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            p.Call("set_vital", (V)1L);
            p.Call("message", (V)3L, (V)"죽음의 그림자가 드리웁니다.");
            yield break;
        }
    }
}
