using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 붕신선각 — 공격력 ×4 + 지구력 ×100 · 마나 170 · 원작에 없는 팩 전용 기술
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("붕신선각", "5.99표/무도가")]
    public class MonkBD95C2E0C120AC01 : SkillScript
    {
        public MonkBD95C2E0C120AC01(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (!MonkStrike.Spend(sprite, Skill, 170))
                return;

            MonkStrike.Use(sprite, Skill, 400, 10000, 0x85);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
