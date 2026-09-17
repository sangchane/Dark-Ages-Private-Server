using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 뮤레칸 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_뮤레칸", "5.99표")]
    public class NpcBBA4B808CE78 : PackNpc
    {
        public NpcBBA4B808CE78(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            yield return Mes((V)1L, (V)"생명의 소중함을 그렇게 일러왔거늘... 앞으로도 사소한 일에 목숨을 걸지 않으리라고 내가 어떻게 믿을 수 있게느냐? 또 생명을 잃고 나를 찾아오지 않겠다고 맹세할 수 있겠느냐?");
            yield return Mes((V)1L, (V)"싸움은 싸움을 부르고, 피는 반드시 피를 보게 되느니라. 이번의 죽음도 네 책임이라는 것을 진심으로 느끼고 반성하고 있으냐?");
            yield return Mes((V)1L, (V)"그렇다면 잃은 물건과 경험치도 다 네 욕심에서 비롯되었음을 인정하겠느냐?");
            yield return Mes((V)1L, (V)"네가 새로 생명을 얻게 되더라도 절대로 무고한 생명을 해치지 않을 것을 맹세하느냐?");
            p.Call("message", (V)3L, (V)"뮤레칸님이 신의기원을 외워주셨습니다.");
            p.Call("set_vita", v_myid, (V)1000L);
            p.Call("set_state", (V)0L, (V)0L);
            if (V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))
            {
                p.Call("set_body", v_myid, (V)16L);
            }
            else
            {
                p.Call("set_body", v_myid, (V)32L);
            }
            if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
            {
                p.Call("warp", (V)"노비스마을", (V)37L, (V)29L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_level", v_myid)) > (V)((V)20L))) && V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)51L))))))
            {
                p.Call("warp", (V)"수오미마을", (V)39L, (V)19L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_level", v_myid)) > (V)((V)50L))) && V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)81L))))))
            {
                p.Call("warp", (V)"아벨마을", (V)57L, (V)24L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_level", v_myid)) > (V)((V)80L))) && V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)99L))))))
            {
                p.Call("warp", (V)"로톤마을", (V)33L, (V)8L);
            }
            if (V.T(((V)(p.Call("get_level", v_myid)) >= (V)((V)99L))))
            {
                p.Call("warp", (V)"마인여관", (V)7L, (V)11L);
            }
            yield return Mes((V)0L, (V)"너의 각오를 믿고, 다시 새로운 생명을 부여해주니 앞으로 보는일은 없도록 하여라.");
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
