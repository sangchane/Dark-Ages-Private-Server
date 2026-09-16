using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 허공답보 — 앞의 적을 넘어 2칸 건너뛰고 돌아서서 공격력 ×50 + 최대 체력 + 지구력 ×100
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("허공답보", "5.99표/무도가")]
    public class MonkD5C8ACF5B2F5BCF4 : SkillScript
    {
        public MonkD5C8ACF5B2F5BCF4(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.Step(sprite, Skill, 2, 5000, 10000, 100);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
