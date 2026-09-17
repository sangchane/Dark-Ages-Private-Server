using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 멜로린 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_멜로린", "5.99표")]
    public class NpcBA5CB85CB9B0 : PackNpc
    {
        public NpcBA5CB85CB9B0(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            yield return Mes((V)1L, (V)"여기는 노비스마을, 초보자들의 공간입니다.");
            L_re: ;
            yield return Menu((V)"어느것을 도와드릴까요??", (V)"기본적인 명령어를 보여줘.", (V)"노비스 마을에 대해 설명해줘.", (V)"기본적인 시스템을 설명해줘.", (V)"나는 무엇부터 해야하지?(주점이동)", (V)"능력치설명");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"기본적인 명령어를 보여드리겠습니다.");
                p.Call("message", (V)8L, (V)"{=u＊기본적인 명령어＊\\n\\n1. /내정보 -> 자신의 정보를 본다.\\n\\n2. /그룹 -> 자신의 그룹정보를 본다.\\n\\n＊사용자 명령어＊\\n\\n1. @명령어 -> 명령어 목록을 본다.\\n\\n2. @접속시간 -> 현재 접속한시간을 출력한다.\\n\\n3. @탈출 -> 던전에서 탈출한다.");
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"노비스 마을은 처음하는 초보자들부터 21까지의 유저들이 머무는 마을입니다.\\n그렇다고, 21이상의 유저들이 못오는건 아닙니다.");
                yield return Mes((V)1L, (V)"노비스마을에는 총 6가지의 공간이있습니다.\\n노비스잡화상점 : 체력&마력물약등을 판매하고 있습니다.\\n노비스무기방어구상점 : 무기&방어구를 판매하고 있습니다.");
                yield return Mes((V)1L, (V)"노비스민가1 : 각 직업 승급자들이 자리잡고있습니다, 스킬&스펠을 배울수 있는 장소입니다.\\n노비스민가2 : 서브퀘스트를 받을 수 있는 공간입니다.");
                yield return Mes((V)1L, (V)"노비스주점 : 메인퀘스트를 받을수 있는 공간입니다.\\n노비스마을식당 : 음식들을 구매할수있습니다.");
                yield return Mes((V)1L, (V)"지금까지 노비스 마을에 대해 설명을 들어주셔서 감사합니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"기본적인 시스템을 설명해드리겠습니다.");
                yield return Mes((V)1L, (V)"블랙팜온라인은 필드사냥을 지향합니다.");
                yield return Mes((V)1L, (V)"물약은 체력물약을 쓰든 마력물약을 쓰든 쿨타임이 중복됩니다.");
                yield return Mes((V)1L, (V)"EG샵에서는 특수기능 아이템들을 구매하실수 있습니다.");
                yield return Mes((V)1L, (V)"던전을 이용시, 던전마다 클리어 방식이 틀립니다. ex)모든 몬스터 처치, 특정 몬스터 처치");
                yield return Mes((V)1L, (V)"저희 블랙팜온라인에서는 배고픔 시스템이 구현되있습니다.");
                yield return Mes((V)1L, (V)"배고픔수치는 최대 100, 최소 0입니다. 이 수치에 따라 게임에 패널티를 받게됩니다.");
                yield return Mes((V)1L, (V)"배고픔수치 확인법은 상태창(G)의 GP수치를 보시면 됩니다.");
                yield return Mes((V)1L, (V)"배고픔수치가 50이하일시 물리,마법공격력이 1단계로 약해집니다.");
                yield return Mes((V)1L, (V)"다음으로 25이하일시 물리,마법공격력이 2단계로 약해집니다.");
                yield return Mes((V)1L, (V)"마지막으로 배고픔수치가 0이 될시, 체력과 마력 틱이 차지 않으며, 약 2분마다 최대체력의 5%에 해당하는 체력이 소모됩니다.");
                yield return Mes((V)1L, (V)"이때 배고픔으로 소모된 체력은 0이 될수 없습니다.(즉 1에서 더내려가지 않습니다.)");
                yield return Mes((V)1L, (V)"배고픔수치는 음식을 먹음으로써 채울수 있습니다.\\n(음식섭취시 즉시 소량의 체,마회복)");
                yield return Mes((V)1L, (V)"지금까지 기본적인 시스템에 대해 설명을 들어주셔서 감사합니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"당신이 처음오는 모험가이시라면,\\n우선 퀘스트를 클리어 하면서 레벨을 올리시는걸 추천합니다.");
                yield return Mes((V)1L, (V)"노비스마을에서는 레벨 21까지 육성하게 될것이며 그이후에는 새로운 마을로 가실수있습니다.\\n(다시 노비스마을로 올수있다.)");
                yield return Mes((V)1L, (V)"노비스마을의 퀘스트를 받으실려면 주점으로 이동하셔야합니다.");
                yield return Menu((V)"원하신다면 지금바로 이동시켜 드리겠습니다.", (V)"이동한다", (V)"이동하지 않는다");
                v_select = reply.Choice;
                if (V.T(((V)(v_select) == (V)((V)1L))))
                {
                    p.Call("warp", (V)"노비스주점", (V)7L, (V)9L);
                    yield return Mes((V)0L, (V)"이동이 완료되었습니다.");
                }
                else
                    if (V.T(((V)(v_select) == (V)((V)2L))))
                    {
                        yield return Mes((V)1L, (V)"노비스주점은 좌표(32,42)로 가시면 됩니다.\\n(탐색기술로 좌표를 볼수있습니다.)");
                        goto L_re;
                    }
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                p.Call("message", (V)8L, (V)"{=u＊능력치에 대한 고찰＊\\n\\n힘 : 1당 10의 물리공격력이 올라간다.\\n인트 : 1당 10의 마법공격력이 올라간다.\\n위즈 : 레벨업시 올라가는 마나최대치에 위즈만큼 + 해준다. (25 + 위즈)\\n콘 : 레벨업시 올라가는 체력최대치에 콘만큼 + 해준다. (30 + 콘)\\n덱스 : 크리티컬 확률을 결정해준다.\\n\\n물리공격력 : 기본공격대미지와 스킬공격대미지를 정해준다. \\n마법공격력 : 스펠공격대미지를 정해준다.");
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
