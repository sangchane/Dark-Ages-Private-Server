using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 소라카2 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_소라카2", "5.99표")]
    public class NpcC18CB77CCE740032 : PackNpc
    {
        public NpcC18CB77CCE740032(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Menu((V)"성직자님, 어느 스킬을 원하십니까?", (V)"에나르마[21]", (V)"디나르콜리[21]", (V)"디소루마[21]", (V)"쿠라노[21]", (V)"이모탈[41]", (V)"안티매직[50]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"에나르마는 일정시간동안 물리,마법공격력이 증가하는 버프 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"에나르마")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"에나르마");
                yield return Mes((V)1L, (V)"에나르마를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"디나르콜리는 유저의 나르콜리를 디버프 시키는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"디나르콜리")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"디나르콜리");
                yield return Mes((V)1L, (V)"디나르콜리를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"디소루마는 유저의 소루마를 디버프 시키는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"디소루마")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"디소루마");
                yield return Mes((V)1L, (V)"디소루마를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"쿠라노는 타겟의 체력을 회복시키는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"쿠라노")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"쿠라노");
                yield return Mes((V)1L, (V)"쿠라노를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"이모탈은 일정시간동안 무적이되는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 41이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)41L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"이모탈")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"이모탈");
                yield return Mes((V)1L, (V)"이모탈를 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                yield return Mes((V)1L, (V)"안티매직은 일정시간동안 타겟에게 몬스터 마법공격을 확률적으로 막아주는 방어막을 생성시켜줍니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 50이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)50L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 별의 부름을!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"안티매직")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"안티매직");
                yield return Mes((V)1L, (V)"안티매직을 익히셧습니다. 앞으로 성직자로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
