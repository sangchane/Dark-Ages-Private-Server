using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    /// <summary>
    /// 붕각. 원작 이름은 세 서버팩이 기술 아이콘 3 에서 모두 일치하고, 사람이 그 아이콘을 보고
    /// 확인했다.
    /// </summary>
    [Script("Martial Awareness", "LOD")]
    public class MartialAwareness : SkillScript
    {
        public MartialAwareness(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
            // 붕각 — 공격력 ×3.5 + 지구력 ×59
            MonkStrike.Use(sprite, Skill, 350, 5900, 0x85);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
