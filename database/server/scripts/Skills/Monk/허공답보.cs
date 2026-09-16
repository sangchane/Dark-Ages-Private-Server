using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 허공답보 — 힘 ×200 + 지구력 ×100 + 민첩성 ×100  (팩의 공격력 50배를 편 것)
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
            MonkStrike.Use(sprite, Skill, 20000, 10000, 10000, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
