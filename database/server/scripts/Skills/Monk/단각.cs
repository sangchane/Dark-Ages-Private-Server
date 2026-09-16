using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 단각 — 힘 ×11.2 + 민첩성 ×5.6  (팩의 공격력 2.8배를 편 것)
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("단각", "5.99표/무도가")]
    public class MonkB2E8AC01 : SkillScript
    {
        public MonkB2E8AC01(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.Use(sprite, Skill, 1120, 0, 560, 0x83);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
