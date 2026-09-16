using System;
using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    internal static class MonkStrike
    {
        /// <summary>
        /// 최대 체력에 비례하는 한 방. 달마신공이 그것이고, 5.99 와 Novaonline 이 글자까지 같은
        /// `최대체력 ÷ 100 × 30` 을 쓴다. **쓴 만큼 제 체력도 준다** — 그것도 두 팩이 같다.
        /// </summary>
        public static void UseVitality(Sprite sprite, Skill skill, int percent, byte motion)
        {
            if (!(sprite is Aisling aisling))
                return;

            var cost = aisling.MaximumHp / 100 * percent;
            Use(sprite, skill, 0, 0, motion);

            aisling.CurrentHp = System.Math.Max(1, aisling.CurrentHp - cost);
            aisling.Client.SendStats(StatusFlags.StructB);
        }

        /// <summary>
        /// 무도가 한 방. 5.99 서버팩 스크립트의 모양을 그대로 쓴다 — 배율은 백분율이라 100 이 1배다.
        ///
        ///   피해 = 공격력 × 공격력배율 + 지구력 × 지구력배율
        ///   공격력 = 힘 × 4 + 민첩성 × 2 + 무기( DmgMin~DmgMax 중 하나 )
        ///
        /// 팩의 `get_att_damage` 가 「무기까지 낀 평타 최종 공격력」이다. 하데스의 평타
        /// (`Assail.cs:55`)는 <c>힘×4 + 민첩성×2</c> 뿐이라 무기가 빠져 있었는데, 무기는
        /// <c>Sprite.ApplyWeaponBonuses</c>(Sprite.cs:1018) 가 이미 굴린다 — 아이템 93장이
        /// `DmgMin`~`DmgMax` 를 들고 있다. 그것을 여기서 **곱하기 전에** 더해야 팩과 같아진다.
        /// <c>ApplyDamage</c> 안에서 더하면 배율이 안 걸린 채로 붙는다.
        /// </summary>
        public static void Use(
            Sprite sprite,
            Skill skill,
            int attackPercent,
            int endurancePercent,
            byte motion,
            Action<Sprite> onHit = null)
        {
            if (!(sprite is Aisling aisling) || !skill.Ready)
                return;

            if (skill.Level < skill.Template.MaxLevel)
                aisling.Client.TrainSkill(skill);

            if (aisling.Invisible)
            {
                aisling.Invisible = false;
                aisling.Client.Refresh();
            }

            var action = new ServerFormat1A
            {
                Serial = aisling.Serial,
                Number = motion,
                Speed = 30
            };
            var targets = aisling.GetInfront();
            var hit = false;

            if (targets != null)
            {
                // 무기까지 낀 평타. 5.99 의 `get_att_damage` 자리다.
                var blow = aisling.ApplyWeaponBonuses(aisling, aisling.Str * 4 + aisling.Dex * 2);

                var damage = blow * attackPercent / 100
                             + aisling.Con * endurancePercent / 100;

                // 기술 레벨이 오르면 1%씩 붙는다. 하데스가 쓰던 보정을 그대로 둔다.
                damage += damage * (10 + skill.Level) / 100;

                foreach (var target in targets
                    .Where(target => target != null)
                    .Where(target => target.Serial != aisling.Serial)
                    .Where(target => !(target is Money))
                    .Where(target => target.Attackable))
                {
                    target.ApplyDamage(aisling, damage, skill.Template.Sound);
                    onHit?.Invoke(target);
                    hit = true;

                    if (target is Aisling player)
                    {
                        player.Client.Aisling.Show(Scope.NearbyAislings,
                            new ServerFormat29((uint) aisling.Serial, (uint) target.Serial, byte.MinValue,
                                skill.Template.TargetAnimation, 100));
                        player.Client.Send(new ServerFormat08(player, StatusFlags.All));
                    }

                    if (target is Monster || target is Mundane || target is Aisling)
                        aisling.Show(Scope.NearbyAislings,
                            new ServerFormat29((uint) aisling.Serial, (uint) target.Serial,
                                skill.Template.TargetAnimation, 0, 100));
                }
            }

            if (!hit)
                aisling.Show(Scope.VeryNearbyAislings, new ServerFormat13(0, 0, skill.Template.Sound));

            aisling.Show(Scope.NearbyAislings, action);
        }
    }
}
