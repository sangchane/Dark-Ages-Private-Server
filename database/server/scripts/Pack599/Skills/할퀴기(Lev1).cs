using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 할퀴기(Lev1) — 5.99 `Monk.txt` 의 SKILL_할퀴기(Lev1) 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("할퀴기(Lev1)", "5.99표")]
    public class SkillD560D034AE300028004C0065007600310029 : SkillScript
    {
        public SkillD560D034AE300028004C0065007600310029(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
        }

        public override void OnUse(Sprite sprite)
        {
            if (!Skill.Ready)
                return;

            var p = new Pack599(sprite, null);
            if (!p.Ready)
                return;

            p.Train(Skill);
            V v_damage = 0;
            V v_myid = 0;
            V v_rnd = 0;
            V v_target = 0;

            v_myid = p.Call("get_myid");
            v_target = p.Call("skill_target");
            L_re: ;
            v_rnd = p.Call("rand", (V)1L, (V)10L);
            if (V.T(V.B(V.T(((V)(v_rnd) < (V)((V)1L))) || V.T(((V)(v_rnd) > (V)((V)10L))))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_rnd) <= (V)((V)5L))))
            {
                v_damage = ((V)(((((V)(p.Call("get_basevita", v_myid)) + (V)((V)1L))))) * (V)((V)6L));
                p.Call("skill_delay", (V)6L);
                p.Call("damaged", v_target, v_damage);
                p.Call("motion", (V)135L, (V)20L);
                p.Call("effect", v_target, (V)0L, (V)70L, (V)100L);  // 노바 이펙트(5.99: 0, 194)
                p.Call("message", (V)3L, (V)"할퀴기을 시전했습니다.");
                p.Call("game_sound", (V)7L, (V)0L);
                return;
            }
            else
            {
                v_damage = ((V)(((((V)(p.Call("get_basevita", v_myid)) + (V)((V)1L))))) * (V)((V)8L));
                p.Call("skill_delay", (V)3L);
                p.Call("damaged", v_target, v_damage);
                p.Call("motion", (V)135L, (V)20L);
                p.Call("effect", v_target, (V)0L, (V)70L, (V)100L);  // 노바 이펙트(5.99: 0, 194)
                p.Call("game_sound", (V)7L, (V)0L);
                p.Call("message", (V)3L, (V)"할퀴기(크리)을 시전했습니다.");
                return;
            }
        }
    }
}
