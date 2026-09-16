using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 다라밀공 — 5.99 `무도가(비전직).txt` 의 SPELL_다라밀공 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("다라밀공", "5.99표")]
    public class SpellB2E4B77CBC00ACF5 : SpellScript
    {
        public SpellB2E4B77CBC00ACF5(Spell spell) : base(spell)
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
            V v_damage = 0;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)1300L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다.");
                return;
            }
            v_damage = ((V)(p.Call("get_vita", v_myid)) * (V)((V)10L));
            v_damage = ((V)(v_damage) + (V)((((V)(p.Call("get_mana", v_myid)) * (V)((V)3L)))));
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)5L))))
            {
                v_damage = ((V)(((V)(v_damage) / (V)((V)8L))) * (V)((V)3L));
            }
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                p.Call("effect", v_target, (V)0L, (V)288L, (V)130L);
                p.Call("damaged", v_target, v_damage);
                p.Call("set_vital", (V)"1");
                p.Call("set_manal", (V)"0");
            }
            else
            {
                if (V.T(V.B(V.T(((V)(v_type) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_damage = p.Call("get_vita", v_myid);
                    p.Call("effect", v_target, (V)0L, (V)288L, (V)130L);
                    p.Call("char_damaged2", v_target, v_damage, v_damage);
                    p.Call("set_vital", (V)"1");
                    p.Call("set_manal", (V)"0");
                }
            }
            p.Call("user_say", (V)0L, (V)"대상은~!?");
            p.Call("game_sound", (V)98L, (V)0L);
            p.Call("motion", (V)136L, (V)75L);
        }
    }
}
