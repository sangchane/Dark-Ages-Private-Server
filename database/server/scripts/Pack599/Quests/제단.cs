using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 제단 — 밀레스신전 빛의 기둥 곁. 가짜 킹아크퍼스(거대문어)가 떨군 문어다리를 바치면 원한의에센스를 준다(원작 공략). 성직자는 게껍질 5개,
    /// 다른 직업은 문어의원한 7개(문어다리를 지닌 사람이 문어를 잡으면 떨어진다 — <c>static/quest-drops.tsv</c>, 원작 「막타를 반드시 해당 캐릭터가」).
    /// 원작은 아벨 시약상점 브레한에게 먼저 물었다 — 사용자 「대화형 부분은 패싱」으로 뺐다.
    /// </summary>
    [Script("NPC_제단", "킹아크퍼스퀘스트")]
    public class NpcMilethAltarOffering : PackNpc
    {
        public const long Priest = 4;     // get_class — Class.Priest
        public const long Grudges = 7, PriestShells = 5;

        public NpcMilethAltarOffering(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = p.Call("get_myid");
            if (!V.T(p.Call("item_exist", v_myid, (V)"문어다리")))
            {
                yield return Mes((V)0L, (V)"오래된 제단이다. 킹아크퍼스의 흔적을 바치면 무슨 일이 일어날 것 같다.");
                yield break;
            }
            var priest = V.T((V)(p.Call("get_class", v_myid)) == (V)Priest);
            var need = priest ? "게껍질" : "문어의원한";
            var count = priest ? PriestShells : Grudges;
            long have = (long) p.Call("item_exist", v_myid, (V)need);
            if (have < count)
            {
                yield return Mes((V)0L, (V)(priest
                    ? $"문어다리가 꿈틀거린다… 게껍질 {count}개를 함께 바쳐라. ({have}/{count})"
                    : $"문어다리가 꿈틀거린다… 문어다리를 지닌 채 아벨해안의 문어를 직접 쓰러뜨려 문어의원한 {count}개를 모아 오라. ({have}/{count})"));
                yield break;
            }
            if (!V.T(p.Call("item_room", (V)"원한의에센스")))
            {
                yield return Mes((V)0L, (V)"가방에 빈 자리를 만들고 다시 바쳐라.");
                yield break;
            }
            yield return Mes((V)1L, (V)"제단이 붉게 빛나며 문어다리가 스러지고, 그 자리에 원한의에센스가 남았다. 이제 진짜 킹아크퍼스를 만날 수 있다.");
            // 「다음」을 기다리는 사이 바닥에 버리거나 건넸으면 주지 않는다 — item_del 은 모자라도 조용히 지나간다(리뷰 2026-10-11).
            if (!V.T(p.Call("item_exist", v_myid, (V)"문어다리")) || (long) p.Call("item_exist", v_myid, (V)need) < count)
                yield break;
            p.Call("item_del", (V)"문어다리", (V)1L);
            p.Call("item_del", (V)need, (V)count);
            p.Call("item_add", (V)"원한의에센스", (V)1L);
            yield break;
        }
    }
}
