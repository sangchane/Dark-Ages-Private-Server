using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 메테오 — 5.99 `Wizard.txt` 의 SPELL_메테오 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("메테오", "5.99표")]
    public class SpellBA54D14CC624 : SpellScript
    {
        public SpellBA54D14CC624(Spell spell) : base(spell)
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
            V v_i = 0;
            V v_j = 0;
            V v_mob = 0;
            V v_myid = 0;
            V v_type = 0;
            V v_x1 = 0;
            V v_x2 = 0;
            V v_y1 = 0;
            V v_y2 = 0;
            bool f_go = false;

            v_myid = p.Call("get_myid");
            if (V.T(p.Call("get_solo", v_myid)))
            {
                p.Call("message", (V)3L, (V)"사용불가 지역입니다.");
                return;
            }
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)20000L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 20000이상]");
                return;
            }
            p.Call("message", (V)3L, (V)"메테오를 외웠습니다.");
            p.Call("game_sound", (V)63L, (V)0L);
            v_damage = ((V)(((V)(p.Call("get_mana", v_myid)) / (V)((V)4L))) * (V)((V)10L));
            v_x1 = ((V)(p.Call("get_xs", v_myid)) - (V)((V)7L));
            v_x2 = ((V)(p.Call("get_xs", v_myid)) + (V)((V)7L));
            v_y1 = ((V)(p.Call("get_ys", v_myid)) - (V)((V)7L));
            v_y2 = ((V)(p.Call("get_ys", v_myid)) + (V)((V)7L));
            for (v_i = v_x1; V.T(((V)(v_i) <= (V)(v_x2))); v_i = ((V)(v_i) + (V)((V)1L)))
            {
                for (v_j = v_y1; V.T(((V)(v_j) <= (V)(v_y2))); v_j = ((V)(v_j) + (V)((V)1L)))
                {
                    v_mob = p.Call("get_mobxy", v_i, v_j);
                    if (V.T(V.B(!V.T(v_mob))))
                    {
                        { f_go = true; goto L_go_enter; }
                    }
                    v_type = p.Call("istype", v_mob);
                    L_go_enter: ;
                    if (V.T(V.B(!f_go && V.T(V.B(V.T(v_mob) && V.T(((V)(v_type) == (V)((V)1L))))))))
                    {
                        p.Call("effect", v_mob, (V)0L, (V)72L, (V)100L);
                        p.Call("effect", v_mob, (V)0L, (V)72L, (V)100L);
                        p.Call("damaged", v_mob, v_damage);
                    }
                    else
                    {
                        f_go = false;
                        if (V.T(V.B(V.T(p.Call("get_map_pk")) && V.T(((V)(p.Call("istype", p.Call("get_char_serial", v_myid, v_i, v_j))) == (V)((V)3L))))))
                        {
                            v_damage = p.Call("get_mana", v_myid);
                            v_mob = p.Call("get_char_serial", v_myid, v_i, v_j);
                            if (V.T(((V)(v_myid) != (V)(v_mob))))
                            {
                                p.Call("effect", v_mob, (V)0L, (V)72L, (V)100L);
                                p.Call("char_damaged2", v_mob, v_damage, v_damage);
                            }
                        }
                    }
                }
            }
            p.Call("set_manal", (V)0L);
        }
    }
}
