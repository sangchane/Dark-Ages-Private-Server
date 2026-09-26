namespace Darkages.Network.ServerFormats
{
    /// <summary>
    /// 동료 사이 — 누가 누구의 동료인가. **우리 확장이다, 원작 패킷이 아니다.** 원작 4.51 클라이언트의 패킷 점프표
    /// (<c>0x510a54</c>, <c>data/disassembly/findings.json</c>)는 0x03~0x4B 뿐이고 하데스도 0x5E 를 <c>Undefined.cs</c> 의
    /// 빈 자리로만 두었다 — 0x5C·0x5D 와 같은 방식으로 고른 번호다.
    /// </summary>
    /// <remarks>
    /// 종류(1) · serial(4) · 이름(길이 한 바이트 + 글자). 빅엔디언.
    /// <list type="bullet">
    /// <item>종류 1 <see cref="Master" /> — 봇에게만: "네 주인은 이 사람이다". serial 0·이름 "" 은 풀려났다는 뜻.</item>
    /// <item>종류 2 <see cref="Companion" /> — 부른 사람에게: "네 동료는 이 봇이다". serial 0 은 동료가 없다는 뜻.</item>
    /// </list>
    /// 귓속말(0x19)로 알리지 않은 까닭: 귓속말은 아무 사람이나 봇에게 보낼 수 있어 주인을 속일 수 있다. 서버만 보내는 번호는 속일 수 없다.
    /// </remarks>
    public class ServerFormat5E : NetworkFormat
    {
        public const byte Master = 1;
        public const byte Companion = 2;

        public ServerFormat5E()
        {
            Secured = true;
            Command = 0x5E;
        }

        public ServerFormat5E(byte kind, int serial, string name) : this()
        {
            Kind = kind;
            Serial = serial;
            Name = name;
        }

        public byte Kind { get; set; }
        public int Serial { get; set; }
        public string Name { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Kind);
            writer.Write(Serial);
            writer.WriteStringA(Name ?? string.Empty);
        }
    }
}
