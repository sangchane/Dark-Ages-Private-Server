#region

using System;
using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Spells
{
    [Script("beag puinsein", "Dean")]
    public class beagpuinsein : SpellScript
    {
        private readonly Random rand = new Random();

        public beagpuinsein(Spell spell) : base(spell)
        {
        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {
            if (sprite is Aisling)
            {
                (sprite as Aisling)
                    .Client
                    .SendMessage(0x02, "걸리지 않습니다.");
                (sprite as Aisling)
                    .Client
                    .SendAnimation(33, target, sprite);
            }
        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                client.TrainSpell(Spell);

                var debuff = new Debuff_poison("beag puinsein", 100, 35, 25, 0.10);
                var curses = target.Debuffs.Values.OfType<Debuff_poison>().ToList();

                if (curses.Count == 0)
                {
                    if (!target.HasDebuff(debuff.Name))
                    {
                        debuff.OnApplied(target, debuff);

                        if (target is Aisling)
                            (target as Aisling).Client
                                .SendMessage(0x02,
                                    $"{client.Aisling.Username}님이 {Spell.Template.Name}(으)로 공격합니다.");

                        client.SendMessage(0x02, $"{Spell.Template.Name}을(를) 외웠습니다.");
                        client.SendAnimation(Spell.Template.Animation, target, sprite);

                        var action = new ServerFormat1A
                        {
                            Serial = sprite.Serial,
                            Number = (byte) (client.Aisling.Path == Class.Priest ? 0x80 :
                                client.Aisling.Path == Class.Wizard ? 0x88 : 0x06),
                            Speed = 30
                        };

                        var hpbar = new ServerFormat13
                        {
                            Serial = sprite.Serial,
                            Health = 255,
                            Sound = Spell.Template.Sound
                        };

                        client.Aisling.Show(Scope.NearbyAislings, action);
                        client.Aisling.Show(Scope.NearbyAislings, hpbar);
                    }
                }
                else
                {
                    var c = curses.FirstOrDefault();
                    if (c != null)
                        client.SendMessage(0x02, $"이미 중독되어 있습니다. [{c.Name}]");
                }
            }
            else
            {
                var debuff = new Debuff_poison("beag puinsein", 100, 35, 25, 0.10);
                var curses = target.Debuffs.Values.OfType<Debuff_poison>().ToList();

                if (curses.Count == 0)
                    if (!target.HasDebuff(debuff.Name))
                    {
                        debuff.OnApplied(target, debuff);

                        if (target is Aisling)
                            (target as Aisling).Client
                                .SendMessage(0x02,
                                    $"{(sprite is Monster ? (sprite as Monster).Template.Name : (sprite as Mundane).Template.Name) ?? "괴물"}이(가) {Spell.Template.Name}(으)로 공격합니다.");

                        target.SendAnimation(Spell.Template.Animation, target, sprite);

                        var action = new ServerFormat1A
                        {
                            Serial = sprite.Serial,
                            Number = 1,
                            Speed = 30
                        };

                        var hpbar = new ServerFormat13
                        {
                            Serial = target.Serial,
                            Health = 255,
                            Sound = Spell.Template.Sound
                        };

                        sprite.Show(Scope.NearbyAislings, action);
                        target.Show(Scope.NearbyAislings, hpbar);
                    }
            }
        }

        public override void OnUse(Sprite sprite, Sprite target)
        {
            if (sprite is Aisling)
            {
                if (sprite.CurrentMp - Spell.Template.ManaCost > 0)
                {
                    sprite.CurrentMp -= Spell.Template.ManaCost;
                }
                else
                {
                    if (sprite is Aisling)
                        (sprite as Aisling).Client.SendMessage(0x02, ServerContext.Config.NoManaMessage);
                    return;
                }

                if (sprite.CurrentMp < 0)
                    sprite.CurrentMp = 0;
            }

            if (rand.Next(0, 100) > target.Mr)
                OnSuccess(sprite, target);
            else
                OnFailed(sprite, target);

            if (sprite is Aisling)
                (sprite as Aisling)
                    .Client
                    .SendStats(StatusFlags.StructB);
        }
    }
}