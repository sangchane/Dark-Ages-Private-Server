using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 소수신공 — 120초 동안 공격력 +40% · 마나 120
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("소수신공", "5.99표/무도가")]
    public class MonkC18CC218C2E0ACF5 : SkillScript
    {
        public MonkC18CC218C2E0ACF5(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (!MonkStrike.Spend(sprite, Skill, 120))
                return;

            MonkStrike.Empower(sprite, Skill, 120, 0x84, "소수신공을 외웠습니다.");
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
