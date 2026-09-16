using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// M포효 — 5.99 `Warrior.txt` 의 SKILL_M포효 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("M포효", "5.99표")]
    public class Skill004DD3ECD6A8 : SkillScript
    {
        public Skill004DD3ECD6A8(Skill skill) : base(skill)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)3L))))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마력 : 3%]");
                return;
            }
            p.Call("manal_del", ((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)3L)));
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
                        goto L_go;
                    }
                    v_type = p.Call("istype", v_mob);
                    if (V.T(V.B(V.T(v_mob) && V.T(((V)(v_type) == (V)((V)1L))))))
                    {
                        p.Call("magic", (V)6L, v_mob, (V)0L, (V)13L, (V)0L, v_myid);
                        p.Call("effect", v_mob, (V)0L, (V)41L, (V)75L);
                    }
                    L_go: ;
                }
            }
            p.Call("message", (V)3L, (V)"M포효를 시전하셧습니다.");
            p.Call("effect", v_myid, (V)73L, (V)0L, (V)100L);
            p.Call("game_sound", (V)48L, (V)0L);
            p.Call("skill_delay", (V)22L);
        }
    }
}
