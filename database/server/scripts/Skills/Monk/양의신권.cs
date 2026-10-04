using Darkages.Common;
using Darkages.Types;

namespace Darkages.Scripting.Scripts.Skills
{
    /// <summary>
    /// 양의신권 — 「일정 확률로 기본공격이 2번 나간다」(템플릿 설명). 평타 단추는 배운 평타형 기술을 모두 쓰므로 기본공격 다음에
    /// 이것이 돈다 — 전에는 평타 스크립트(Assail)를 그대로 붙여 늘 두 번 쳤다. 원작 실행 파일에도 확률은 없지만(docs/disassembly.md
    /// 양의신권 줄) 설명대로 <see cref="Chance" /> % 일 때만 한 번 더 친다(사용자 2026-10-04).
    /// </summary>
    [Script("양의신권", "Monk")]
    public class YangUiSinGwon : Assail
    {
        /// <summary>두 번째 타격이 나갈 확률(%).</summary>
        public const int Chance = 30;

        public YangUiSinGwon(Skill skill) : base(skill)
        {
        }

        public override void OnUse(Sprite sprite)
        {
            int roll;
            lock (Generator.Random)
                roll = Generator.Random.Next(100);

            if (roll < Chance)
                base.OnUse(sprite);
        }
    }
}
