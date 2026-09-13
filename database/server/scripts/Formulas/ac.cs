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
        /// Puts a blow through the target's armour. Ac runs from +100 on somebody wearing nothing
        /// (SetAislingStartupVariables hands out 100 - Level / 3) down to a floor of -70 with real armour on,
        /// so the multiplier runs from about 2.03 down to about 0.31 and -2 is where armour starts helping.
        /// </summary>
        /// <remarks>
        /// This used to end with <c>return calculatedDmg + Math.Abs(value - calculatedDmg)</c>, which is the
        /// larger of the two numbers — so every reduction the multiplier worked out was handed straight back
        /// and armour could only ever make a blow hurt more. Naked took 2.03 times, and the best armour in
        /// the game took exactly the same as no armour at all.
        ///
        /// <c>Math.Abs</c> is gone from the multiplier for the same reason it hid this: Ac cannot reach -101,
        /// so it never did anything, and if the floor in <c>Sprite.Ac</c> were ever lifted it would turn
        /// excellent armour back into a liability without a word.
        /// </remarks>
        public override int Calculate(Sprite obj, int value)
        {
            var armor = obj.Ac;

            var calculatedDmg = value * (armor + 101) / 99;

            // A blow that lands is worth something. Below one rather than below zero, because the division
            // truncates: a small blow against good armour reaches nothing long before it could go negative.
            if (calculatedDmg < 1)
                calculatedDmg = 1;

            return calculatedDmg;
        }
    }
}
