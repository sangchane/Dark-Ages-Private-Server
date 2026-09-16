using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 무영신공 — 앞 3칸에 공격력 ×15 + 지구력 ×130 · 마나 320
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("무영신공", "5.99표/무도가")]
    public class MonkBB34C601C2E0ACF5 : SkillScript
    {
        public MonkBB34C601C2E0ACF5(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (!MonkStrike.Spend(sprite, Skill, 320))
                return;

            MonkStrike.Use(sprite, Skill, 1500, 13000, 0x84, reach: 3);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
