using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 이블린3 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_이블린3", "5.99표")]
    public class NpcC774BE14B9B00033 : PackNpc
    {
        public NpcC774BE14B9B00033(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_select = (V)0L;
            yield return Mes((V)1L, (V)"저는 도적 스킬사범 이블린입니다.");
            yield return Mes((V)1L, (V)"당신이 도적이라면, 저의 가르침을 받으시길 바랍니다.");
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)2L))))
            {
                yield return Mes((V)0L, (V)"당신은 도적이 아니라서 저의 가르침을 받을수 없습니다.");
                yield break;
            }
            L_re: ;
            v_myid = p.Call("get_myid");
            yield return Menu((V)"도적님, 어느 스킬을 원하십니까?", (V)"찔러휘비기[57]", (V)"소매치기[62]", (V)"만개표창[71]", (V)"원기지옥[77]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"찔러휘비기는 앞에있는 몬스터및 적에게 대미지를 입히는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 57이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)57L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"후벼쑤시기")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"찔러휘비기")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"찔러휘비기");
                yield return Mes((V)1L, (V)"찔러휘비기을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"소매치기는 앞에있는 몬스터에게서 돈을 훔치는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 62이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)62L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"소매치기")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"소매치기");
                yield return Mes((V)1L, (V)"소매치기을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"만개표창은 자신의 주변으로 표창을 던지는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 71이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)71L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"만개표창")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"만개표창");
                yield return Mes((V)1L, (V)"만개표창을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"원기지옥은 몬스터에게 대미지를 입히고 체력을 흡수하는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 74이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)74L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"원기지옥")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"원기지옥");
                yield return Mes((V)1L, (V)"원기지옥을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
