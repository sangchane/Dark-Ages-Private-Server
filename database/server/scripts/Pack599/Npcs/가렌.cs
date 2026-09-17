using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 가렌 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_가렌", "5.99표")]
    public class NpcAC00B80C : PackNpc
    {
        public NpcAC00B80C(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Menu((V)"전사님, 어느 스킬을 원하십니까?", (V)"숏블레이드[5]", (V)"더블어택[7]", (V)"윈드블레이드[11]", (V)"파워단련[11]", (V)"내려치기[16]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"숏블레이드는 전방 1칸을 공격하는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 11이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)5L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                p.Call("skill_add", (V)"숏블레이드");
                yield return Mes((V)1L, (V)"숏블레이드를 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"더블어택은 확률적으로 기본공격을 두번하는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 11이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)7L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                p.Call("skill_add", (V)"더블어택");
                yield return Mes((V)1L, (V)"더블어택을 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"윈드블레이드는 전방 3칸을 공격하는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 11이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)11L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"룬블레이드")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"윈드블레이드")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"윈드블레이드");
                yield return Mes((V)1L, (V)"윈드블레이드를 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"파워단련(은 숏블레이드의 대미지를 상승시켜주는 패시브 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 11이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)11L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"파워단련")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"파워단련");
                yield return Mes((V)1L, (V)"파워단련을 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"내려치기는 적에게 강력한 대미지를 가한후 스턴을 거는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 16이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)16L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다. 데마시아!");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"내려치기")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"내려치기");
                yield return Mes((V)1L, (V)"내려치기를 익히셧습니다. 앞으로 전사로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
