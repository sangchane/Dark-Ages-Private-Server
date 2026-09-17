using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 가렌2 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_가렌2", "5.99표")]
    public class NpcAC00B80C0032 : PackNpc
    {
        public NpcAC00B80C0032(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Menu((V)"전사님, 어느 스킬을 원하십니까?", (V)"바투[21]", (V)"메가블레이드[21]", (V)"완전방어[41]", (V)"매드소울[50]");
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
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"바투")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"바투");
                yield return Mes((V)1L, (V)"바투를 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"메가블레이드는 자신을 기준으로 4방향을 공격하는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"스톰블레이드")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"메가블레이드")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"메가블레이드");
                yield return Mes((V)1L, (V)"메가블레이드을 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"완전방어는 적의 일반공격을 모두 방어하는 버프기술 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 41이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)41L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"완전방어")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"완전방어");
                yield return Mes((V)1L, (V)"완전방어를 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"매드소울은 자신의 체력을 소모하여 엄청난 대미지를 입히는 필살기 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 50이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)50L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"매드소울진")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"매드소울")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"매드소울");
                yield return Mes((V)1L, (V)"매드소울를 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
        }
    }
}
