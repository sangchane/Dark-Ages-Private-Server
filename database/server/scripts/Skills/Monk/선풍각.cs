using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 선풍각 — 힘 ×14 + 지구력 ×66 + 민첩성 ×7  (팩의 공격력 3.5배를 편 것)
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("선풍각", "5.99표/무도가")]
    public class MonkC120D48DAC01 : SkillScript
    {
        public MonkC120D48DAC01(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.Use(sprite, Skill, 1400, 6600, 700, 0x85);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
