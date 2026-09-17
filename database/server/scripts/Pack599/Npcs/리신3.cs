using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 리신3 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_리신3", "5.99표")]
    public class NpcB9ACC2E00033 : PackNpc
    {
        public NpcB9ACC2E00033(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Menu((V)"무도가님, 어느 스킬을 원하십니까?", (V)"쿠라노토[55]", (V)"붕각[59]", (V)"선풍각[63]", (V)"소수신공[70]", (V)"연환포[75]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"쿠라노토는 자신의 체력을 회복하는 2단계 회복기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 55이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)55L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"쿠라노토")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"쿠라노토");
                yield return Mes((V)1L, (V)"쿠라노토를 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"붕각은 전방에 있는 적을 강하게 차는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 59이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)59L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"붕신선각")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"붕각")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"붕각");
                yield return Mes((V)1L, (V)"붕각을 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"선풍각은 4방향에 있는 적을 강하게 차는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 63이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)63L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"파천각")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"선풍각")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"선풍각");
                yield return Mes((V)1L, (V)"선풍각을 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"소수신공은 주먹을 강화하여 대미지를 늘리는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 70이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)70L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"소수신공")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"소수신공");
                yield return Mes((V)1L, (V)"소수신공을 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"연환포는 엄청난 기력을 손에담아 적을 강타하는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 74이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)74L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 심안의 힘으로!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"연환포")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"연환포");
                yield return Mes((V)1L, (V)"연환포을 익히셧습니다. 앞으로 무도가로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
        }
    }
}
