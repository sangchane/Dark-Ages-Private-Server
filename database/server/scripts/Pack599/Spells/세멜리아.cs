using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 세멜리아 — 5.99 `법사(비전직).txt` 의 SPELL_세멜리아 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("세멜리아", "5.99표")]
    public class SpellC138BA5CB9ACC544 : SpellScript
    {
        public SpellC138BA5CB9ACC544(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)100L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 100이상]");
                return;
            }
            v_damage = ((V)(p.Call("get_mana", v_myid)) * (V)((V)3L));
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                p.Call("effect", v_target, (V)0L, (V)52L, (V)100L);
                p.Call("damaged", v_target, v_damage);
                p.Call("set_manal", (V)0L);
            }
            else
            {
                if (V.T(V.B(V.T(((V)(v_type) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_damage = p.Call("get_mana", v_myid);
                    p.Call("char_damaged2", v_target, v_damage, v_damage);
                    p.Call("set_manal", (V)0L);
                }
            }
            p.Call("message", (V)3L, (V)"세멜리아를 외웠습니다.");
            p.Call("effect", v_target, (V)0L, (V)52L, (V)100L);
            p.Call("motion", (V)136L, (V)75L);
            p.Call("game_sound", (V)63L, (V)0L);
        }
    }
}
