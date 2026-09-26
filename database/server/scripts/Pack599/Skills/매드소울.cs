using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 매드소울 — 5.99 `전사(비전직).txt` 의 SKILL_매드소울 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("매드소울", "5.99표")]
    public class SkillB9E4B4DCC18CC6B8 : SkillScript
    {
        public SkillB9E4B4DCC18CC6B8(Skill skill) : base(skill)
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
            V v_target = 0;

            v_myid = p.Call("get_myid");
            v_target = p.Call("skill_target");
            v_damage = ((V)(((V)(p.Call("get_vita", v_myid)) / (V)((V)2L))) * (V)((V)7L));
            if (V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)1L))))
            {
                v_damage = ((V)(((V)(v_damage) / (V)((V)4L))) * (V)((V)4L));
            }
            p.Call("skill_delay", (V)15L);
            if (V.T(p.Call("focus", v_myid, (V)0L)))
            {
                v_damage = ((V)(v_damage) * (V)((V)2L));
            }
            if (V.T(V.B(!V.T(v_target))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("istype", p.Call("get_front_char", v_myid))) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_damage = p.Call("get_vita", v_myid);
                    v_target = p.Call("get_front_char", v_myid);
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                    p.Call("set_vital", ((V)(((V)(p.Call("get_basevita", v_myid)) / (V)((V)100L))) * (V)((V)2L)));
                    p.Call("motion", (V)130L, (V)20L);
                    p.Call("effect", v_target, (V)0L, (V)53L, (V)75L);  // 노바 이펙트(5.99: 0, 289, 속도 75)
                    p.Call("game_sound", (V)17L, (V)0L);
                }
            }
            else
            {
                p.Call("effect", v_target, (V)0L, (V)53L, (V)75L);  // 노바 이펙트(5.99: 0, 289, 속도 75)
                p.Call("damaged", v_target, v_damage);
                p.Call("set_vital", ((V)(((V)(p.Call("get_basevita", v_myid)) / (V)((V)100L))) * (V)((V)2L)));
                p.Call("motion", (V)130L, (V)20L);
                p.Call("game_sound", (V)17L, (V)0L);
            }
        }
    }
}
