using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 프라베라 — 5.99 `Wizard.txt` 의 SPELL_프라베라 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("프라베라", "5.99표")]
    public class SpellD504B77CBCA0B77C : SpellScript
    {
        public SpellD504B77CBCA0B77C(Spell spell) : base(spell)
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
            V v_i = 0;
            V v_j = 0;
            V v_mob = 0;
            V v_myid = 0;
            V v_rnd = 0;
            V v_type = 0;
            V v_x1 = 0;
            V v_x2 = 0;
            V v_y1 = 0;
            V v_y2 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)3000L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 3000 이상]");
                return;
            }
            p.Call("manal_del", (V)"3000");
            v_x1 = ((V)(p.Call("get_xs", v_myid)) - (V)((V)8L));
            v_x2 = ((V)(p.Call("get_xs", v_myid)) + (V)((V)8L));
            v_y1 = ((V)(p.Call("get_ys", v_myid)) - (V)((V)8L));
            v_y2 = ((V)(p.Call("get_ys", v_myid)) + (V)((V)8L));
            for (v_i = v_x1; V.T(((V)(v_i) <= (V)(v_x2))); v_i = ((V)(v_i) + (V)((V)1L)))
            {
                for (v_j = v_y1; V.T(((V)(v_j) <= (V)(v_y2))); v_j = ((V)(v_j) + (V)((V)1L)))
                {
                    v_mob = p.Call("get_mobxy", v_i, v_j);
                    if (V.T(V.B(!V.T(v_mob))))
                    {
                        goto L_go;
                    }
                    v_type = p.Call("istype", v_mob);
                    if (V.T(V.B(V.T(v_mob) && V.T(((V)(v_type) == (V)((V)1L))))))
                    {
                        v_rnd = p.Call("rand", (V)1L, (V)10L);
                        if (V.T(V.B(!V.T(p.Call("magic_exist", (V)1L, v_mob, (V)0L)))))
                        {
                            if (V.T(((V)(v_rnd) < (V)((V)6L))))
                            {
                                p.Call("magic", (V)1L, v_mob, (V)"프라베라", (V)120L, (V)45L, v_myid);
                                p.Call("effect", v_mob, (V)0L, (V)43L, (V)100L);  // 노바 이펙트(5.99: 0, 257)
                            }
                            else
                            {
                                p.Call("effect", v_mob, (V)0L, (V)33L, (V)130L);
                            }
                        }
                    }
                    L_go: ;
                }
            }
            p.Call("message", (V)3L, (V)"프라베라를 외웠습니다.");
            p.Call("game_sound", (V)27L, (V)0L);
        }
    }
}
