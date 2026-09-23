namespace Darkages.Network.ServerFormats
{
    /// <summary>
    /// 그룹 청을 받은 사람에게 묻는다 — "누가 그룹을 청합니다". 받은 쪽은 0x2E 3(수락)과 청한 이의 이름으로 답한다.
    /// </summary>
    /// <remarks>
    /// 원작 모양: 종류 한 바이트(1 = 묻기) 뒤에 이름(길이 한 바이트 + 글자). Arbiter
    /// <c>ServerGroupMessage</c>·<c>ServerGroupAction.Ask</c>. 5.99 한국 클라이언트에도 이 창이 있다
    /// (<c>Legend.exe</c> 의 <c>GroupAskList</c>·<c>GroupAlertPane</c>·<c>packet\SGroup.cpp</c>).
    /// </remarks>
    public class ServerFormat63 : NetworkFormat
    {
        public const byte Ask = 0x01;

        public ServerFormat63(byte type, string name) : this()
        {
            Type = type;
            Name = name;
        }

        public ServerFormat63()
        {
            Secured = true;
            Command = 0x63;
        }

        public byte Type { get; set; }
        public string Name { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Type);
            writer.WriteStringA(Name ?? string.Empty);
        }
    }
}
