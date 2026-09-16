using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 적무기해체 — 5.99 `전사(비전직).txt` 의 SKILL_적무기해체 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("적무기해체", "5.99표")]
    public class SkillC801BB34AE30D574CCB4 : SkillScript
    {
        public SkillC801BB34AE30D574CCB4(Skill skill) : base(skill)
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
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            p.Call("skill_delay", (V)13L);
            v_myid = p.Call("get_myid");
            if (V.T(V.B(!V.T(p.Call("get_map_pk")))))
            {
                p.Call("message", (V)3L, (V)"PK가 가능한 장소에서만 시전가능합니다.");
                return;
            }
            v_target = p.Call("get_front_char", v_myid);
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) == (V)((V)3L))))
            {
                p.Call("message", (V)3L, (V)"적무기해체를 시전하셧습니다.");
                p.Call("weapon_del", v_target);
                p.Call("motion", (V)129L, (V)20L);
            }
        }
    }
}
