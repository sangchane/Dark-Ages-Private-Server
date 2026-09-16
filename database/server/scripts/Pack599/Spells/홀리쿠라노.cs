using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 홀리쿠라노 — 5.99 `Jigja.txt` 의 SPELL_홀리쿠라노 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("홀리쿠라노", "5.99표")]
    public class SpellD640B9ACCFE0B77CB178 : SpellScript
    {
        public SpellD640B9ACCFE0B77CB178(Spell spell) : base(spell)
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
            V v_hill = 0;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)430L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 430이상]");
                return;
            }
            p.Call("manal_del", (V)"430");
            v_hill = ((V)((((V)(p.Call("get_wis", v_myid)) * (V)((V)100L)))) + (V)((V)50000L));
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            p.Call("motion", (V)128L, (V)55L);
            if (V.T(((V)(v_type) == (V)((V)3L))))
            {
                p.Call("set_vita", v_target, ((V)(p.Call("get_vita", v_target)) + (V)(v_hill)));
                p.Call("message", (V)3L, (V)"홀리쿠라노를 외웠습니다.");
                if (V.T(((V)(v_target) != (V)(v_myid))))
                {
                    p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 홀리쿠라노를 외워주셨습니다.")));
                    p.Call("effect", v_target, (V)0L, (V)71L, (V)120L);
                }
                else
                {
                    p.Call("effect", v_target, (V)71L, (V)0L, (V)120L);
                }
                p.Call("game_sound", (V)38L, (V)0L);
            }
        }
    }
}
