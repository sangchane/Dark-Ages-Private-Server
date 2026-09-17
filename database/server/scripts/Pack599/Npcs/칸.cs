using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 칸 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_칸", "5.99표")]
    public class NpcCE78 : PackNpc
    {
        public NpcCE78(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_auto_cnt = 0;
            V v_auto_hp = 0;
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            L_re: ;
            if (V.T(((V)(p.Call("get_level")) != (V)((V)99L))))
            {
                yield return Mes((V)1L, (V)"능력치를 사기에는 어립니다.");
                yield break;
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class_sub")) == (V)((V)0L))) && V.T(((V)(p.Call("get_basemana", v_myid)) > (V)((V)100000L))))))
            {
                yield return Mes((V)1L, (V)"승급을 하셔야 마력을 사실수있습니다.");
                yield break;
            }
            if (V.T(((V)(v_auto_hp) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"이미 경험치를 팔고 있습니다.");
                yield break;
            }
            yield return Menu((V)"마력을 사시겠습니까? 마력을 살때는 경험치가 필요합니다.", (V)"마력을 산다.", (V)"나중에 산다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Input((V)"경험치를 파셔서 마력을살 횟수를 적어주세요.");
                v_auto_cnt = reply.Words;
                if (V.T(((V)(v_auto_cnt) <= (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"제대로 입력해 주세요.");
                    yield break;
                }
                else
                {
                    p["#auto_hp"] = (V)1L;
                    while (V.T(v_auto_cnt))
                    {
                        if (V.T(((V)(p.Call("get_ac", v_myid)) != (V)((V)100L))))
                        {
                            p["#auto_hp"] = (V)0L;
                            yield return Mes((V)1L, (V)" 무장해제하시고 다시시도 해주세요.");
                            yield break;
                        }
                        if (V.T(V.B(V.T(((V)(p.Call("get_baseexp", v_myid)) >= (V)(p.Call("get_basemana2", v_myid)))) || V.T(V.B(V.T(((V)(p.Call("get_baseexp", v_myid)) < (V)((V)0L))) && V.T(((V)(p.Call("get_baseexp", v_myid)) <= (V)((-(V)((V)2147483648L))))))))))
                        {
                            p.Call("exp_del", ((V)(p.Call("get_basemana2", v_myid)) * (V)((V)500L)));
                            p.Call("set_basemana", ((V)(p.Call("get_basemana2", v_myid)) + (V)((V)25L)));
                        }
                        else
                        {
                            p["#auto_hp"] = (V)0L;
                            yield return Mes((V)1L, (V)"경험치가 부족합니다.");
                            break;
                        }
                        p.Call("sleep", (V)1L);
                        v_auto_cnt = ((V)(v_auto_cnt) - (V)((V)1L));
                    }
                }
                p["#auto_hp"] = (V)0L;
                yield break;
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield break;
                }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
