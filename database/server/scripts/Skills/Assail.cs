#region

using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Types;

#endregion

namespace Darkages.Scripting.Scripts.Skills
{
    [Script("Assail", "Test")]
    public class Assail : SkillScript
    {
        public Skill _skill;

        public Sprite Target;

        // 등 뒤 ×2 · 옆 ×1.5 · 정면 ×1 은 여기가 아니라 피해가 들어가는 공통 길에 있다
        // (Sprite.BlowFacing). 평타만이 아니라 때리는 기술이 모두 같은 판정을 받는다.

        /// <summary>5.99 공격속성 배수(Novaonline.exe 0x415cff — 속성 1~5 면 ×13/10).</summary>
        private const double MonsterBlowElement = 1.3;

        public Assail(Skill skill) : base(skill)
        {
            _skill = skill;
        }

        public override void OnFailed(Sprite sprite)
        {
            if (Target != null)
                if (sprite is Aisling)
                {
                    var client = (sprite as Aisling).Client;
                    client.Aisling.Show(Scope.NearbyAislings,
                        new ServerFormat29(Skill.Template.MissAnimation, (ushort) Target.XPos, (ushort) Target.YPos));
                }
        }

        /// <summary>
        /// 평타의 몸 동작과 속도. 5.99 서버(Novaonline.exe 0x4160f7)대로 무기를 꼈으면 무기의 공격모션·공격속도, 무기가
        /// 없으면 갑옷의 것 — 칸이 0 이면 동작은 1, 속도는 20(갑옷만 입고 둘 다 0 이면 22). 방패·직업·배운 기술은 보지
        /// 않는다. 공격모션이 없는 하데스 무기는 하데스가 하던 대로 전사 양손이면 0x81 이다.
        /// </summary>
        private static (byte Number, short Speed) BlowMotion(Aisling aisling)
        {
            var weapon = aisling.EquipmentManager?.Weapon?.Item?.Template;

            if (weapon != null)
            {
                if (weapon.AttackMotion == 0 && weapon.AttackSpeed == 0)
                    return ((byte) (aisling.Path == Class.Warrior && aisling.UsingTwoHanded ? 0x81 : 0x01), 20);

                return (weapon.AttackMotion == 0 ? (byte) 1 : weapon.AttackMotion,
                    (short) (weapon.AttackSpeed == 0 ? 20 : weapon.AttackSpeed));
            }

            var armor = aisling.EquipmentManager?.Armor?.Item?.Template;

            if (armor != null)
                return (armor.AttackMotion == 0 ? (byte) 1 : armor.AttackMotion,
                    (short) (armor.AttackSpeed != 0 ? armor.AttackSpeed : armor.AttackMotion != 0 ? 20 : 22));

            return (aisling.Path == Class.Monk ? (byte) 132 : (byte) 1, 20);
        }

        public override void OnSuccess(Sprite sprite)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                var (number, speed) = BlowMotion(client.Aisling);
                var action = new ServerFormat1A
                {
                    Serial = client.Aisling.Serial,
                    Number = number,
                    Speed = speed
                };

                var enemy = client.Aisling.GetInfront();
                var success = false;

                if (enemy != null)
                {
                    var imp = 10 + Skill.Level;
                    var dmg = client.Aisling.Str * 4 + client.Aisling.Dex * 2;

                    dmg += dmg * imp / 100;

                    if (sprite.EmpoweredAssail)
                        if (((Aisling) sprite).Weapon == 0)
                            dmg *= 3;

                    foreach (var i in from i in enemy
                        where i != null
                        where client.Aisling.Serial != i.Serial
                        where !(i is Money)
                        where i.Attackable
                        select i)
                    {
                        Target = i;

                        i.ApplyDamage(sprite, dmg, Skill.Template.Sound);
                        success = true;

                        if (i is Aisling)
                        {
                            (i as Aisling).Client.Aisling.Show(Scope.NearbyAislings,
                                new ServerFormat29((uint) client.Aisling.Serial, (uint) i.Serial, byte.MinValue,
                                    Skill.Template.TargetAnimation, 100));
                            (i as Aisling).Client.Send(new ServerFormat08(i as Aisling, StatusFlags.All));
                        }

                        if (i is Monster || i is Mundane || i is Aisling)
                            client.Aisling.Show(Scope.NearbyAislings,
                                new ServerFormat29((uint) client.Aisling.Serial, (uint) i.Serial,
                                    Skill.Template.TargetAnimation, 0, 100));
                    }
                }

                if (!success)
                {
                    client.Aisling.Show(Scope.VeryNearbyAislings, new ServerFormat13(0, 0, Skill.Template.Sound));
                    Darkages.Storage.locales.Scripts.Skills.MonkStrike.ShowMiss(client.Aisling, Skill);
                }

                client.Aisling.Show(Scope.NearbyAislings, action);
            }
        }

        public override void OnUse(Sprite sprite)
        {
            if (sprite is Aisling aisling)
            {
                if (Skill.Level < Skill.Template.MaxLevel)
                    aisling.Client.TrainSkill(Skill);

                if (aisling.Invisible)
                {
                    aisling.Invisible = false;
                    aisling.Client.Refresh();
                }

                OnSuccess(sprite);
            }
            else
            {
                if (!Skill.Ready)
                    return;

                var enemy = sprite.GetInfront();

                var action = new ServerFormat1A
                {
                    Serial = sprite.Serial,
                    Number = 0x01,
                    Speed = 30
                };

                if (enemy != null)
                    foreach (var i in from i in enemy
                        where i != null
                        where sprite.Serial != i.Serial
                        where !(i is Money)
                        where i.Attackable
                        select i)
                    {
                        Target = i;

                        var dmg = sprite.GetBaseDamage(Target, MonsterDamageType.Physical);
                        {
                            // 5.99 는 괴물 평타를 방어로 거른 뒤 공격속성으로 ×1.3 한다(Novaonline.exe 0x425dc3 →
                            // 0x415cff). 공격속성이 안 적힌 괴물에게도 생길 때 1~4 를 붙이므로(0x422bc5) 괴물 평타는
                            // 늘 ×1.3 이다. 괴물 마법(`char_damaged2`)은 이 단계를 거치지 않는다.
                            if (sprite is Monster)
                                i.ApplyDamageAfterArmour(sprite, dmg, MonsterBlowElement);
                            else
                                i.ApplyDamage(sprite, dmg, sprite.OffenseElement);
                        }

                        if (Skill.Template.TargetAnimation > 0)
                            if (i is Monster || i is Mundane || i is Aisling)
                                sprite.Show(Scope.NearbyAislings,
                                    new ServerFormat29((uint) sprite.Serial, (uint) i.Serial,
                                        Skill.Template.TargetAnimation, 0, 100));

                        sprite.Show(Scope.NearbyAislings, action);
                    }
            }
        }
    }
}