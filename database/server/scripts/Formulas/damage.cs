using System;
using Darkages.Common;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.formulas
{
    [Script("Base Damage", "Wren", "Formula used to calculate monsters Base Damage.")]
    public class Damage : DamageFormulaScript
    {
        private readonly Sprite _obj;
        private readonly Sprite _target;

        public Damage(Sprite obj, Sprite target, MonsterDamageType type)
        {
            _obj = obj;
            _target = target;
        }

        public override int Calculate(Sprite obj, Sprite target, MonsterDamageType type)
        {
            // 정의가 한 방의 세기를 적어 두면 그것이 답이다. 최소~최대 사이를 굴린다(최대까지 포함).
            // 안 적어 둔 괴물은 아래 그대로 레벨과 사람과의 레벨 차이에서 나온다.
            if (obj is Monster attacker
                && attacker.Template.DmgMin is { } least
                && attacker.Template.DmgMax is { } most)
            {
                lock (Generator.Random)
                    return Math.Max(1, Generator.Random.Next(Math.Min(least, most), Math.Max(least, most) + 1));
            }

            if (obj is Monster || obj is Mundane)
            {
                var mod = 0.0;
                var diff = 0;

                if (target is Aisling aisling)
                    diff = obj.Level + 1 - aisling.ExpLevel;

                if (target is Monster monster)
                    diff = obj.Level + 1 - monster.Template.Level;

                if (diff <= 0)
                    mod = obj.Level * (type == MonsterDamageType.Physical ? 0.1 : 2) * ServerContext.Config.BaseDamageMod;
                else
                    mod = obj.Level * (type == MonsterDamageType.Physical ? 0.1 : 2) * (ServerContext.Config.BaseDamageMod * diff);

                var dmg = Math.Abs((int)(mod + 1));

                if (dmg <= 0)
                    dmg = 1;

                return dmg;
            }

            return 1;
        }
    }
}
