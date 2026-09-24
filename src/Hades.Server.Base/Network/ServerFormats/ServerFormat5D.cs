using Darkages.Types;

namespace Darkages.Network.ServerFormats
{
    /// <summary>
    /// How much one blow took or one heal gave, as a number — for the mobile client's floating figures.
    /// **This is our own packet, not the original's.** The original tells only a percentage (0x13 health bar), and
    /// neither the 5.99 client nor 7.x registers 0x5D, so it is a number no original packet uses (like 0x5C).
    /// </summary>
    /// <remarks>
    /// target serial(4) · source serial(4, 0 when nobody in particular) · amount(4, what really changed after
    /// armour and the cap) · kind(1, 0 damage · 1 heal). Big-endian like the rest. Sent to the same people who see
    /// the target's health bar; a heal that changed nothing (already full) is not sent.
    /// </remarks>
    public class ServerFormat5D : NetworkFormat
    {
        public const byte Damage = 0;
        public const byte Heal = 1;

        public int Amount;
        public byte Kind;
        public int Serial;
        public int Source;

        public ServerFormat5D()
        {
            Secured = true;
            Command = 0x5D;
        }

        public ServerFormat5D(int serial, int source, int amount, byte kind) : this()
        {
            Serial = serial;
            Source = source;
            Amount = amount;
            Kind = kind;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Serial);
            writer.Write(Source);
            writer.Write(Amount);
            writer.Write(Kind);
        }

        /// <summary>Tells everybody near <paramref name="target" /> (and the target) what one blow took.</summary>
        public static void Hurt(Sprite target, Sprite source, int amount)
        {
            if (target == null || amount <= 0)
                return;

            target.Show(Scope.VeryNearbyAislings, new ServerFormat5D(target.Serial, source?.Serial ?? 0, amount, Damage));
        }

        /// <summary>
        /// Tells everybody near <paramref name="target" /> how much health came back — the difference between
        /// <paramref name="before" /> and what the target has now, so a capped heal shows what it really gave.
        /// </summary>
        public static void Healed(Sprite target, Sprite source, int before)
        {
            if (target == null || target.CurrentHp <= before)
                return;

            target.Show(Scope.NearbyAislings,
                new ServerFormat5D(target.Serial, source?.Serial ?? 0, target.CurrentHp - before, Heal));
        }
    }
}
