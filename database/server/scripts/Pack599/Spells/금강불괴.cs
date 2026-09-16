using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 금강불괴 — 5.99 `무도가(비전직).txt` 의 SPELL_금강불괴 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("금강불괴", "5.99표")]
    public class SpellAE08AC15BD88AD34 : SpellScript
    {
        public SpellAE08AC15BD88AD34(Spell spell) : base(spell)
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
            V v_rnd = 0;
            V v_target = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)500L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 500]");
                return;
            }
            p.Call("manal_del", (V)"500");
            v_target = p.Call("spell_target");
            L_RE: ;
            v_rnd = p.Call("rand", (V)1L, (V)10L);
            if (V.T(((V)(v_rnd) <= (V)((V)0L))))
            {
                goto L_RE;
            }
            if (V.T(((V)(v_rnd) > (V)((V)4L))))
            {
                if (V.T(((V)(p.Call("immortal", v_myid, (V)8L)) == (V)((V)1L))))
                {
                    p.Call("effect", v_myid, (V)6L, (V)0L, (V)100L);
                    p.Call("game_sound", (V)8L, (V)0L);
                    p.Call("message", (V)3L, (V)"금강불괴을 외웠습니다.");
                }
                return;
            }
            p.Call("message", (V)3L, (V)"실패했습니다.");
        }
    }
}
