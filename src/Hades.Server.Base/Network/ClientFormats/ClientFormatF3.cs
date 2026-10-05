using System.Text;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormatF3 : NetworkFormat
    {
        public ClientFormatF3() { Secured = true; Command = 0xF3; }
        public string Payload { get; private set; }
        public override void Serialize(NetworkPacketReader reader)
        {
            // ReadStringB uses CP949. App JSON escapes non-ASCII characters, keeping the wire ASCII.
            if (reader.Position + 2 > reader.Packet.Data.Length) return;
            int position = reader.Position;
            int length = reader.ReadUInt16();
            reader.Position = position;
            if (length > 2048 || position + 2 + length != reader.Packet.Data.Length) return;
            Payload = reader.ReadStringB();
            if (Encoding.UTF8.GetByteCount(Payload) > 2048) Payload = null;
        }
        public override void Serialize(NetworkPacketWriter writer) { }
    }
}
