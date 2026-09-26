using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 암살격진 — 5.99 `Rogue.txt` 의 SKILL_암살격진 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("암살격진", "5.99표")]
    public class SkillC554C0B4ACA9C9C4 : SkillScript
    {
        public SkillC554C0B4ACA9C9C4(Skill skill) : base(skill)
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
            v_damage = ((V)(((p.Call("get_vita", v_myid)))) * (V)((V)4L));
            if (V.T(p.Call("get_hide", v_myid)))
                v_damage = ((V)(v_damage) * (V)((V)4L));
            p.Call("skill_delay", (V)5L);
            if (V.T(V.B(!V.T(v_target))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("istype", p.Call("get_front_char", v_myid))) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_target = p.Call("get_front_char", v_myid);
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                    p.Call("set_vital", ((V)(((V)(p.Call("get_basevita", v_myid)) / (V)((V)100L))) * (V)((V)2L)));
                    p.Call("motion", (V)135L, (V)20L);
                    p.Call("effect", v_target, (V)0L, (V)82L, (V)75L);  // 노바 이펙트(5.99: 0, 360, 속도 75)
                    p.Call("game_sound", (V)17L, (V)0L);
                }
            }
            else
            {
                p.Call("damaged", v_target, v_damage);
                p.Call("set_vital", ((V)(((V)(p.Call("get_basevita", v_myid)) / (V)((V)100L))) * (V)((V)2L)));
                p.Call("motion", (V)135L, (V)20L);
                p.Call("effect", v_target, (V)0L, (V)82L, (V)75L);  // 노바 이펙트(5.99: 0, 360, 속도 75)
                p.Call("game_sound", (V)17L, (V)0L);
            }
        }
    }
}
