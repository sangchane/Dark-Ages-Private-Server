using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 리신4 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_리신4", "5.99표")]
    public class NpcB9ACC2E00034 : PackNpc
    {
        public NpcB9ACC2E00034(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_select = (V)0L;
            yield return Mes((V)1L, (V)"저는 무도가 스킬사범 리신입니다.");
            yield return Mes((V)1L, (V)"당신이 무도가라면, 저의 가르침을 받으시길 바랍니다. 심안의 힘으로!");
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)5L))))
            {
                yield return Mes((V)0L, (V)"당신은 무도가가 아니라서 저의 가르침을 받을수 없습니다.");
                yield break;
            }
            L_re: ;
            v_myid = p.Call("get_myid");
            yield return Menu((V)"무도가님, 어느 스킬을 원하십니까?", (V)"일루메나[81]", (V)"달마신공[81]", (V)"일음지[83]", (V)"백보신권[87]", (V)"발경[93]", (V)"다라밀공[99]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"일루메나는 대상의 시야를 회복해주는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 81이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)81L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"일루메나")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"일루메나");
                yield return Mes((V)1L, (V)"일루메나를 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"달마신공은 앞에있는 타겟의 방어력및 무적상태를 무시하고 대미지를 입히는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 81이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)81L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"달마신공")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"달마신공");
                yield return Mes((V)1L, (V)"달마신공를 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"일음지는 앞에있는 적의 눈을 멀게하는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 83이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)83L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"일음지")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"일음지");
                yield return Mes((V)1L, (V)"일음지을 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"백보신권은 전방 3칸의 몬스터에게 타격을 입히는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 87이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)87L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"백보신권")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"백보신권");
                yield return Mes((V)1L, (V)"백보신권을 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"발경은 앞의 몬스터에게 스턴을 입히는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 93이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)93L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"발경")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"발경");
                yield return Mes((V)1L, (V)"발경을 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                yield return Mes((V)1L, (V)"다라밀공은 자신의 모든 체력과 마력을 소모하여 타겟에게 엄청난 대미지를 주는 필살기 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 99이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)99L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"다라밀공")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"다라밀공");
                yield return Mes((V)1L, (V)"다라밀공을 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
