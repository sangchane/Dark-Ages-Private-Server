using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 히야트입장 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_히야트입장", "5.99표")]
    public class NpcD788C57CD2B8C785C7A5 : PackNpc
    {
        public NpcD788C57CD2B8C785C7A5(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            yield return Mes((V)1L, (V)"파티 어빌사냥터인 히야트던전으로 이동가능 ");
            yield return Mes((V)1L, (V)"4인파티기준으로 시작하며 매너 지켜주세요");
            yield return Mes((V)1L, (V)"입장료는 40만골드 파티원당 10만원씩 보태세요");
            yield return Mes((V)1L, (V)"그룹도 없는데, 괞히 입장할려하면 돈만 날라갑니다.");
            if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)400000L))))
            {
                yield return Mes((V)0L, (V)"골드가 부족하다네.");
                yield break;
            }
            if (V.T(V.B(!V.T(p.Call("group_exist", v_myid)))))
            {
                p.Call("message", (V)3L, (V)"자네는 그룹이 없지않은가!");
                yield break;
            }
            if (V.T(((V)(p.Call("group_count", v_myid, (V)0L)) < (V)((V)4L))))
            {
                yield return Mes((V)0L, (V)"현재 이공간에 같은그룹원이 4명이상이 되지 않다네.");
                yield break;
            }
            if (V.T(((V)(p.Call("group_class_sub2", v_myid)) == (V)((V)1L))))
            {
                yield return Mes((V)0L, (V)"그룹원중 비승급자가 존재해서 입장할수 없다네.");
                yield break;
            }
            yield return Menu((V)"이동할껀가요?", (V)"히야트던전 이동");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                p.Call("group_warp", (V)"히야트던전A-1", (V)36L, (V)57L);
                p.Call("money_del", (V)400000L);
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
