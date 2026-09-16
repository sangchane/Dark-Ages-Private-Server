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
            // Sting — 5.99 정권의 공격력 ×2.5
            MonkStrike.Use(sprite, Skill, 250, 0, 0x84);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
