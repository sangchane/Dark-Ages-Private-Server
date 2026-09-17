using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 소라카3 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_소라카3", "5.99표")]
    public class NpcC18CB77CCE740033 : PackNpc
    {
        public NpcC18CB77CCE740033(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_select = (V)0L;
            yield return Mes((V)1L, (V)"저는 성직자 스킬사범 소라카입니다.");
            yield return Mes((V)1L, (V)"당신이 성직자라면, 저의 가르침을 받으시길 바랍니다. 별의 부름을!");
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)4L))))
            {
                yield return Mes((V)0L, (V)"당신은 성직자가 아니라서 저의 가르침을 받을수 없습니다.");
                yield break;
            }
            L_re: ;
            v_myid = p.Call("get_myid");
            yield return Menu((V)"성직자님, 어느 스킬을 원하십니까?", (V)"쿠라노소[55]", (V)"쿠라누스[63]", (V)"홀리랜서[73]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"쿠라노소는 대상의 체력을 회복시켜주는 힐링 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 55이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)55L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"쿠라노소")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"쿠라노소");
                yield return Mes((V)1L, (V)"쿠라노소를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"쿠라누스는 그룹원들의 체력을 회복 시키는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 63이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)63L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"쿠라누스")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"쿠라누스");
                yield return Mes((V)1L, (V)"쿠라누스를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"홀리랜서는 적을 신성의 힘으로 공격하는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 73이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)73L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"홀리랜서")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"홀리랜서");
                yield return Mes((V)1L, (V)"홀리랜서를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
