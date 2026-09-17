using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 가렌4 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_가렌4", "5.99표")]
    public class NpcAC00B80C0034 : PackNpc
    {
        public NpcAC00B80C0034(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_select = (V)0L;
            yield return Mes((V)1L, (V)"저는 전사 스킬사범 가렌입니다.");
            yield return Mes((V)1L, (V)"당신이 전사라면, 저의 가르침을 받으시길 바랍니다. 데마시아!");
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)1L))))
            {
                yield return Mes((V)0L, (V)"당신은 전사가 아니라서 저의 가르침을 받을수 없습니다.");
                yield break;
            }
            L_re: ;
            v_myid = p.Call("get_myid");
            yield return Menu((V)"전사님, 어느 스킬을 원하십니까?", (V)"타겟어택[83]", (V)"트리플어택[87]", (V)"크래셔[99]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"타겟어택은 대상을 지정해서 기본공격시마다 타겟을주는 액티브기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 83이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)83L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"타겟어택")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"타겟어택");
                yield return Mes((V)1L, (V)"타겟어택을 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"트리플어택은 기본공격시마다 일정확률로 3번공격하는 패시브기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 87이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)87L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"트리플어택")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"트리플어택");
                yield return Mes((V)1L, (V)"트리플어택를 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"크래셔는 자신의 체력이 최소한으로 남았을때 사용하는 최강의 필살기입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 99이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)99L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"데빌크래셔")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"크래셔")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"크래셔");
                yield return Mes((V)1L, (V)"크래셔를 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
        }
    }
}
