using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 마구때리기 — ((힘 + 10) + (지구력 + 10)) × 5000
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("마구때리기", "5.99표/무도가")]
    public class MonkB9C8AD6CB54CB9ACAE30 : SkillScript
    {
        public MonkB9C8AD6CB54CB9ACAE30(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.UseStrengthAndEndurance(sprite, Skill, 10, 10, 5000, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
