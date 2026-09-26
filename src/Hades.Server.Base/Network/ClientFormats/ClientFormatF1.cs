namespace Darkages.Network.ClientFormats
{
    /// <summary>
    /// "동료를 불러 줘 / 보내 줘." 원작에는 없는 말이다 — 모바일 앱의 [동료 부르기] 단추(사용자, 2026-09-26).
    /// </summary>
    /// <remarks>
    /// 몸은 한 바이트: 1 부르기 · 0 보내기. 0xF1 인 까닭: 원작 클라이언트는 0x80 넘는 명령을 보내지 않고(0xF0 월드맵 열기와 같은
    /// 근거), 하데스는 0xF1 을 <c>Undefined.cs</c> 의 빈 자리로만 두었다 — 0xF0 다음 빈 번호다.
    /// </remarks>
    public class ClientFormatF1 : NetworkFormat
    {
        public const byte Dismiss = 0;
        public const byte Call = 1;

        public ClientFormatF1()
        {
            Secured = true;
            Command = 0xF1;
        }

        public byte Kind { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            Kind = reader.ReadByte();
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}
