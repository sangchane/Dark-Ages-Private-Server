using System.Collections.Generic;

namespace Darkages.Network.ClientFormats
{
    /// <summary>모바일 상점 일괄 거래. 원작에는 없고 0xF2 예약 슬롯을 사용한다.</summary>
    public class ClientFormatF2 : NetworkFormat
    {
        public const byte Buy = 1;
        public const byte Sell = 2;
        public const byte BackToMenu = 3;

        public ClientFormatF2()
        {
            Secured = true;
            Command = 0xF2;
        }

        public byte Kind { get; private set; }
        public uint Merchant { get; private set; }
        public List<BulkTradeLine> Lines { get; } = new();

        public override void Serialize(NetworkPacketReader reader)
        {
            Kind = reader.ReadByte();
            Merchant = reader.ReadUInt32();
            ushort count = reader.ReadUInt16();

            if (count > 128)
                return;

            for (int i = 0; i < count && reader.GetCanRead(); i++)
            {
                string name = Kind == Buy ? reader.ReadStringA() : string.Empty;
                byte slot = Kind == Sell ? reader.ReadByte() : (byte)0;
                ushort quantity = reader.ReadUInt16();
                Lines.Add(new BulkTradeLine(name, slot, quantity));
            }
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }

    public sealed record BulkTradeLine(string Name, byte Slot, ushort Quantity);
}
