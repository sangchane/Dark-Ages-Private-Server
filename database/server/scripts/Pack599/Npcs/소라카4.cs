using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 소라카4 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_소라카4", "5.99표")]
    public class NpcC18CB77CCE740034 : PackNpc
    {
        public NpcC18CB77CCE740034(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Menu((V)"성직자님, 어느 스킬을 원하십니까?", (V)"일루메나[81]", (V)"수페라쿠라노[83]", (V)"쿠라네라[87]", (V)"코마디아[93]", (V)"엑스쿠라노[99]", (V)"엑스쿠라네라[99]", (V)"리베라토[99]");
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
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"일루메나")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"일루메나");
                yield return Mes((V)1L, (V)"일루메나를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"수페라쿠라노는 대상의 체력을 회복 시키는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 83이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)83L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"수페라쿠라노")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"수페라쿠라노");
                yield return Mes((V)1L, (V)"수페라쿠라노를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"쿠라네라는 그룹원들의 체력을 회복하는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 87이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)87L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"쿠라네라")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"쿠라네라");
                yield return Mes((V)1L, (V)"쿠라네라를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"코마디아는 자신 앞의 유저의 코마상태를 치료하는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 93이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)93L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"코마디아")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"코마디아");
                yield return Mes((V)1L, (V)"코마디아를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"엑스쿠라노는 대상의 체력을 회복 시키는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 99이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)99L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"엑스쿠라노")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"엑스쿠라노");
                yield return Mes((V)1L, (V)"엑스쿠라노를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                yield return Mes((V)1L, (V)"엑스쿠라네라는 그룹원들의 체력을 회복하는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 99이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)99L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"엑스쿠라네라")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"엑스쿠라네라");
                yield return Mes((V)1L, (V)"엑스쿠라네라를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)7L))))
            {
                yield return Mes((V)1L, (V)"리베라토는 대상의 모든마법을 푸는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 99이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)99L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"리베라토")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"리베라토");
                yield return Mes((V)1L, (V)"리베라토를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
