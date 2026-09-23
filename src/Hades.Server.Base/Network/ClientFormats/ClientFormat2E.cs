namespace Darkages.Network.ClientFormats
{
    public class ClientFormat2E : NetworkFormat
    {
        public ClientFormat2E()
        {
            Secured = true;
            Command = 0x2E;
        }

        public string Name { get; set; }
        public bool ShowOnMap { get; set; }
        public byte Type { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            Type = reader.ReadByte();

            // 1 청하기(모집창) · 2 청하기 · 3 받아들이기 — 셋 다 뒤에 이름 하나 (Arbiter ClientGroupAction).
            if (Type == 0x01 || Type == 0x02 || Type == 0x03)
                Name = reader.ReadStringA();

            if (Type == 0x08)
            {
                Name = reader.ReadStringA();
                ShowOnMap = reader.ReadBool();
            }
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}