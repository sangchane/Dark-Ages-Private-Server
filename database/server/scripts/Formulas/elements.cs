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

        public override double Calculate(Sprite obj, ElementManager.Element element)
        {
            var defenseElement = obj.DefenseElement;

            while (defenseElement == ElementManager.Element.Random) defenseElement = Sprite.CheckRandomElement(defenseElement);

            if (defenseElement == ElementManager.Element.None && element != ElementManager.Element.None)
                return 1.00;

            // 속성이 없는 쪽끼리는 그대로다. 예전에는 0.50 이라 사람과 괴물 사이의 거의 모든 한 방이 반이 됐다 —
            // 5.99 서버(Novaonline.exe)의 피해 함수에는 상대 속성을 보는 표도, 반으로 깎는 단계도 없다
            // (괴물이 맞을 때 0x424215, 사람이 맞을 때 0x415341).
            if (defenseElement == ElementManager.Element.None && element == ElementManager.Element.None) return 1.00;

            if (defenseElement == ElementManager.Element.Fire)
                switch (element)
                {
                    case ElementManager.Element.Fire:
                        return 0.05;

                    case ElementManager.Element.Water:
                        return 0.85;

                    case ElementManager.Element.Wind:
                        return 0.55;

                    case ElementManager.Element.Earth:
                        return 0.65;

                    case ElementManager.Element.Dark:
                        return 0.75;

                    case ElementManager.Element.Light:
                        return 0.55;

                    case ElementManager.Element.None:
                        return 0.01;
                }

            if (defenseElement == ElementManager.Element.Wind)
                switch (element)
                {
                    case ElementManager.Element.Wind:
                        return 0.05;

                    case ElementManager.Element.Fire:
                        return 0.85;

                    case ElementManager.Element.Water:
                        return 0.65;

                    case ElementManager.Element.Earth:
                        return 0.55;

                    case ElementManager.Element.Dark:
                        return 0.75;

                    case ElementManager.Element.Light:
                        return 0.55;

                    case ElementManager.Element.None:
                        return 0.01;
                }

            if (defenseElement == ElementManager.Element.Earth)
                switch (element)
                {
                    case ElementManager.Element.Wind:
                        return 0.85;

                    case ElementManager.Element.Fire:
                        return 0.65;

                    case ElementManager.Element.Water:
                        return 0.55;

                    case ElementManager.Element.Earth:
                        return 0.05;

                    case ElementManager.Element.Dark:
                        return 0.75;

                    case ElementManager.Element.Light:
                        return 0.55;

                    case ElementManager.Element.None:
                        return 0.01;
                }

            if (defenseElement == ElementManager.Element.Water)
                switch (element)
                {
                    case ElementManager.Element.Wind:
                        return 0.65;

                    case ElementManager.Element.Fire:
                        return 0.55;

                    case ElementManager.Element.Water:
                        return 0.05;

                    case ElementManager.Element.Earth:
                        return 0.85;

                    case ElementManager.Element.Dark:
                        return 0.75;

                    case ElementManager.Element.Light:
                        return 0.55;

                    case ElementManager.Element.None:
                        return 0.01;
                }

            if (defenseElement == ElementManager.Element.Dark)
                return element switch
                {
                    ElementManager.Element.Dark => 0.10,
                    ElementManager.Element.Light => 0.80,
                    ElementManager.Element.None => 0.01,
                    _ => 0.60
                };

            if (defenseElement == ElementManager.Element.Light)
                return element switch
                {
                    ElementManager.Element.Dark => 0.80,
                    ElementManager.Element.Light => 0.10,
                    ElementManager.Element.None => 0.01,
                    _ => 0.65
                };

            return 0.00;
        }
    }
}
