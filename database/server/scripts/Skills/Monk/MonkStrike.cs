using System;
using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    internal static class MonkStrike
    {
        /// <summary>
        /// 무도가 한 방. 원작 서버팩(5.99)이 기술마다 주는 두 계수를 그대로 받는다.
        ///
        ///   피해 = 평타 × <paramref name="attackMultiplier" /> + 지구력 × <paramref name="enduranceMultiplier" />
        ///   평타 = 힘 × 4 + 민첩성 × 2   (Assail.cs 가 쓰는 것과 같은 값)
        ///
        /// 팩의 `get_att_damage` 가 그 평타 자리다. 5.99 는 단각을 평타의 2.8배, 붕각을 3.5배에
        /// 지구력 59배를 더한 것으로 둔다 — 기술마다 **두 계수만** 다르고 모양은 하나다.
        /// Novaonline 은 같은 것을 `힘 + 상수`(단각 75 · 붕각 114 · 선풍각 184)로 적었는데, 순서가
        /// 5.99 와 같다. 상수는 레벨이 올라도 안 커지므로 배율 쪽을 쓴다.
        /// 혼든은 자릿수가 100배라(공격력×100 + 지구력×2500) 버린다.
        ///
        /// 백분율로 받는 이유는 2.8배·3.5배처럼 정수가 아닌 계수가 있어서다.
        /// </summary>
        /// <summary>
        /// 최대 체력에 비례하는 한 방. 달마신공이 그것이고, 5.99 와 Novaonline 이 글자까지 같은
        /// `최대체력 ÷ 100 × 30` 을 쓴다. **쓴 만큼 제 체력도 준다** — 그것도 두 팩이 같다.
        /// </summary>
        public static void UseVitality(Sprite sprite, Skill skill, int percent, byte motion)
        {
            if (!(sprite is Aisling aisling))
                return;

            var cost = aisling.MaximumHp / 100 * percent;
            Use(sprite, skill, 0, 0, motion, _ => { });

            aisling.CurrentHp = Math.Max(1, aisling.CurrentHp - cost);
            aisling.Client.SendStats(StatusFlags.StructB);
        }

        public static void Use(
            Sprite sprite,
            Skill skill,
            int attackMultiplier,
            int enduranceMultiplier,
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
                // 평타. Assail.cs:55 가 쓰는 것과 같은 값이라, 기술은 「평타의 몇 배」로 말할 수 있다.
                var blow = aisling.Str * 4 + aisling.Dex * 2;

                var damage = blow * attackMultiplier / 100
                             + aisling.Con * enduranceMultiplier;

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
