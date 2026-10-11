using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 미레 — 킹아크퍼스의본거지(사본)에 붙잡혀 있는 소녀. 암초 대본(<c>킹아크퍼스암초.cs</c>)이 진짜 길일 때 `npc_spawn` 으로 세운다. 킹아크퍼스를
    /// 잡은 뒤 누르면 누른 사람에게 세줄금반지(원작 세줄황금반지와 수치 같음)와 전설을 주고 아벨해안대기실로 보낸다(원작 공략 그림 — 대기실 (20,10)).
    /// 원한의에센스는 여기서 누른 사람이 가졌으면 하나 쓴다(가는 길에 죽어도 잃지 않게, SPEC). 그룹원은 누구나 하나씩 받는다.
    /// </summary>
    [Script("NPC_미레", "킹아크퍼스퀘스트")]
    public class NpcMire : PackNpc
    {
        public NpcMire(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = p.Call("get_myid");
            if (V.T((V)(p.Call("map_objmob", v_myid)) > (V)0L))
            {
                yield return Mes((V)0L, (V)"미레: 킹아크퍼스가 아직 저기 있어요! 무서워요…");
                yield break;
            }
            if (!V.T(p.Call("item_room", (V)"세줄금반지")))
            {
                yield return Mes((V)0L, (V)"미레: 고마워요! 드릴 게 있는데… 가방에 빈 자리를 만들고 다시 말을 걸어 주세요.");
                yield break;
            }
            yield return Mes((V)1L, (V)"미레: 우앙~ 드디어 누군가가 날 구하러 와 줬군요. 커다란 문어에게 붙잡혀 와서 너무 무서웠어요. 제 반지를 드릴게요.");
            p.Call("item_del", (V)"원한의에센스", (V)1L);   // 가진 사람만 하나 — 없으면 조용히 지나간다(그룹원)
            p.Call("item_add", (V)"세줄금반지", (V)1L);
            p.Call("legend_add", (V)7L, (V)68L, (V)"아벨해안의 무법자 킹아크퍼스를 물리치다");
            p.Call("message", (V)3L, (V)"{=c※킹아크퍼스※ 미레를 구하고 세줄금반지를 받았습니다.");
            p.Call("warp", (V)NpcKingArcfusReef.Waiting, (V)NpcKingArcfusReef.WaitX, (V)NpcKingArcfusReef.WaitY);
            yield break;
        }
    }
}
