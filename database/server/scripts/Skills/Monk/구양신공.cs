using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 구양신공 — 현재 체력 ×10 로 사방 네 칸, 내 체력은 최대의 2% 로
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("구양신공", "5.99표/무도가")]
    public class MonkAD6CC591C2E0ACF5 : SkillScript
    {
        public MonkAD6CC591C2E0ACF5(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.UseCross(sprite, Skill, 10, 2, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
