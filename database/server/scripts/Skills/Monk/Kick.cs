using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    [Script("Kick", "LOD")]
    public class Kick : SkillScript
    {
        public Kick(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            // 단각 — 5.99: 공격력 ÷10 ×28 = 공격력 ×2.8
            MonkStrike.Use(sprite, Skill, 280, 0, 0x83);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
