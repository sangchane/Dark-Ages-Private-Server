using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 나르콜룸 — 5.99 `Wizard.txt` 의 SPELL_나르콜룸 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("나르콜룸", "5.99표")]
    public class SpellB098B974CF5CB8F8 : SpellScript
    {
        public SpellB098B974CF5CB8F8(Spell spell) : base(spell)
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
            V v_mobnar = 0;
            V v_myid = 0;
            V v_rnd = 0;
            V v_type = 0;
            V v_x1 = 0;
            V v_x2 = 0;
            V v_y1 = 0;
            V v_y2 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)10L))))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 10%]");
                return;
            }
            p.Call("set_manal", ((V)(p.Call("get_mana", v_myid)) - (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)10L)))));
            v_x1 = ((V)(p.Call("get_xs", v_myid)) - (V)((V)8L));
            v_x2 = ((V)(p.Call("get_xs", v_myid)) + (V)((V)8L));
            v_y1 = ((V)(p.Call("get_ys", v_myid)) - (V)((V)8L));
            v_y2 = ((V)(p.Call("get_ys", v_myid)) + (V)((V)8L));
            for (v_i = v_x1; V.T(((V)(v_i) <= (V)(v_x2))); v_i = ((V)(v_i) + (V)((V)1L)))
            {
                for (v_j = v_y1; V.T(((V)(v_j) <= (V)(v_y2))); v_j = ((V)(v_j) + (V)((V)1L)))
                {
                    v_mobnar = p.Call("get_mobxy", v_i, v_j);
                    if (V.T(V.B(!V.T(v_mobnar))))
                    {
                        goto L_go;
                    }
                    v_type = p.Call("istype", v_mobnar);
                    if (V.T(V.B(V.T(v_mobnar) && V.T(((V)(v_type) == (V)((V)1L))))))
                    {
                        v_rnd = p.Call("rand", (V)1L, (V)10L);
                        if (V.T(V.B(!V.T(p.Call("magic_exist", (V)2L, v_mobnar, (V)0L)))))
                        {
                            if (V.T(((V)(v_rnd) < (V)((V)7L))))
                            {
                                p.Call("magic", (V)2L, v_mobnar, (V)"나르콜룸", (V)120L, (V)45L, v_myid);
                                p.Call("effect", v_mobnar, (V)0L, (V)20L, (V)0L);
                            }
                            else
                            {
                                p.Call("effect", v_mobnar, (V)0L, (V)33L, (V)130L);
                            }
                        }
                    }
                    L_go: ;
                }
            }
            p.Call("message", (V)3L, (V)"나르콜룸을 외웠습니다.");
            p.Call("game_sound", (V)47L, (V)0L);
        }
    }
}
