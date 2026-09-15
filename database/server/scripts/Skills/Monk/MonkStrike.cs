using System;
using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Skills
{
    internal static class MonkStrike
    {
        public static void Use(
            Sprite sprite,
            Skill skill,
            int strengthMultiplier,
            int constitutionMultiplier,
            int dexterityMultiplier,
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
                var damage = aisling.Str * strengthMultiplier
                             + aisling.Con * constitutionMultiplier
                             + aisling.Dex * dexterityMultiplier;
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
