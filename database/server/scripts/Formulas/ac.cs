using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.formulas
{
    [Script("AC Formula", "Wren", "Base Formula for all AC damage calculations.")]
    public class Ac : FormulaScript
    {
        private readonly Sprite _obj;

        public Ac(Sprite obj)
        {
            _obj = obj;
        }

        /// <summary>
        /// Puts a blow through the target's armour. 원작 5.99 `0x4150f2` 식 그대로다(사용자 2026-10-04):
        /// d + trunc(d × AC × k), k = 0.01(AC &gt; 0) · 0.009(AC ≤ 0). 맨몸 +100 이면 ×2, 0 이면 ×1, −70 이면 ×0.37.
        /// </summary>
        /// <remarks>
        /// −70 하한(<c>Sprite.Ac</c>)은 남긴다 — 원작엔 없지만 AC 가 −112 아래로 가면 배수가 음수가 된다.
        /// 예전 하데스 식 <c>d × (AC+101) / 99</c> 는 원작과 맨몸 2.03·−70 0.31 로 조금씩 어긋났다.
        /// </remarks>
        public override int Calculate(Sprite obj, int value) => Apply(value, obj.Ac);

        /// <summary>원작 5.99 `0x4150f2` — 상수 [0x481298] 0.01 · [0x4812a0] 0.009. 들어간 한 방은 적어도 1.</summary>
        public static int Apply(int dmg, int armor)
        {
            var k = armor > 0 ? 0.01 : 0.009;
            var calculatedDmg = dmg + (int) ((long) dmg * armor * k);

            // A blow that lands is worth something.
            return calculatedDmg < 1 ? 1 : calculatedDmg;
        }
    }
}
