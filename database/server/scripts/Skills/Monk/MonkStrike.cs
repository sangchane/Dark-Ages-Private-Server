using System;
using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    internal static class MonkStrike
    {
        /// <summary>
        /// 무도가 한 방. 능력치마다 배율을 받는다 — 백분율이라 100 이 1배다.
        ///
        ///   피해 = (힘 × 힘배율 + 지구력 × 지구력배율 + 민첩성 × 민첩성배율) ÷ 100
        ///
        /// 배율은 원작 서버팩(5.99)에서 나왔다. 팩은 기술을 「공격력의 몇 배」로 적는데, 그
        /// `get_att_damage` 자리에 하데스의 평타(`Assail.cs:55` 의 <c>힘×4 + 민첩성×2</c>)를 놓고
        /// 펴면 능력치 배율이 된다 — 단각의 2.8배는 <c>힘 ×11.2 + 민첩성 ×5.6</c> 이다.
        ///
        /// **그 자리 맞춤은 추측이다.** 팩 엔진의 `공격력` 이 무엇을 세는지는 우리 자료에 없고
        /// (명령 문서에 옵코드 `0x8C` 만 있다), 하데스 평타와 같다고 본 것이다. 팩이 따로 주는
        /// 지구력 계수(붕각 59 · 선풍각 66)는 그대로 옮긴다.
        ///
        /// Novaonline 은 같은 것을 `힘 + 상수`(단각 75 · 붕각 114 · 선풍각 184)로 적었다. 순서는
        /// 5.99 와 같지만 상수는 레벨이 올라도 안 커져서 쓰지 않는다. 혼든은 자릿수가 100배다.
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
            Use(sprite, skill, 0, 0, 0, motion);

            aisling.CurrentHp = System.Math.Max(1, aisling.CurrentHp - cost);
            aisling.Client.SendStats(StatusFlags.StructB);
        }

        public static void Use(
            Sprite sprite,
            Skill skill,
            int strengthPercent,
            int endurancePercent,
            int agilityPercent,
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
                var damage = (aisling.Str * strengthPercent
                              + aisling.Con * endurancePercent
                              + aisling.Dex * agilityPercent) / 100;

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
