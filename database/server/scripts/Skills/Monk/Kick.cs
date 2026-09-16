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
            // 단각 — 5.99: 공격력 ÷10 ×28 = 평타의 2.8배. 지구력 계수 없음.
            // 평타 2.8배를 편 것: 힘 ×11.2 + 민첩성 ×5.6
            MonkStrike.Use(sprite, Skill, 1120, 0, 560, 0x83);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
