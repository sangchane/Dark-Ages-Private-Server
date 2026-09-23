using Darkages.Types;

namespace Darkages.Network.ServerFormats
{
    /// <summary>
    /// What is on somebody else — a buff or a debuff on another player or a monster, told to everybody who can see
    /// them. **This is our own packet, not the original's.** The original tells only the one afflicted (0x3A,
    /// 5.99 <c>SSpelled</c> → the own status bar <c>SpelledViewPane</c>), and neither the 5.99 client nor 7.x
    /// registers 0x5C, so it is a number no original packet uses.
    /// </summary>
    /// <remarks>
    /// serial(4) · icon(2, the same spell001 frame 0x3A sends) · grade(1, 6..1, 0 = over — <c>Debuff.Display</c>) ·
    /// harmful(1, 1 debuff · 0 buff) · effect(2, the picture the spell drew on the target, 0 when unknown).
    /// The mobile client draws a person's as badges under the health bar and tints a monster in the effect's colour.
    /// </remarks>
    public class ServerFormat5C : NetworkFormat
    {
        public ushort Effect;
        public byte Grade;
        public bool Harmful;
        public ushort Icon;
        public int Serial;

        public ServerFormat5C()
        {
            Secured = true;
            Command = 0x5C;
        }

        public ServerFormat5C(int serial, ushort icon, byte grade, bool harmful, ushort effect) : this()
        {
            Serial = serial;
            Icon = icon;
            Grade = grade;
            Harmful = harmful;
            Effect = effect;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Serial);
            writer.Write(Icon);
            writer.Write(Grade);
            writer.Write((byte) (Harmful ? 1 : 0));
            writer.Write(Effect);
        }

        /// <summary>Tells everybody who can see <paramref name="affected" />, but not <paramref name="affected" /> — they have 0x3A.</summary>
        public static void Tell(Sprite affected, ushort icon, byte grade, bool harmful, ushort effect) =>
            affected.Show(Scope.NearbyAislingsExludingSelf,
                new ServerFormat5C(affected.Serial, icon, grade, harmful, effect));

        /// <summary>Everything on <paramref name="seen" /> now, for somebody who has just come to see it.</summary>
        public static void TellAll(Sprite seen, Aisling viewer)
        {
            if (seen == null || viewer?.Client == null || seen.Serial == viewer.Serial)
                return;

            foreach (var debuff in seen.Debuffs.Values)
                viewer.Client.Send(new ServerFormat5C(seen.Serial, debuff.Icon, debuff.Grade, true, debuff.Animation));

            foreach (var buff in seen.Buffs.Values)
                viewer.Client.Send(new ServerFormat5C(seen.Serial, buff.Icon, buff.Grade, false, 0));
        }
    }
}
