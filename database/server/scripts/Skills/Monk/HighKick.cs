using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    [Script("High Kick", "LOD")]
    public class HighKick : SkillScript
    {
        public HighKick(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            // High Kick — 5.99 의 백보신권과 같은 3.5배 자리. 한글 이름은 아직 미확정.
            MonkStrike.Use(sprite, Skill, 350, 0, 0x85);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
