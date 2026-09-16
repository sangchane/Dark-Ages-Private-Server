using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 늑대의위상 — (최대 체력 + 1) × 3 또는 × 5 반반
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("늑대의위상", "5.99표/무도가")]
    public class MonkB291B300C758C704C0C1 : SkillScript
    {
        public MonkB291B300C758C704C0C1(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.UseWolf(sprite, Skill, 3, 5, 0x87);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
