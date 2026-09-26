using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 봉인 — 5.99 `공통스킬.txt` 의 SPELL_봉인 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("봉인", "5.99표")]
    public class SpellBD09C778 : SpellScript
    {
        public SpellBD09C778(Spell spell) : base(spell)
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
            V v_rnd = 0;
            V v_target = 0;
            V v_type = 0;
            V v_x1 = 0;
            V v_x2 = 0;
            V v_y1 = 0;
            V v_y2 = 0;

            v_myid = p.Call("get_myid");
            v_x1 = ((V)(p.Call("get_xs", v_myid)) - (V)((V)8L));
            v_x2 = ((V)(p.Call("get_xs", v_myid)) + (V)((V)8L));
            v_y1 = ((V)(p.Call("get_ys", v_myid)) - (V)((V)8L));
            v_y2 = ((V)(p.Call("get_ys", v_myid)) + (V)((V)8L));
            v_damage = ((V)(((((V)(p.Call("get_mana", v_myid)) + (V)((V)100L))))) * (V)((V)5L));
            for (v_i = v_x1; V.T(((V)(v_i) <= (V)(v_x2))); v_i = ((V)(v_i) + (V)((V)1L)))
            {
                for (v_j = v_y1; V.T(((V)(v_j) <= (V)(v_y2))); v_j = ((V)(v_j) + (V)((V)1L)))
                {
                    v_mob = p.Call("get_mobxy", v_i, v_j);
                    v_type = p.Call("istype", v_mob);
                    v_rnd = p.Call("rand", (V)1L, (V)10L);
                    if (V.T(((V)(v_rnd) <= (V)((V)10L))))
                    {
                        if (V.T(V.B(V.T(v_mob) && V.T(((V)(v_type) == (V)((V)1L))))))
                        {
                            if (V.T(V.B(!V.T(p.Call("magic_exist", (V)1L, v_mob, (V)0L)))))
                            {
                                p.Call("magic", (V)2L, v_mob, (V)0L, (V)60L, (V)0L, v_myid);
                                p.Call("message", (V)3L, (V)"봉인을 외웠습니다.");
                                p.Call("effect", v_mob, (V)0L, (V)43L, (V)130L);  // 노바 이펙트(5.99: 0, 104)
                                p.Call("game_sound", (V)8L, (V)0L);
                            }
                            else
                            {
                                p.Call("effect", v_target, (V)0L, (V)33L, (V)130L);
                                p.Call("message", (V)3L, (V)"몬스터가 마법을 피했다!");
                            }
                        }
                    }
                }
            }
        }
    }
}
