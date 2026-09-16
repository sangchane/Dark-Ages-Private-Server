using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 발경 — 앞의 적을 15초 빙결
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("발경", "5.99표/무도가")]
    public class MonkBC1CACBD : SkillScript
    {
        public MonkBC1CACBD(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.Afflict(sprite, Skill, new debuff_frozen(), 15, true, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
