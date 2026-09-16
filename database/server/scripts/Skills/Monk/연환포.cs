using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 연환포 — 힘 ×24 + 민첩성 ×12  (팩의 공격력 6배를 편 것)
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-monk-skills.py` 가 5.99 서버팩 스크립트에서 다시 만든다.
    /// </remarks>
    [Script("연환포", "5.99표/무도가")]
    public class MonkC5F0D658D3EC : SkillScript
    {
        public MonkC5F0D658D3EC(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.Use(sprite, Skill, 2400, 0, 1200, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
