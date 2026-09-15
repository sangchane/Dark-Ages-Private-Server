using System.Linq;
using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    [Script("Poison Punch", "LOD")]
    public class PoisonPunch : SkillScript
    {
        public PoisonPunch(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            MonkStrike.Use(sprite, Skill, 4, 2, 0, 0x84, target =>
            {
                if (target.Debuffs.Values.OfType<Debuff_poison>().Any())
                    return;

                var poison = new Debuff_poison("Poison Punch", 15, 35, 25, 0.05);
                poison.OnApplied(target, poison);
            });
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
