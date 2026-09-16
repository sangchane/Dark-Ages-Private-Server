using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 연천단각 — 공격력 ×2 + 지구력 ×80 · 마나 130
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("연천단각", "5.99표/무도가")]
    public class MonkC5F0CC9CB2E8AC01 : SkillScript
    {
        public MonkC5F0CC9CB2E8AC01(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (!MonkStrike.Spend(sprite, Skill, 130))
                return;

            MonkStrike.Use(sprite, Skill, 200, 8000, 0x83);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
