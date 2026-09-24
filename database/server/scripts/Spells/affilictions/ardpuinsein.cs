#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Storage.locales.debuffs;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Spells
{
    [Script("ard puinsein", "Dean")]
    public class ardpuinsein : SpellScript
    {
        private readonly Random rand = new Random();

        public ardpuinsein(Spell spell) : base(spell)
        {
        }

        public void AlreadyCursed(GameClient client, List<Debuff_poison> curses)
        {
            var c = curses.FirstOrDefault();
            if (c != null)
                client.SendMessage(0x02, $"이미 중독되어 있습니다. [{c.Name}]");
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

                var debuff = new Debuff_poison("ard puinsein", 700, 35, 25, 0.02);
                var curses = target.Debuffs.Values.OfType<Debuff_poison>().ToList();

                if (curses.Count == 0)
                {
                    if (!target.HasDebuff(debuff.Name))
                    {
                        debuff.OnApplied(target, debuff);

                        SendCastSpellOrder(sprite, target, client);
                    }
                }
                else
                {
                    AlreadyCursed(client, curses);
                }
            }
            else
            {
                var debuff = new Debuff_poison("ard puinsein", 700, 35, 25, 0.02);
                var curses = target.Debuffs.Values.OfType<Debuff_poison>().ToList();

                if (curses.Count == 0)
                    if (!target.HasDebuff(debuff.Name))
                    {
                        debuff.OnApplied(target, debuff);

                        SpriteSpellCastOrder(sprite, target);
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

        public void SpriteSpellCastOrder(Sprite sprite, Sprite target)
        {
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

            target.Show(Scope.NearbyAislings, hpbar);
            target.SendAnimation(Spell.Template.Animation, target, sprite);

            if (target is Aisling)
                (target as Aisling).Client
                    .SendMessage(0x02,
                        $"{(sprite is Monster ? (sprite as Monster).Template.Name : (sprite as Mundane).Template.Name) ?? "괴물"}이(가) {Spell.Template.Name}(으)로 공격합니다.");

            sprite.Show(Scope.NearbyAislings, action);
        }

        private void SendCastSpellOrder(Sprite sprite, Sprite target, GameClient client, bool IsAttack = false)
        {
            var hpbar = new ServerFormat13
            {
                Serial = sprite.Serial,
                Health = 255,
                Sound = Spell.Template.Sound
            };

            var action = new ServerFormat1A
            {
                Serial = sprite.Serial,
                Number = (byte) (client.Aisling.Path == Class.Priest ? 0x80 :
                    client.Aisling.Path == Class.Wizard ? 0x88 : 0x06),
                Speed = 30
            };

            client.SendStats(StatusFlags.StructB | StatusFlags.StructD);
            client.Aisling.Show(Scope.NearbyAislings, hpbar);
            client.SendAnimation(Spell.Template.Animation, target, sprite);
            client.SendMessage(0x02, $"{Spell.Template.Name}을(를) 외웠습니다.");

            if (IsAttack)
                if (target is Aisling)
                    (target as Aisling).Client.SendMessage(0x02,
                        $"{client.Aisling.Username}님이 {Spell.Template.Name}(으)로 공격합니다.");

            client.Aisling.Show(Scope.NearbyAislings, action);
        }
    }
}