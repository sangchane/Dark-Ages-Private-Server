using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 데빌크래셔 — 5.99 `Warrior.txt` 의 SKILL_데빌크래셔 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("데빌크래셔", "5.99표")]
    public class SkillB370BE4CD06CB798C154 : SkillScript
    {
        public SkillB370BE4CD06CB798C154(Skill skill) : base(skill)
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
            if (V.T(((V)(p.Call("get_vita", v_myid)) > (V)(((V)(((V)(p.Call("get_basevita", v_myid)) / (V)((V)100L))) * (V)((V)2L))))))
            {
                p.Call("message", (V)3L, (V)"체력이 너무많습니다.");
                return;
            }
            p.Call("skill_delay", (V)8L);
            if (V.T(((V)(p.Call("rand", (V)1L, (V)10L)) >= (V)((V)8L))))
            {
                p.Call("message", (V)3L, (V)"실패했습니다.");
                return;
            }
            v_target = p.Call("skill_target");
            v_damage = ((V)(p.Call("get_basevita", v_myid)) * (V)((V)10L));
            if (V.T(p.Call("focus", v_myid, (V)0L)))
            {
                v_damage = ((V)(v_damage) * (V)((V)3L));
            }
            if (V.T(V.B(!V.T(v_target))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("istype", p.Call("get_front_char", v_myid))) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_target = p.Call("get_front_char", v_myid);
                    p.Call("effect", v_target, (V)50L, (V)50L, (V)130L);  // 노바 이펙트(5.99: 303, 303)
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                    p.Call("motion", (V)130L, (V)60L);
                    p.Call("game_sound", (V)44L, (V)0L);
                    return;
                }
                else
                {
                    return;
                }
            }
            p.Call("motion", (V)130L, (V)60L);
            p.Call("game_sound", (V)44L, (V)0L);
            p.Call("effect", v_target, (V)50L, (V)50L, (V)130L);  // 노바 이펙트(5.99: 303, 303)
            p.Call("damaged", v_target, v_damage);
            return;
        }
    }
}
