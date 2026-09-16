using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    [Script("Sting", "LOD")]
    public class Sting : SkillScript
    {
        public Sting(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            // Sting — 5.99 정권의 2.5배. 조건 없는 첫 주먹이라 가장 낮다.
            // 평타 2.5배(정권)를 편 것: 힘 ×10 + 민첩성 ×5
            MonkStrike.Use(sprite, Skill, 1000, 0, 500, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
