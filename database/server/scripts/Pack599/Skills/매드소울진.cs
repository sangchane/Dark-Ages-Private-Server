using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 매드소울진 — 5.99 `Warrior.txt` 의 SKILL_매드소울진 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("매드소울진", "5.99표")]
    public class SkillB9E4B4DCC18CC6B8C9C4 : SkillScript
    {
        public SkillB9E4B4DCC18CC6B8C9C4(Skill skill) : base(skill)
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
            v_damage = ((V)(((V)(p.Call("get_vita", v_myid)) / (V)((V)2L))) * (V)((V)8L));
            p.Call("skill_delay", (V)8L);
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
                    p.Call("effect", v_target, (V)0L, (V)289L, (V)75L);
                    p.Call("game_sound", (V)17L, (V)0L);
                }
            }
            else
            {
                p.Call("effect", v_target, (V)0L, (V)289L, (V)75L);
                p.Call("damaged", v_target, v_damage);
                p.Call("set_vital", ((V)(((V)(p.Call("get_basevita", v_myid)) / (V)((V)100L))) * (V)((V)2L)));
                p.Call("motion", (V)130L, (V)20L);
                p.Call("game_sound", (V)17L, (V)0L);
            }
        }
    }
}
