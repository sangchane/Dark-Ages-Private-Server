using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 붕각 — 힘 ×14 + 지구력 ×59 + 민첩성 ×7  (팩의 공격력 3.5배를 편 것)
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("붕각", "5.99표/무도가")]
    public class MonkBD95AC01 : SkillScript
    {
        public MonkBD95AC01(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.Use(sprite, Skill, 1400, 5900, 700, 0x85);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
