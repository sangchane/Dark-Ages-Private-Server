using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 바르도 — 5.99 `법사(비전직).txt` 의 SPELL_바르도 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("바르도", "5.99표")]
    public class SpellBC14B974B3C4 : SpellScript
    {
        public SpellBC14B974B3C4(Spell spell) : base(spell)
        {
        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {
        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {
        }

        public override void OnUse(Sprite sprite, Sprite target)
        {
            var p = new Pack599(sprite, target);
            if (!p.Ready)
                return;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(V.B(V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)1L))))) && V.T(p.Call("get_mobdie")))))
            {
                return;
            }
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)50L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 50]");
                return;
            }
            p.Call("manal_del", (V)"50");
            if (V.T(((V)(v_type) != (V)((V)1L))))
            {
                return;
            }
            if (V.T(p.Call("magic_exist", (V)1L, v_target, (V)1L)))
            {
                return;
            }
            p.Call("magic", (V)1L, v_target, (V)"바르도", (V)120L, (V)25L, v_myid);
            p.Call("message", (V)3L, (V)"바르도를 외웠습니다.");
            p.Call("effect", v_target, (V)0L, (V)44L, (V)130L);  // 노바 이펙트(5.99: 0, 258)
            p.Call("game_sound", (V)27L, (V)0L);
        }
    }
}
