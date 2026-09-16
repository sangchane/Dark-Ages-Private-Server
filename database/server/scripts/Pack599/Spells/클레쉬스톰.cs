using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 클레쉬스톰 — 5.99 `Wizard.txt` 의 SPELL_클레쉬스톰 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("클레쉬스톰", "5.99표")]
    public class SpellD074B808C26CC2A4D1B0 : SpellScript
    {
        public SpellD074B808C26CC2A4D1B0(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)620L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 620이상]");
                return;
            }
            p.Call("manal_del", (V)"620");
            v_damage = ((V)(p.Call("get_mgc_damage", v_myid)) * (V)((V)4L));
            v_damage = ((V)(((V)(v_damage) / (V)((V)2L))) * (V)((V)3L));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_damage = ((V)(v_damage) * (V)((V)2L));
            }
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(V.B(V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)1L))))) && V.T(p.Call("get_mobdie")))))
            {
                return;
            }
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                p.Call("effect", v_target, (V)0L, (V)97L, (V)75L);
                p.Call("damaged", v_target, v_damage);
            }
            else
            {
                if (V.T(V.B(V.T(((V)(v_type) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    p.Call("char_damaged2", v_target, v_damage, v_damage);
                    if (V.T(((V)(v_target) != (V)(v_myid))))
                    {
                        p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 클레쉬스톰을 가합니다.")));
                    }
                }
            }
            p.Call("effect", v_target, (V)0L, (V)97L, (V)75L);
            p.Call("message", (V)3L, (V)"클레쉬스톰을 외웠습니다.");
            p.Call("motion", (V)136L, (V)75L);
            p.Call("game_sound", (V)78L, (V)0L);
        }
    }
}
