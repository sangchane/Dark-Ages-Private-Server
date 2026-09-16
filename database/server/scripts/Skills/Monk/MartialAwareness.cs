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
            // 붕각 — 5.99: (공격력 ×3 + 공격력 ÷2) = 평타의 3.5배, 거기에 지구력 ×59.
            // Novaonline 은 같은 자리를 `힘 + 114` 로 적는데 정권(53)·단각(75)보다 크고
            // 선풍각(184)보다 작다 — 5.99 의 배수 순서와 같다.
            // 붕각 — 평타 3.5배 + 지구력 59: 힘 ×14 + 지구력 ×59 + 민첩성 ×7
            MonkStrike.Use(sprite, Skill, 1400, 5900, 700, 0x85);
        }

        public override void OnUse(Sprite sprite)
        {
            OnSuccess(sprite);
        }
    }
}
