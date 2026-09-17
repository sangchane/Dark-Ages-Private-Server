using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 달인 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_달인", "5.99표")]
    public class NpcB2ECC778 : PackNpc
    {
        public NpcB2ECC778(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V h_EG = 0;
            V v_myid = 0;
            V v_select = 0;

            v_select = (V)0L;
            v_myid = p.Call("get_myid");
            L_re: ;
            yield return Menu(((V)((V)"어서오게나, 나는 생활의 달인이야!\\n나에게 스킬을 전수받겠나?\\n보유EG : ") + (V)(h_EG)), (V)"아들레스투[1000EG]", (V)"센스[1500EG]", (V)"품뒤져보기[5000EG]", (V)"휴식[1000EG]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"아들레스투는 첫번째 슬롯에있는 아이템의 정보를 볼수있게 해준다네.\\n가격은 1000EG야.");
                if (V.T(((V)(h_EG) < (V)((V)1000L))))
                {
                    yield return Mes((V)0L, (V)"EG가 부족하지 않는가!");
                    yield break;
                }
                if (V.T(p.Call("spell_exist", (V)"아들레스투")))
                {
                    yield return Mes((V)1L, (V)"이미 너는 아들레스투가 있어.");
                    goto L_re;
                }
                h_EG = ((V)(h_EG) - (V)((V)1000L));
                p.Call("spell_add", (V)"아들레스투");
                yield return Mes((V)0L, (V)"아들레스투를 성공적으로 익혔다네.\\n스펠창을 확인해 보게나.");
                yield break;
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)1L, (V)"센스는 앞에있는 유저의 정보를 확인할수 있다네.\\n가격은 1500EG야.");
                    if (V.T(((V)(h_EG) < (V)((V)1500L))))
                    {
                        yield return Mes((V)0L, (V)"EG가 부족하지 않는가!");
                        yield break;
                    }
                    if (V.T(p.Call("skill_exist", (V)"센스")))
                    {
                        yield return Mes((V)1L, (V)"이미 너는 센스가 있어.");
                        goto L_re;
                    }
                    h_EG = ((V)(h_EG) - (V)((V)1500L));
                    p.Call("skill_add", (V)"센스");
                    yield return Mes((V)0L, (V)"센스를 성공적으로 익혔다네.\\n스킬창을 확인해 보게나.");
                    yield break;
                }
                else
                    if (V.T(((V)(v_select) == (V)((V)3L))))
                    {
                        yield return Mes((V)1L, (V)"품뒤져보기는 앞에있는 유저의 아이템정보를 확인할수 있다네. 가격은 5000EG야.");
                        if (V.T(((V)(h_EG) < (V)((V)5000L))))
                        {
                            yield return Mes((V)0L, (V)"EG가 부족하지 않는가!");
                            yield break;
                        }
                        if (V.T(p.Call("skill_exist", (V)"품뒤져보기")))
                        {
                            yield return Mes((V)1L, (V)"이미 너는 품뒤져보기가 있어.");
                            goto L_re;
                        }
                        h_EG = ((V)(h_EG) - (V)((V)5000L));
                        p.Call("skill_add", (V)"품뒤져보기");
                        yield return Mes((V)0L, (V)"품뒤져보기를 성공적으로 익혔다네.\\n스킬창을 확인해 보게나.");
                        yield break;
                    }
                    else
                        if (V.T(((V)(v_select) == (V)((V)4L))))
                        {
                            yield return Mes((V)1L, (V)"휴식은 제자리에서 가만히 쉬는것이라네. 가격은 1000EG야.");
                            if (V.T(((V)(h_EG) < (V)((V)1000L))))
                            {
                                yield return Mes((V)0L, (V)"EG가 부족하지 않는가!");
                                yield break;
                            }
                            if (V.T(p.Call("spell_exist", (V)"휴식")))
                            {
                                yield return Mes((V)1L, (V)"이미 너는 휴식이 있어.");
                                goto L_re;
                            }
                            h_EG = ((V)(h_EG) - (V)((V)1000L));
                            p.Call("spell_add", (V)"휴식");
                            yield return Mes((V)0L, (V)"휴식을 성공적으로 익혔다네.\\n스펠창을 확인해 보게나.");
                            yield break;
                        }
        }
    }
}
