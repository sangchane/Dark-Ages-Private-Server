#region

using System.Collections.Generic;
using Darkages.Types;

#endregion

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat07 : NetworkFormat
    {
        private readonly List<Sprite> Sprites;

        public ServerFormat07(Sprite[] objectsToAdd)
        {
            Secured = true;
            Command = 0x07;
            Sprites = new List<Sprite>(objectsToAdd);
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            if (Sprites.Count > 0)
            {
                writer.Write((ushort) Sprites.Count);

                foreach (var sprite in Sprites)
                {
                    if (sprite is Money || sprite is Item)
                    {
                        // Written in the same shape as everything else in this packet: place, serial,
                        // drawing, four bytes spare, a direction, one spare, and what kind of thing it is.
                        //
                        // It used to be written four bytes shorter, with no direction and no kind, and
                        // nothing in the record said so - a reader could not tell a dropped item from the
                        // first thirteen bytes of a monster, and everything after it in the same packet was
                        // read at the wrong offset. Walk-through is what the original calls a thing you
                        // step over rather than fight.
                        writer.Write((ushort) sprite.XPos);
                        writer.Write((ushort) sprite.YPos);
                        writer.Write((uint) sprite.Serial);
                        writer.Write(sprite is Money money ? money.Image : ((Item) sprite).DisplayImage);
                        writer.Write((uint) 0x0);
                        writer.Write(sprite.Direction);
                        writer.Write(sprite is Item dropped ? dropped.Color : byte.MinValue);
                        writer.Write((byte) 0x01);
                    }

                    if (sprite is Monster)
                    {
                        writer.Write((ushort) sprite.XPos);
                        writer.Write((ushort) sprite.YPos);
                        writer.Write((uint) sprite.Serial);
                        writer.Write((sprite as Monster).Image);
                        writer.Write((uint) 0x0);
                        writer.Write(sprite.Direction);
                        writer.Write(byte.MinValue);
                        writer.Write(byte.MinValue);
                    }

                    if (sprite is Mundane)
                    {
                        writer.Write((ushort) sprite.XPos);
                        writer.Write((ushort) sprite.YPos);
                        writer.Write((uint) sprite.Serial);
                        writer.Write((ushort) (sprite as Mundane).Template.Image);
                        writer.Write(uint.MinValue);
                        writer.Write(sprite.Direction);
                        writer.Write(byte.MinValue);
                        writer.Write((byte) 0x02);
                        writer.WriteStringA((sprite as Mundane).Template.Name);
                    }
                }
            }
        }
    }
}