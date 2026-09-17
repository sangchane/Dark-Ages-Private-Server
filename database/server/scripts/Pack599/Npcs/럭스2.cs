using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 럭스2 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_럭스2", "5.99표")]
    public class NpcB7EDC2A40032 : PackNpc
    {
        public NpcB7EDC2A40032(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_select = (V)0L;
            yield return Mes((V)1L, (V)"저는 마법사 스킬사범 럭스입니다.");
            yield return Mes((V)1L, (V)"당신이 마법사라면, 저의 가르침을 받으시길 바랍니다. 데마시아!");
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)3L))))
            {
                yield return Mes((V)0L, (V)"당신은 마법사가 아니라서 저의 가르침을 받을수 없습니다.");
                yield break;
            }
            L_re: ;
            v_myid = p.Call("get_myid");
            yield return Menu((V)"마법사님, 어느 스킬을 원하십니까?", (V)"수페라마레나[21]", (V)"바르도[21]", (V)"마레누스[31]", (V)"나르콜리[41]", (V)"세멜리아[50]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"수페라마레나는 자신의 4방향의 적에게 대미지를 가하는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"수페라마레나")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"수페라마레나");
                yield return Mes((V)1L, (V)"수페라마레나를 익히셧습니다. 앞으로 마법사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"바르도는 타겟의 방어력을 깍는 저주 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 21이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)21L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"바르도")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"바르도");
                yield return Mes((V)1L, (V)"바르도를 익히셧습니다. 앞으로 마법사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"마레누스는 타겟에게 대미지를 가하는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 31이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)31L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"마레누스")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"마레누스");
                yield return Mes((V)1L, (V)"마레누스를 익히셧습니다. 앞으로 마법사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"나르콜리는 일정시간동안 적을 잠재우는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 41이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)41L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"나르콜리")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"나르콜리");
                yield return Mes((V)1L, (V)"나르콜리를 익히셧습니다. 앞으로 마법사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"세멜리아는 자신의 마나를 전부 소모하여 공격을 가하는 필살기 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 50이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)50L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"세멜리아")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"세멜리아");
                yield return Mes((V)1L, (V)"세멜리아를 익히셧습니다. 앞으로 마법사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
        }
    }
}
