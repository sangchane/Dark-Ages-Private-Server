namespace Darkages.Network.ServerFormats
{
    public class ServerFormat15 : NetworkFormat
    {
        public ServerFormat15(Area area) : this()
        {
            Area = area;
        }

        public ServerFormat15()
        {
            Secured = true;
            Command = 0x15;
        }

        public Area Area { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            if (Area != null)
            {
                writer.Write((ushort) (Area.ClientNumber > 0 ? Area.ClientNumber : Area.Id));
                writer.Write((byte) Area.Cols);
                writer.Write((byte) Area.Rows);
                writer.Write((byte) Area.Flags);
                writer.Write(ushort.MinValue);
                writer.Write(Area.Hash);
                writer.WriteStringA(Area.ClientName ?? Area.Name);
            }
        }
    }
}