namespace Darkages.Network.ClientFormats
{
    /// <summary>
    /// 경매장 요청(우리 확장 0xF4, 2026-10-07). 첫 바이트(종류)가 뒤의 모양을 정한다 — <c>autopilot/loot-auction/05-api-contract.md</c> P-01~08.
    /// 본문이 그 종류에 모자라면 <see cref="Kind" /> 를 <see cref="Unknown" /> 로 두어 핸들러가 버린다.
    /// </summary>
    public class ClientFormatF4 : NetworkFormat
    {
        public const byte Browse = 0;
        public const byte Mine = 1;
        public const byte Claims = 2;
        public const byte Post = 3;
        public const byte Bid = 4;
        public const byte Buyout = 5;
        public const byte Cancel = 6;
        public const byte Take = 7;
        public const byte Unknown = 0xFF;

        public ClientFormatF4()
        {
            Secured = true;
            Command = 0xF4;
        }

        public byte Kind { get; private set; } = Unknown;
        public byte Category { get; private set; }
        public byte Sort { get; private set; }
        public ushort Page { get; private set; }
        public string Query { get; private set; } = string.Empty;
        public byte Slot { get; private set; }
        public uint Start { get; private set; }
        public uint BuyoutPrice { get; private set; }
        public byte Hours { get; private set; }
        public uint Id { get; private set; }
        public uint Amount { get; private set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            int length = reader.Packet.Data.Length - reader.Position;
            if (length < 1)
                return;

            byte kind = reader.ReadByte();
            int need = kind switch
            {
                Browse => 1 + 1 + 1 + 2 + 1,
                Mine or Claims => 1 + 2,
                Post => 1 + 1 + 4 + 4 + 1,
                Bid => 1 + 4 + 4,
                Buyout or Cancel or Take => 1 + 4,
                _ => int.MaxValue
            };
            if (length < need)
                return;

            switch (kind)
            {
                case Browse:
                    Category = reader.ReadByte();
                    Sort = reader.ReadByte();
                    Page = reader.ReadUInt16();
                    if (reader.Position + 1 + reader.Packet.Data[reader.Position] > reader.Packet.Data.Length)
                        return;
                    Query = reader.ReadStringA();
                    break;
                case Mine:
                case Claims:
                    Page = reader.ReadUInt16();
                    break;
                case Post:
                    Slot = reader.ReadByte();
                    Start = reader.ReadUInt32();
                    BuyoutPrice = reader.ReadUInt32();
                    Hours = reader.ReadByte();
                    break;
                case Bid:
                    Id = reader.ReadUInt32();
                    Amount = reader.ReadUInt32();
                    break;
                default:
                    Id = reader.ReadUInt32();
                    break;
            }

            Kind = kind;
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}
