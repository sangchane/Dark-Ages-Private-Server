using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 백보신권 — 앞 3칸에 공격력 ×3.5 · 마나 120
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("백보신권", "5.99표/무도가")]
    public class MonkBC31BCF4C2E0AD8C : SkillScript
    {
        public MonkBC31BCF4C2E0AD8C(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (!MonkStrike.Spend(sprite, Skill, 120))
                return;

            MonkStrike.Use(sprite, Skill, 350, 0, 0x84, reach: 3);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
