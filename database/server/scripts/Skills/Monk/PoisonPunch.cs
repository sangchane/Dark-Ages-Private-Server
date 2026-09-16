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
            // Poison Punch — 주먹 배수는 정권과 같고, 독은 아래 onHit 가 건다.
            // 평타 2.5배를 편 것. 독은 아래 onHit 가 건다
            MonkStrike.Use(sprite, Skill, 1000, 0, 500, 0x84, target =>
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
