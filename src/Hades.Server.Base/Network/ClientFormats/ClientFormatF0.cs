namespace Darkages.Network.ClientFormats
{
    /// <summary>
    /// "월드맵을 열어 줘." 원작에는 없는 말이다 — 원작은 바닥의 숨은 칸을 밟아야 열렸고, 모바일에서는
    /// 그 칸을 찾을 길이 없어 메뉴 단추를 두었다(사용자, 2026-09-19). 원작 클라이언트는 0x80 넘는 명령을
    /// 보내지 않으므로 이 번호는 우리 클라이언트 전용이다.
    /// </summary>
    public class ClientFormatF0 : NetworkFormat
    {
        public ClientFormatF0()
        {
            Secured = true;
            Command = 0xF0;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}
