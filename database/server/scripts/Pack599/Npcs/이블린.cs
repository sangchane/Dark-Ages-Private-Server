using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 이블린 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_이블린", "5.99표")]
    public class NpcC774BE14B9B0 : PackNpc
    {
        public NpcC774BE14B9B0(GameServer server, Mundane mundane) : base(server, mundane)
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
            yield return Menu((V)"도적님, 어느 스킬을 원하십니까?", (V)"마구찌르기[11]", (V)"명중률향상[11]", (V)"아무네지아[15]");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"마구찌르기는 찌르기의 대미지를 상승시켜주는 패시브 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 11이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)11L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"마구찌르기")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"마구찌르기");
                yield return Mes((V)1L, (V)"마구찌르기를 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"명중률향상(Lev1)은 크리티컬 확률을 상승시켜주는 패시브 입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 11이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)11L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"명중률향상(Lev2)")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬의 상위스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                if (V.T(p.Call("spell_exist", (V)"명중률향상(Lev1)")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("spell_add", (V)"명중률향상(Lev1)");
                yield return Mes((V)1L, (V)"명중률향상(Lev1)을 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"아무네지아는 적의 공격능력을 일정시간동안 상실시키는 기술입니다.");
                yield return Mes((V)1L, (V)"배우실려면 레벨이 15이상이셔야 합니다.");
                if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)15L))))
                {
                    yield return Mes((V)1L, (V)"이스킬을 습득하시기엔 아직 어립니다.");
                    goto L_re;
                }
                if (V.T(p.Call("skill_exist", (V)"아무네지아")))
                {
                    yield return Mes((V)1L, (V)"이미 이 스킬을 습득 하셧습니다.");
                    goto L_re;
                }
                p.Call("skill_add", (V)"아무네지아");
                yield return Mes((V)1L, (V)"아무네지아를 익히셧습니다. 앞으로 도적으로서 사명감을 가져주시길 바랍니다.");
                goto L_re;
            }
        }
    }
}
