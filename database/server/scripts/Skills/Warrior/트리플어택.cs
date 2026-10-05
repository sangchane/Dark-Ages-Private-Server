using Darkages.Common;
using Darkages.Types;

namespace Darkages.Scripting.Scripts.Skills
{
    /// <summary>
    /// 트리플어택 — 「일정확률로 기본공격이 3번 나간다」(5.99 Skill.txt). 전사 41레벨에 더블어택을 지우며 저절로 배운다(사용자 2026-10-05).
    /// 기본공격 다음에 <see cref="Chance" /> % 로 두 번 더 친다. 확률은 더블어택과 같다.
    /// </summary>
    [Script("트리플어택", "Warrior")]
    public class TripleAttack : Assail
    {
        public const int Chance = DoubleAttack.Chance;

        public TripleAttack(Skill skill) : base(skill)
        {
        }

        public override void OnUse(Sprite sprite)
        {
            int roll;
            lock (Generator.Random)
                roll = Generator.Random.Next(100);

            if (roll >= Chance)
                return;

            base.OnUse(sprite);
            base.OnUse(sprite);
        }
    }
}
