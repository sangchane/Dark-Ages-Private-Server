using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 암살 — 5.99 `도적(비전직).txt` 의 SPELL_암살 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("암살", "5.99표")]
    public class SpellC554C0B4 : SpellScript
    {
        public SpellC554C0B4(Spell spell) : base(spell)
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
            V v_side = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)200L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 200이상]");
                return;
            }
            if (V.T(p.Call("get_map_pk", v_myid)))
            {
                p.Call("message", (V)3L, (V)"PK맵에서는 사용할수 없는기술 입니다.");
                return;
            }
            p.Call("manal_del", (V)"200");
            v_damage = (((V)(p.Call("get_att_damage", v_myid)) * (V)((V)2L)));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_damage = ((V)(v_damage) * (V)((V)2L));
            }
            if (V.T(V.B(V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)1L))))) && V.T(p.Call("get_mobdie")))))
            {
                return;
            }
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                v_side = p.Call("get_mobside", v_target);
                if (V.T(((V)(v_side) == (V)((V)0L))))
                {
                    if (V.T(((V)(p.Call("get_xy_block", v_myid, p.Call("get_mobxs", v_target), ((V)(p.Call("get_mobys", v_target)) + (V)((V)1L)))) == (V)((V)1L))))
                    {
                        p.Call("message", (V)3L, (V)"몬스터등이 벽을 향해 사용이 불가능합니다.");
                        return;
                    }
                    p.Call("set_side", (V)0L);
                    p.Call("set_xs", p.Call("get_mobxs", v_target));
                    p.Call("set_ys", ((V)(p.Call("get_mobys", v_target)) + (V)((V)1L)));
                }
                if (V.T(((V)(v_side) == (V)((V)1L))))
                {
                    if (V.T(((V)(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_mobxs", v_target)) - (V)((V)1L)), p.Call("get_mobys", v_target))) == (V)((V)1L))))
                    {
                        p.Call("message", (V)3L, (V)"몬스터등이 벽을 향해 사용이 불가능합니다.");
                        return;
                    }
                    p.Call("set_side", (V)1L);
                    p.Call("set_xs", ((V)(p.Call("get_mobxs", v_target)) - (V)((V)1L)));
                    p.Call("set_ys", p.Call("get_mobys", v_target));
                }
                if (V.T(((V)(v_side) == (V)((V)2L))))
                {
                    if (V.T(((V)(p.Call("get_xy_block", v_myid, p.Call("get_mobxs", v_target), ((V)(p.Call("get_mobys", v_target)) - (V)((V)1L)))) == (V)((V)1L))))
                    {
                        p.Call("message", (V)3L, (V)"몬스터등이 벽을 향해 사용이 불가능합니다.");
                        return;
                    }
                    p.Call("set_side", (V)2L);
                    p.Call("set_xs", p.Call("get_mobxs", v_target));
                    p.Call("set_ys", ((V)(p.Call("get_mobys", v_target)) - (V)((V)1L)));
                }
                if (V.T(((V)(v_side) == (V)((V)3L))))
                {
                    if (V.T(((V)(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_mobxs", v_target)) + (V)((V)1L)), p.Call("get_mobys", v_target))) == (V)((V)1L))))
                    {
                        p.Call("message", (V)3L, (V)"몬스터등이 벽을 향해 사용이 불가능합니다.");
                        return;
                    }
                    p.Call("set_side", (V)3L);
                    p.Call("set_xs", ((V)(p.Call("get_mobxs", v_target)) + (V)((V)1L)));
                    p.Call("set_ys", p.Call("get_mobys", v_target));
                }
                p.Call("hide", v_myid, (V)10L);
                p.Call("effect", v_target, (V)0L, (V)266L, (V)100L);
                p.Call("damaged", v_target, v_damage);
            }
            else
                if (V.T(V.B(V.T(((V)(v_type) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_side = p.Call("get_side", v_target);
                    if (V.T(((V)(v_side) == (V)((V)0L))))
                    {
                        if (V.T(((V)(p.Call("get_xy_block", v_myid, p.Call("get_xs1", v_target), ((V)(p.Call("get_ys1", v_target)) + (V)((V)1L)))) == (V)((V)1L))))
                        {
                            p.Call("message", (V)3L, (V)"몬스터등이 벽을 향해 사용이 불가능합니다.");
                            return;
                        }
                        p.Call("set_side", (V)0L);
                        p.Call("set_xs", p.Call("get_xs1", v_target));
                        p.Call("set_ys", ((V)(p.Call("get_ys1", v_target)) + (V)((V)1L)));
                    }
                    if (V.T(((V)(v_side) == (V)((V)1L))))
                    {
                        if (V.T(((V)(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs1", v_target)) - (V)((V)1L)), p.Call("get_ys1", v_target))) == (V)((V)1L))))
                        {
                            p.Call("message", (V)3L, (V)"몬스터등이 벽을 향해 사용이 불가능합니다.");
                            return;
                        }
                        p.Call("set_side", (V)1L);
                        p.Call("set_xs", ((V)(p.Call("get_xs1", v_target)) - (V)((V)1L)));
                        p.Call("set_ys", p.Call("get_ys1", v_target));
                    }
                    if (V.T(((V)(v_side) == (V)((V)2L))))
                    {
                        if (V.T(((V)(p.Call("get_xy_block", v_myid, p.Call("get_xs1", v_target), ((V)(p.Call("get_ys1", v_target)) - (V)((V)1L)))) == (V)((V)1L))))
                        {
                            p.Call("message", (V)3L, (V)"몬스터등이 벽을 향해 사용이 불가능합니다.");
                            return;
                        }
                        p.Call("set_side", (V)2L);
                        p.Call("set_xs", p.Call("get_xs1", v_target));
                        p.Call("set_ys", ((V)(p.Call("get_ys1", v_target)) - (V)((V)1L)));
                    }
                    if (V.T(((V)(v_side) == (V)((V)3L))))
                    {
                        if (V.T(((V)(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs1", v_target)) + (V)((V)1L)), p.Call("get_ys1", v_target))) == (V)((V)1L))))
                        {
                            p.Call("message", (V)3L, (V)"몬스터등이 벽을 향해 사용이 불가능합니다.");
                            return;
                        }
                        p.Call("set_side", (V)3L);
                        p.Call("set_xs", ((V)(p.Call("get_xs1", v_target)) + (V)((V)1L)));
                        p.Call("set_ys", p.Call("get_ys1", v_target));
                    }
                    p.Call("hide", v_myid, (V)10L);
                    p.Call("effect", v_target, (V)0L, (V)266L, (V)100L);
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                }
        }
    }
}
