using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 코마디아 — 5.99 `성직자(비전직).txt` 의 SPELL_코마디아 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("코마디아", "5.99표")]
    public class SpellCF54B9C8B514C544 : SpellScript
    {
        public SpellCF54B9C8B514C544(Spell spell) : base(spell)
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

            v_myid = p.Call("get_myid");
            v_target = p.Call("get_front_char", v_myid);
            if (V.T(V.B(!V.T(v_target))))
            {
                return;
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_state1", v_target)) != (V)((V)1L))) && V.T(((V)(p.Call("get_coma", v_target)) == (V)((V)0L))))))
            {
                return;
            }
            p.Call("set_state1", v_target, (V)0L);
            p.Call("set_coma", v_target, (V)0L);
            p.Call("coma_delay", v_target, (V)0L);
            p.Call("del_coma", v_target);
            p.Call("set_vita", v_target, (V)1000L);
            p.Call("set_mana", v_target, (V)1000L);
            p.Call("effect", v_target, (V)0L, (V)5L, (V)75L);
            p.Call("game_sound", (V)8L, (V)0L);
        }
    }
}
