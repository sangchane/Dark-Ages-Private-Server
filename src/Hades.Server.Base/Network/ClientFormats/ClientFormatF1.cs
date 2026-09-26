namespace Darkages.Network.ClientFormats
{
    /// <summary>
    /// 봇(동료)에게 하는 말 — 우리 앱의 말이다. 원작에는 없다(사용자, 2026-09-26).
    /// </summary>
    /// <remarks>
    /// 첫 바이트가 종류: 1 부르기 · 0 보내기 · 2 주기(내 가방 칸(1) · 개수(2) — 장비면 봇에게 입히고, 겹치는 물건(포션)이면
    /// 그만큼 봇 가방으로, 개수 0 은 다) · 3 벗기기(봇 장비 자리(1) — 내 가방으로).
    /// 0xF1 인 까닭: 원작 클라이언트는 0x80 넘는 명령을 보내지 않고(0xF0 월드맵 열기와 같은 근거), 하데스는 0xF1 을
    /// <c>Undefined.cs</c> 의 빈 자리로만 두었다 — 0xF0 다음 빈 번호다.
    /// </remarks>
    public class ClientFormatF1 : NetworkFormat
    {
        public const byte Dismiss = 0;
        public const byte Call = 1;
        public const byte Give = 2;
        public const byte TakeOff = 3;

        public ClientFormatF1()
        {
            Secured = true;
            Command = 0xF1;
        }

        public byte Kind { get; set; }
        public byte Slot { get; set; }
        public ushort Count { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            Kind = reader.ReadByte();

            if (Kind == Give || Kind == TakeOff)
                Slot = reader.ReadByte();

            if (Kind == Give)
                Count = reader.ReadUInt16();
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}
