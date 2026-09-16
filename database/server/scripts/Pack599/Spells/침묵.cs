using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 침묵 — 5.99 `법사(비전직).txt` 의 SPELL_침묵 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("침묵", "5.99표")]
    public class SpellCE68BB35 : SpellScript
    {
        public SpellCE68BB35(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)50L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 50이상]");
                return;
            }
            p.Call("manal_del", (V)"50");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)1L))))) && V.T(p.Call("get_mobdie")))) || V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)3L))))))))
            {
                return;
            }
            if (V.T(p.Call("silence", v_target, (V)10L)))
            {
                p.Call("message", (V)3L, (V)"침묵을 시전 하였습니다.");
                p.Call("effect", v_target, (V)0L, (V)25L, (V)80L);
                p.Call("game_sound", (V)6L, (V)0L);
            }
        }
    }
}
