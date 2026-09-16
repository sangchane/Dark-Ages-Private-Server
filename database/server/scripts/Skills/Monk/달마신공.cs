using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 달마신공 — 최대 체력의 30%. 5.99 와 Novaonline 이 글자까지 같다.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("달마신공", "5.99표/무도가")]
    public class MonkB2ECB9C8C2E0ACF5 : SkillScript
    {
        public MonkB2ECB9C8C2E0ACF5(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.UseVitality(sprite, Skill, 30, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
