using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 파천각 — 둘레 네 칸에 공격력 ×5 + 지구력 ×110 · 마나 185
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("파천각", "5.99표/무도가")]
    public class MonkD30CCC9CAC01 : SkillScript
    {
        public MonkD30CCC9CAC01(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (!MonkStrike.Spend(sprite, Skill, 185))
                return;

            MonkStrike.Use(sprite, Skill, 500, 11000, 0x85, around: true);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
