using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 대장장이 — 아벨무기점. 터틀껍질·게껍질·퐁퐁이조각을 2개씩 받고 대지의인장을 만든다(원작 공략: 대장장이에게 「대지의 인장」 → 「재료를 가져왔습니다」
    /// 타이핑 — 사용자 「대화형 부분은 패싱」으로 누르면 바로).
    /// </summary>
    [Script("NPC_대장장이", "킹아크퍼스퀘스트")]
    public class NpcBlacksmith : PackNpc
    {
        public static readonly string[] Shells = { "터틀껍질", "게껍질", "퐁퐁이조각" };
        public const long Each = 2;

        public NpcBlacksmith(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = p.Call("get_myid");
            var have = new List<string>();
            var ready = true;
            foreach (var shell in Shells)
            {
                long count = (long) p.Call("item_exist", v_myid, (V)shell);
                have.Add($"{shell} {System.Math.Min(count, Each)}/{Each}");
                ready &= count >= Each;
            }
            if (!ready)
            {
                yield return Mes((V)0L, (V)$"대장장이: 아벨해안의 터틀·크라켄·퐁퐁이 껍질을 2개씩 가져오면 대지의인장을 만들어 주지. ({string.Join(" · ", have)})");
                yield break;
            }
            if (!V.T(p.Call("item_room", (V)"대지의인장")))
            {
                yield return Mes((V)0L, (V)"대장장이: 가방에 빈 자리를 만들고 다시 오게.");
                yield break;
            }
            yield return Mes((V)1L, (V)"대장장이: 좋아, 재료가 다 모였군. 이 인장을 지닌 사람은 대지의 힘으로 굳건히 땅 위에 설 수 있다네.");
            foreach (var shell in Shells)                     // 「다음」을 기다리는 사이 버렸으면 만들지 않는다(제단과 같음)
                if ((long) p.Call("item_exist", v_myid, (V)shell) < Each)
                    yield break;
            foreach (var shell in Shells)
                p.Call("item_del", (V)shell, (V)Each);
            p.Call("item_add", (V)"대지의인장", (V)1L);
            yield break;
        }
    }
}
