using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 아만마을 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_아만마을", "5.99표")]
    public class NpcC544B9CCB9C8C744 : PackNpc
    {
        public NpcC544B9CCB9C8C744(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            if (V.T(((V)(p["#santaclos"]) == (V)((V)1L))))
            {
                goto L_go;
            }
            yield return Mes((V)1L, (V)"콜록, 콜록..");
            yield return Mes((V)1L, (V)"콜록... 어험.. 모험가님 어서오시오.");
            yield return Mes((V)1L, (V)"아만마을 에 오신것을 환영하오..");
            yield return Mes((V)1L, (V)"이마을은 365일 24시간오픈이며, 재미 있다오. 콜록.");
            yield return Mes((V)1L, (V)"얼마전부터 이 평화롭던 아만마을에 이상한 병이 퍼지길 시작했소.\\n내 예상에는 아만정글 과 동굴에 있는 몹에서부터 시작된것같은대..");
            yield return Mes((V)1L, (V)"현재 그 증표들은 직접 채집할수는 없고, 그얼음들을 먹고 병에걸려 변해버린 동물들에게서 얻을수 있을것이오.");
            yield return Mes((V)1L, (V)"내 좀만 젊었다면 직접가서 얻어 왔을터인대.. 너무 늙어버려 몬스터로 변해버린 동물들을 상대할수가 없구려.");
            yield return Mes((V)1L, (V)"모험가분 께서 가서 나대신 구해와 준다면, 악어악세를 드리겠소.");
            p["#santaclos"] = (V)1L;
            L_go: ;
            yield return Menu((V)"아만의증표 5개를 구해온다면 악어악세 를 드리겠소. 교환하겠소?", (V)"교환한다.", (V)"하지않는다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_go;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("item_exist", v_myid, (V)"아만의증표")) < (V)((V)5L))))
                {
                    yield return Mes((V)0L, (V)"보유하고 있는 아만의증표가 없거나 5개 미만이오.");
                    yield break;
                }
                p.Call("item_del", (V)"아만의증표", (V)5L);
                p.Call("item_add", (V)"악어악세", (V)1L);
                yield return Mes((V)0L, (V)"성공적으로 아만의증표와 악어악세교환이 이루어졌소. 앞으로도 수고 하시오.");
                yield break;
            }
            else
            {
                yield break;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
