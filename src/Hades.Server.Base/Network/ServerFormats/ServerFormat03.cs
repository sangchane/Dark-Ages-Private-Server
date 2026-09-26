#region

using System;
using System.Net;
using System.Text;
using Darkages.Types;

#endregion

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat03 : NetworkFormat
    {
        public ServerFormat03()
        {
            Secured = false;
            Command = 0x03;
        }

        public IPEndPoint EndPoint { get; set; }

        public Redirect Redirect { get; set; }
        // 이름은 WriteStringA 가 949(한글 2바이트)로 쓴다 — 글자 수로 세면 한글 이름에서 입장권 길이가 모자라, 받는 쪽이
        // 입장권을 잘라 읽고 로그인이 끝나지 않았다(봇 "동료사제", 2026-09-26). 쓰는 바이트 수로 센다.
        public byte Remaining => (byte) (Redirect.Salt.Length + NameEncoding.GetByteCount(Redirect.Name) + 7);

        private static readonly Encoding NameEncoding = Encoding.GetEncoding(949);

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(EndPoint);
            writer.Write(Remaining);
            writer.Write(Convert.ToByte(Redirect.Seed));
            writer.Write(
                (byte) Redirect.Salt.Length);
            writer.Write(Encoding.UTF8.GetBytes(Redirect.Salt));
            writer.WriteStringA(Redirect.Name);
            writer.Write(Convert.ToInt32(Redirect.Serial));
        }
    }
}