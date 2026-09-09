#region

using System;
using System.Net.Sockets;
using Darkages.Network.Object;

#endregion

namespace Darkages.Network
{
    public class NetworkSocket : ObjectManager
    {
        internal Socket Socket;
        private const int HeaderLength = 3;
        private const byte FrameMagic = 0xAA;

        private readonly byte[] _header = new byte[HeaderLength];
        private readonly byte[] _packet = new byte[65534];

        private int _headerOffset;
        private int _packetLength;
        private int _packetOffset;

        public NetworkSocket(Socket socket)
        {
            ConfigureTcpSocket(socket);
            Socket = socket;
        }

        public bool HeaderComplete => _headerOffset == HeaderLength;

        /// <summary>When the last bytes arrived, used to spot a frame that stopped half way.</summary>
        public DateTime LastReceivedUtc { get; private set; } = DateTime.UtcNow;

        /// <summary>
        /// True while a frame has started but not finished. A connection resting between frames is not
        /// partial, so waiting quietly is never mistaken for stalling.
        /// </summary>
        public bool HasPartialFrame => _headerOffset > 0 && (!HeaderComplete || !PacketComplete);

        public bool PacketComplete => _packetOffset == _packetLength;

        public virtual IAsyncResult BeginReceiveHeader(AsyncCallback callback, out SocketError error, object state)
        {
            return Socket.BeginReceive(
                _header,
                _headerOffset,
                HeaderLength - _headerOffset,
                SocketFlags.None,
                out error,
                callback,
                state);
        }

        public virtual IAsyncResult BeginReceivePacket(AsyncCallback callback, out SocketError error, object state)
        {
            return Socket.BeginReceive(
                _packet,
                _packetOffset,
                _packetLength - _packetOffset,
                SocketFlags.None,
                out error,
                callback,
                state);
        }

        public virtual int EndReceiveHeader(IAsyncResult result, out SocketError error)
        {
            var bytes = Socket.EndReceive(result, out error);

            if (bytes == 0)
                return 0;

            LastReceivedUtc = DateTime.UtcNow;
            _headerOffset += bytes;

            if (!HeaderComplete)
                return bytes;

            // Reject frames that cannot be a packet under any reading. Reporting a socket error here is what
            // closes this one connection: both receive callbacks already disconnect the client on it.
            if (_header[0] != FrameMagic)
            {
                error = SocketError.ProtocolNotSupported;
                return 0;
            }

            var declaredLength = (_header[1] << 8) | _header[2];

            // A frame carries at least its command byte, and can never outgrow the receive buffer.
            if (declaredLength < 1 || declaredLength > _packet.Length)
            {
                error = SocketError.MessageSize;
                return 0;
            }

            _packetLength = declaredLength;
            _packetOffset = 0;

            return bytes;
        }

        public virtual int EndReceivePacket(IAsyncResult result, out SocketError error)
        {
            var bytes = Socket.EndReceive(result, out error);

            if (bytes == 0)
                return 0;

            LastReceivedUtc = DateTime.UtcNow;
            _packetOffset += bytes;

            if (PacketComplete) _headerOffset = 0;

            return bytes;
        }

        public NetworkPacket ToPacket()
        {
            return PacketComplete ? new NetworkPacket(_packet, _packetLength) : null;
        }

        private static void ConfigureTcpSocket(Socket tcpSocket)
        {
            tcpSocket.NoDelay = true;
            tcpSocket.ReceiveBufferSize = 65534;
            tcpSocket.SendBufferSize = 65534;
        }
    }
}