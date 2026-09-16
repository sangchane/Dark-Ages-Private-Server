using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 전체크래셔 — 5.99 `Warrior.txt` 의 SKILL_전체크래셔 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("전체크래셔", "5.99표")]
    public class SkillC804CCB4D06CB798C154 : SkillScript
    {
        public SkillC804CCB4D06CB798C154(Skill skill) : base(skill)
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
            V v_i = 0;
            V v_j = 0;
            V v_mob = 0;
            V v_myid = 0;
            V v_type = 0;
            V v_x1 = 0;
            V v_x2 = 0;
            V v_y1 = 0;
            V v_y2 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)7000L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 7000이상]");
                return;
            }
            if (V.T(((V)(p.Call("get_vita", v_myid)) < (V)(((V)(((V)(p.Call("get_basevita", v_myid)) / (V)((V)100L))) * (V)((V)10L))))))
            {
                p.Call("message", (V)3L, (V)"체력이 부족합니다. [필요체력 : 10%]");
                return;
            }
            p.Call("manal_del", (V)"7000");
            p.Call("game_sound", (V)44L, (V)0L);
            p.Call("skill_delay", (V)2L);
            v_damage = ((V)(((p.Call("get_vita", v_myid)))) / (V)((V)6L));
            p.Call("set_vital", ((V)(p.Call("get_vita", v_myid)) - (V)(((V)(((V)(p.Call("get_basevita", v_myid)) / (V)((V)100L))) * (V)((V)10L)))));
            v_x1 = ((V)(p.Call("get_xs", v_myid)) - (V)((V)6L));
            v_x2 = ((V)(p.Call("get_xs", v_myid)) + (V)((V)6L));
            v_y1 = ((V)(p.Call("get_ys", v_myid)) - (V)((V)6L));
            v_y2 = ((V)(p.Call("get_ys", v_myid)) + (V)((V)6L));
            for (v_i = v_x1; V.T(((V)(v_i) <= (V)(v_x2))); v_i = ((V)(v_i) + (V)((V)1L)))
            {
                for (v_j = v_y1; V.T(((V)(v_j) <= (V)(v_y2))); v_j = ((V)(v_j) + (V)((V)1L)))
                {
                    v_mob = p.Call("get_mobxy", v_i, v_j);
                    v_type = p.Call("istype", v_mob);
                    if (V.T(V.B(V.T(v_mob) && V.T(((V)(v_type) == (V)((V)1L))))))
                    {
                        p.Call("damaged", v_mob, v_damage);
                        p.Call("effect", v_mob, (V)0L, (V)303L, (V)80L);
                    }
                }
            }
        }
    }
}
