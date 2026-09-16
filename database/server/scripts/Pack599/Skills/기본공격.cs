using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 기본공격 — 5.99 `공통스킬.txt` 의 SKILL_기본공격 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("기본공격", "5.99표")]
    public class SkillAE30BCF8ACF5ACA9 : SkillScript
    {
        public SkillAE30BCF8ACF5ACA9(Skill skill) : base(skill)
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
            V v_target = 0;

            p.Call("attack_skill");
            p.Call("effect", v_target, (V)0L, (V)0L, (V)75L);
        }
    }
}
