using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 럭스3 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_럭스3", "5.99표")]
    public class NpcB7EDC2A40033 : PackNpc
    {
        public NpcB7EDC2A40033(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Menu((V)"마법사님, 어느 스킬을 원하십니까?", (V)"데프레코[55]", (V)"마네나로[73]", (V)"침묵[77]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"데프레코는 저주 마법입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 55이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)55L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"데프레코")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"데프레코");
                yield return Mes((V)1L, (V)"데프레코를 익히셧습니다. 앞으로 마법사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"마네나로는 자신의 시야에있는 모든 몬스터에게 공격을 가하는 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 71이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)71L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"마네나로")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"마네나로");
                yield return Mes((V)1L, (V)"마네나로를 익히셧습니다. 앞으로 마법사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"침묵은 몬스터의 마법사용을 막는 저주 마법 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 74이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)74L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"침묵")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"침묵");
                yield return Mes((V)1L, (V)"침묵를 익히셧습니다. 앞으로 마법사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
        }
    }
}
