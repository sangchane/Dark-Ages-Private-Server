using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 일음지 — 앞의 적을 10초 실명 · 마나 80
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("일음지", "5.99표/무도가")]
    public class MonkC77CC74CC9C0 : SkillScript
    {
        public MonkC77CC74CC9C0(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (!MonkStrike.Spend(sprite, Skill, 80))
                return;

            MonkStrike.Afflict(sprite, Skill, new debuff_blind(), 10, false, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
