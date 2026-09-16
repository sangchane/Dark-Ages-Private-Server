using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 딜루메니 — 5.99 `법사(비전직).txt` 의 SPELL_딜루메니 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("딜루메니", "5.99표")]
    public class SpellB51CB8E8BA54B2C8 : SpellScript
    {
        public SpellB51CB8E8BA54B2C8(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)100L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 100]");
                return;
            }
            p.Call("manal_del", (V)"100");
            if (V.T(((V)(p.Call("rand", (V)1L, (V)10L)) < (V)((V)3L))))
            {
                p.Call("message", (V)3L, (V)"실패하셧습니다.");
                return;
            }
            if (V.T(V.B(V.T(p.Call("get_map_pk")) && V.T(((V)(p.Call("istype", v_target)) == (V)((V)3L))))))
            {
                return;
                if (V.T(p.Call("set_strabismus", v_target, (V)12L)))
                {
                    return;
                }
                else
                {
                    p.Call("game_sound", (V)8L, (V)0L);
                    if (V.T(((V)(v_target) != (V)(v_myid))))
                    {
                        p.Call("effect", v_target, (V)0L, (V)276L, (V)80L);
                        p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 딜루메니를 가합니다.")));
                    }
                    else
                    {
                        p.Call("effect", v_target, (V)276L, (V)0L, (V)80L);
                    }
                    p.Call("message", (V)3L, (V)"딜루메니를 시전하셧습니다.");
                }
            }
            else
                if (V.T(((V)(p.Call("istype", v_target)) == (V)((V)1L))))
                {
                    if (V.T(V.B(!V.T(p.Call("mob_strabismus", v_target, (V)8L)))))
                    {
                        return;
                    }
                    else
                    {
                        p.Call("game_sound", (V)8L, (V)0L);
                        p.Call("effect", v_target, (V)0L, (V)276L, (V)80L);
                    }
                }
        }
    }
}
