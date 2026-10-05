using Darkages.Common;
using Darkages.Types;

namespace Darkages.Scripting.Scripts.Skills
{
    /// <summary>
    /// 더블어택 — 「일정확률로 기본공격이 2번 나간다」(5.99 Skill.txt, script_do SKILL_기본공격). 전사 11레벨에 저절로 배운다
    /// (사용자 2026-10-05). 평타 단추는 배운 평타형 기술을 모두 쓰므로 기본공격 다음에 이것이 <see cref="Chance" /> % 로 한 번 더 친다.
    /// 확률은 팩에 없어 양의신권과 같게 둔다.
    /// </summary>
    [Script("더블어택", "Warrior")]
    public class DoubleAttack : Assail
    {
        public const int Chance = 30;

        public DoubleAttack(Skill skill) : base(skill)
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
