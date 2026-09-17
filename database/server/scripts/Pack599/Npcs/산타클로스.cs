using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 산타클로스 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_산타클로스", "5.99표")]
    public class NpcC0B0D0C0D074B85CC2A4 : PackNpc
    {
        public NpcC0B0D0C0D074B85CC2A4(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Mes((V)1L, (V)"크리스마스 마을에 오신것을 환영하오..");
            yield return Mes((V)1L, (V)"이마을은 365일 24시간 겨울이며, 눈이 온다오. 콜록.");
            yield return Mes((V)1L, (V)"얼마전부터 이 평화롭던 크리스마스마을에 이상한 병이 퍼지길 시작했소.\\n내 예상에는 마운틴메리에 있는 얼음에서부터 시작된것같은대..");
            yield return Mes((V)1L, (V)"현재 그 얼음들은 직접 채집할수는 없고, 그얼음들을 먹고 병에걸려 변해버린 동물들에게서 얻을수 있을것이오.");
            yield return Mes((V)1L, (V)"내 좀만 젊었다면 직접가서 얻어 왔을터인대.. 너무 늙어버려 몬스터로 변해버린 동물들을 상대할수가 없구려.");
            yield return Mes((V)1L, (V)"모험가분 께서 가서 나대신 구해와 준다면, 선물보따리를 드리겠소.");
            p["#santaclos"] = (V)1L;
            L_go: ;
            yield return Menu((V)"크리스마스얼음 10개를 구해올때마다 1개의 선물보따리를 드리겠소. 교환하겠소?", (V)"교환한다.", (V)"하지않는다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_go;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("item_exist", v_myid, (V)"크리스마스얼음")) < (V)((V)10L))))
                {
                    yield return Mes((V)0L, (V)"보유하고 있는 크리스마스얼음이 없거나 10개 미만이오.");
                    yield break;
                }
                p.Call("item_del", (V)"크리스마스얼음", (V)10L);
                p.Call("item_add", (V)"크리스마스선물주머니", (V)1L);
                yield return Mes((V)0L, (V)"성공적으로 크리스마스얼음과 선물주머니교환이 이루어졌소. 앞으로도 계속 가져와주시오.");
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
