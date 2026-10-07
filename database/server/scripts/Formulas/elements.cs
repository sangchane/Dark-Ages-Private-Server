using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.formulas
{
    [Script("Elements 1.0", "wren", "Default Elemental Table Script used to manage damage mods.")]
    public class Elements : ElementFormulaScript
    {
        public Elements(Sprite obj)
        {

        }

        /// <summary>
        /// 새 상성표(사용자 2026-10-04). 원작 5.99 에는 표가 없다(`0x415cff` 는 받는 쪽 속성을 읽지 않는다).
        /// 유리 ×1.3 · 같음·무관·공격 무속성 ×1.0 · 불리 ×0.7 · 방어 무속성은 속성 있는 공격을 ×1.3(10-07, <see cref="Unguarded" />).
        /// 고리는 수 &gt; 화 &gt; 풍 &gt; 토 &gt; 수. 암흑(Dark)은 넷 모두를 이기고,
        /// 생명(Light)은 암흑만 이긴다 — 생명과 넷 사이는 ×1.0.
        /// 예전 표는 값이 모두 1 이하라(유리해도 0.85, 속성 없이 치면 0.01) 속성을 켜면 전투가 크게 약해졌다.
        /// </summary>
        public override double Calculate(Sprite obj, ElementManager.Element element)
        {
            var defenseElement = obj.DefenseElement;

            while (defenseElement == ElementManager.Element.Random) defenseElement = Sprite.CheckRandomElement(defenseElement);

            return Multiplier(element, defenseElement);
        }

        public const double Advantage = 1.3;
        public const double Disadvantage = 0.7;

        /// <summary>
        /// 방어 무속성(허리띠에 속성이 없음)은 속성 있는 공격을 이만큼 받는다(사용자 2026-10-07 「없으면 공격 속성처럼 적용해」).
        /// 원작 자료에 무속성 배수는 없다 — 5.99 는 받는 쪽 속성을 읽지 않고 공격 쪽에 속성이 있으면 ×1.3(`0x415cff`), 벨트 설명은
        /// 「…의보호를 받을 수 있게 된다」뿐이다. 그래서 공격 속성과 같은 1.3.
        /// </summary>
        public const double Unguarded = 1.3;

        public static double Multiplier(ElementManager.Element attack, ElementManager.Element defense)
        {
            if (defense == ElementManager.Element.None && attack != ElementManager.Element.None)
                return Unguarded;

            if (Beats(attack, defense))
                return Advantage;

            if (Beats(defense, attack))
                return Disadvantage;

            return 1.00;
        }

        /// <summary><paramref name="a" /> 가 <paramref name="b" /> 를 이기는가 — 수 → 화 → 풍 → 토 → 수 고리 · 암흑 → 넷 · 생명 → 암흑.</summary>
        private static bool Beats(ElementManager.Element a, ElementManager.Element b) => (a, b) switch
        {
            (ElementManager.Element.Water, ElementManager.Element.Fire) => true,
            (ElementManager.Element.Fire, ElementManager.Element.Wind) => true,
            (ElementManager.Element.Wind, ElementManager.Element.Earth) => true,
            (ElementManager.Element.Earth, ElementManager.Element.Water) => true,
            (ElementManager.Element.Dark, ElementManager.Element.Water) => true,
            (ElementManager.Element.Dark, ElementManager.Element.Fire) => true,
            (ElementManager.Element.Dark, ElementManager.Element.Wind) => true,
            (ElementManager.Element.Dark, ElementManager.Element.Earth) => true,
            (ElementManager.Element.Light, ElementManager.Element.Dark) => true,
            _ => false
        };
    }
}
