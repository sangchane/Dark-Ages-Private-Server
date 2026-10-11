using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 킹아크퍼스숨은칸 — 아벨해안3-C (20,37)을 밟으면 도는 대본(워프 <c>ScriptNpc</c>). 대지의인장을 가진 사람만 외딴곳(브라토)으로 보낸다.
    /// 원작은 붉은진주가 있어야 들어갔다 — 진주 단계를 뺀 대신 인장을 열쇠로 삼아, 사냥 벌판 한가운데라도 자동 사냥·생태계 봇이 우연히
    /// 빨려 들어가지 않는다(리뷰 2026-10-11). 없으면 아무 말 없이 지나간다(숨은 칸이다).
    /// </summary>
    [Script("NPC_킹아크퍼스숨은칸", "킹아크퍼스퀘스트")]
    public class NpcKingArcfusHole : PackNpc
    {
        public const string Hole = "아벨해안3-C";
        public const string Nook = "아벨신해안외딴곳";

        public NpcKingArcfusHole(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = p.Call("get_myid");
            if (V.T((V)(p.Call("get_mapname", v_myid)) != (V)Hole) || !V.T(p.Call("item_exist", v_myid, (V)"대지의인장")))
                yield break;
            p.Call("message", (V)3L, (V)"{=c※킹아크퍼스※ 대지의인장이 울리며 발밑 모래가 꺼집니다.");
            p.Call("warp", (V)Nook, (V)5L, (V)8L);
            yield break;
        }
    }
}
