using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 화론법사 — 5.99 `Npc_Skill.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_화론법사", "5.99표")]
    public class NpcD654B860BC95C0AC : PackNpc
    {
        public NpcD654B860BC95C0AC(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            L_re: ;
            v_myid = p.Call("get_myid");
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)3L))) || V.T(((V)(p.Call("get_class_sub")) != (V)((V)2L))))))
            {
                yield return Mes((V)1L, (V)"소환사가 아닙니다.");
                yield break;
            }
            yield return Menu((V)"2차승급기술을 가르쳐드립니다.", (V)"화이라소환(Lev1)", (V)"화이라소환(Lev2)", (V)"화이라소환(Lev3)", (V)"칸의눈(Lev1)", (V)"매직프로텍션");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("spell_exist", (V)"화이라소환(Lev1)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 화이라소환(Lev1)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("spell_exist", (V)"화이라소환(Lev2)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 화이라소환(Lev2)를 배웠습니다.");
                    goto L_re;
                }
                if (V.T(((V)(p.Call("spell_exist", (V)"화이라소환(Lev3)")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 화이라소환(Lev3)를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"화이라소환(Lev1) 조건 : 2차승급");
                if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))))
                {
                    p.Call("spell_add", (V)"화이라소환(Lev1)");
                    yield return Mes((V)1L, (V)"화이라소환(Lev1) 스킬을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"화이라소환(Lev1)를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                if (V.T(((V)(p.Call("spell_exist", (V)"세오의손길")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 세오의손길를 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"세오의손길 조건 : 2차승급 어빌리티레벨 20 재료 : 옐로토닉7개, 블루토닉7개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)20L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"옐로토닉")) >= (V)((V)7L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"블루토닉")) >= (V)((V)7L))))))
                {
                    p.Call("spell_add2", (V)"세오의손길");
                    p.Call("item_del", (V)"옐로토닉", (V)7L);
                    p.Call("item_del", (V)"블루토닉", (V)7L);
                    yield return Mes((V)1L, (V)"세오의손길 스펠을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"세오의손길를 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                if (V.T(((V)(p.Call("spell_exist", (V)"매직프로텍션")) == (V)((V)1L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 매직프로텍션을 배웠습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"매직프로텍션 조건 : 2차승급 어빌리티레벨 15 재료 : 메가토닉5개");
                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(p.Call("get_class_sub")) == (V)((V)2L))))) && V.T(((V)(p.Call("get_ability")) >= (V)((V)15L))))) && V.T(((V)(p.Call("item_exist", v_myid, (V)"메가토닉")) >= (V)((V)5L))))))
                {
                    p.Call("spell_add2", (V)"매직프로텍션");
                    p.Call("item_del", (V)"메가토닉", (V)5L);
                    yield return Mes((V)1L, (V)"매직프로텍션 스펠을 익혔습니다.");
                    goto L_re;
                }
                yield return Mes((V)1L, (V)"매직프로텍션을 배우기에는 능력치가 충족되지 않습니다.");
                goto L_re;
            }
        }
    }
}
