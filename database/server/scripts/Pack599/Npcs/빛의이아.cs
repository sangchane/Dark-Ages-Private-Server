using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 빛의이아 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_빛의이아", "5.99표")]
    public class NpcBE5BC758C774C544 : PackNpc
    {
        public NpcBE5BC758C774C544(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            yield return Mes((V)1L, (V)"황폐해진 영혼이여...안심하세요...여기는 빛의 신전...저는 빛과 치유의 여신 이아입니다.");
            yield return Mes((V)1L, (V)"험한 싸움에서 패배하셧다고 너무 낙심하지 마세요. 제가 당신을 위해 해드릴 수있는 것은 영혼을 새롭게 해드리는 것 뿐이지만, 그것으로 당신이 새롭게 도전할수 있길 바랍니다.");
            yield return Mes((V)1L, (V)"자 일어나세요...신의 축복이 당신에게 가득하기를...");
            L_re: ;
            yield return Menu((V)"당신이 돌아가야 할 곳이 있나요?", (V)"마을로 가길 원합니다.", (V)"밀레스대련장으로 가길 원합니다.", (V)"수오미대련장으로 가길 원합니다.", (V)"루어스대련장으로 가길 원합니다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                p.Call("set_vita", v_myid, p.Call("get_basevita"));
                p.Call("set_state", (V)0L, (V)0L);
                if (V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))
                {
                    p.Call("set_body", v_myid, (V)16L);
                }
                else
                {
                    p.Call("set_body", v_myid, (V)32L);
                }
                p.Call("warp", (V)"뤼케시온마을", (V)42L, (V)27L);
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                p.Call("warp", (V)"밀레스대련장", (V)4L, (V)4L);
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                p.Call("warp", (V)"수오미대련장", (V)4L, (V)4L);
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                p.Call("warp", (V)"루어스대련장", (V)4L, (V)4L);
                yield break;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
