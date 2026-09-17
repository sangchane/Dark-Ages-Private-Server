using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 2차할까 — 5.99 `Npc_Making.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_2차할까", "5.99표")]
    public class Npc0032CC28D560AE4C : PackNpc
    {
        public Npc0032CC28D560AE4C(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_class_sub")) < (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"비승급자는 2차전직을 할수 없습니다.");
                yield break;
            }
            if (V.T(((V)(p.Call("get_class_sub")) > (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"당신은 이미 2차전직을 하셧습니다.");
                yield break;
            }
            if (V.T(((V)(p.Call("get_ac", v_myid)) != (V)((V)100L))))
            {
                yield return Mes((V)1L, (V)"무장을 해체하고 시도해주세요.");
                yield break;
            }
            if (V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)1L))) || V.T(((V)(p.Call("get_class")) == (V)((V)2L))))) || V.T(((V)(p.Call("get_class")) == (V)((V)5L))))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("get_basevita")) < (V)((V)20000L))) || V.T(((V)(p.Call("get_basemana")) < (V)((V)15000L))))))
                {
                    yield return Mes((V)1L, (V)"당신은 아직 2차전직을하기에 어립니다. 격수 2차전직조건은 체력 : 20000 마력 : 15000 입니다.");
                    yield break;
                }
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)3L))) || V.T(((V)(p.Call("get_class")) == (V)((V)4L))))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("get_basevita")) < (V)((V)15000L))) || V.T(((V)(p.Call("get_basemana")) < (V)((V)20000L))))))
                {
                    yield return Mes((V)1L, (V)"당신은 아직 2차전직을하기에 어립니다. 비격수 2차전직조건은 체력 : 15000 마력 : 20000 입니다.");
                    yield break;
                }
            }
            yield return Mes((V)1L, (V)"2차전직을 할려면 제물로 체력과 마력을 3000씩 받쳐야 합니다.");
            yield return Menu((V)"그래도 2차전직을 하시겠습니까?", (V)"2차전직", (V)"나중에..");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                p.Call("set_basevita", ((V)(p.Call("get_basevita", v_myid)) - (V)((V)3000L)));
                p.Call("set_basemana", ((V)(p.Call("get_basemana", v_myid)) - (V)((V)3000L)));
                p.Call("set_class_sub", (V)2L);
                p.Call("game_sound", (V)47L, (V)0L);
                p.Call("effect", v_myid, (V)90L, (V)90L, (V)100L);
                p.Call("legend_add", (V)7L, (V)90L, (V)"블랙팜1년, 2차승급을 하다.");
                p.Call("broadcast", (V)5L, ((V)(((V)((V)"[") + (V)(p.Call("get_name")))) + (V)((V)"]님이 2차승급하셨습니다 축하드립니다")));
                yield break;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"잘 생각하시고 2차전직을 실행하십시오.");
                yield break;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
