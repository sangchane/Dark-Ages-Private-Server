using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 이블린2 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_이블린2", "5.99표")]
    public class NpcC774BE14B9B00032 : PackNpc
    {
        public NpcC774BE14B9B00032(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Menu((V)"도적님, 어느 스킬을 원하십니까?", (V)"더블어택[21]", (V)"습격[30]", (V)"표창던지기[35]", (V)"명중률향상(Lev2)[31]", (V)"하이드[41]", (V)"암살격[50]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"더블어택은 일정확률로 기본공격시 2번 공격하는 패시브기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"더블어택")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"더블어택");
                yield return Mes((V)1L, (V)"더블어택을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"습격은 앞에있는 대상 모르게 공격하는 기술이다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 30이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)30L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"습격진")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"습격")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"습격");
                yield return Mes((V)1L, (V)"습격을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"표창던지기은 전방으로 표창을 던지는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 35이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)35L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"표창던지기")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"표창던지기");
                yield return Mes((V)1L, (V)"표창던지기을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"명중률향상(Lev2)은 크리티컬 확률을 상승시켜주는 패시브 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 31이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)31L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"명중률향상(Lev2)")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"명중률향상(Lev2)");
                p.Call("spell_del", (V)"명중률향상(Lev1)");
                yield return Mes((V)1L, (V)"명중률향상(Lev2)을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"하이드는 자신의 모습을 숨기는 기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 41이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)41L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"하이드")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"하이드");
                yield return Mes((V)1L, (V)"하이드를 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                yield return Mes((V)1L, (V)"암살격은 앞에있는 대상에게 온힘을 다해 공격하는 필사기 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 50이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)50L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"암살격")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"암살격");
                yield return Mes((V)1L, (V)"암살격을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
