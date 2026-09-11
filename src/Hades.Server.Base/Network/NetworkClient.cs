#region

using Darkages.Network.ClientFormats;
using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using Darkages.Security;
using System;
using System.Globalization;
using System.Net.Sockets;
using System.Threading;
using Newtonsoft.Json;

#endregion

namespace Darkages.Network
{
    public abstract partial class NetworkClient : ObjectManager
    {
        private const int DefaultSendQueueDepth = 512;

        private readonly object _outboundLock = new object();

        private SendQueue _outbound;

        protected NetworkClient()
        {
            Reader = new NetworkPacketReader();
            Writer = new NetworkPacketWriter();
            Encryption = new SecurityProvider();
        }

        public SecurityProvider Encryption { get; set; }
        public byte Ordinal { get; set; }
        public NetworkPacketReader Reader { get; set; }
        public int Serial { get; set; }
        public NetworkPacketWriter Writer { get; set; }
        public Socket Socket => State.Socket;
        public bool MapOpen { get; set; }
        internal NetworkSocket State { get; set; }
        public DateTime LastMessageFromClient { get; set; }

        public void FlushAndSend(NetworkFormat format)
        {
            if (!Socket.Connected)
                return;

            byte[] buffer;

            lock (Writer)
            {
                Writer.Position = 0x0;
                Writer.Write(format.Command);

                if (format.Secured)
                    Writer.Write(Ordinal++);

                format.Serialize(Writer);

                var packet = Writer.ToPacket();
                if (packet == null)
                    return;

                if (format.Secured)
                    Encryption.Transform(packet);

                buffer = packet.ToArray();
            }

            Post(buffer);
        }

        /// <summary>
        /// Hands a packet to this connection's writer. Every path out goes through here, so two of them can
        /// no longer interleave their bytes in the middle of one packet.
        /// </summary>
        private void Post(byte[] buffer)
        {
            if (buffer == null || buffer.Length == 0)
                return;

            var outbound = Outbound;

            if (outbound == null || !outbound.Enqueue(buffer))
                GiveUp();
        }

        /// <summary>Made on the first send, because a client exists before its socket does.</summary>
        private SendQueue Outbound
        {
            get
            {
                lock (_outboundLock)
                {
                    if (_outbound != null)
                        return _outbound;

                    var socket = State?.Socket;

                    if (socket == null)
                        return null;

                    _outbound = new SendQueue(
                        socket,
                        ServerContext.Config?.SendQueueDepth ?? DefaultSendQueueDepth,
                        GiveUp);

                    return _outbound;
                }
            }
        }

        /// <summary>
        /// Closes the socket, which is how a connection ends from here: the read side notices and the
        /// server's own disconnect path runs.
        /// </summary>
        private void GiveUp()
        {
            try
            {
                State?.Socket?.Close();
            }
            catch (Exception)
            {
                // Already gone.
            }
        }

        /// <summary>Stops writing to this connection.</summary>
        public void CloseOutbound()
        {
            lock (_outboundLock)
            {
                _outbound?.Dispose();
                _outbound = null;
            }
        }

        public void Read(NetworkPacket packet, NetworkFormat format)
        {
            if (packet == null)
                return;

            lock (Reader)
            {
                if (format.Secured)
                {
                    Encryption.Transform(packet);

                    if (format.Command == 0x39 || format.Command == 0x3A)
                    {
                        TransFormDialog(packet);
                        Reader.Position = 0x6;
                    }
                    else
                    {
                        Reader.Position = 0x0;
                    }
                }
                else
                {
                    Reader.Position = -0x1;
                }

                Reader.Packet = packet;
                format.Serialize(Reader);
                Reader.Position = -0x1;
            }
        }

        public void Send(NetworkFormat format)
        {
            FlushAndSend(format);
        }

        public void Send(NetworkPacketWriter data)
        {
            if (!Socket.Connected)
                return;

            byte[] array;

            // Everything guarded here belongs to this connection — the cipher is this client's. Taking the
            // process-wide lock made every send on the server queue behind every other, so one connection
            // asking for thousands of refreshes starved the rest. Writer is the per-connection mutex the
            // Send(NetworkFormat) path above already uses.
            lock (Writer)
            {
                var packet = data.ToPacket();
                Encryption.Transform(packet);

                array = packet.ToArray();
            }

            Post(array);
        }

        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture,
                    "The binary key cannot have an odd number of digits: {0}",
                    hexString));
            }

            var data = new byte[hexString.Length / 2];
            for (var index = 0; index < data.Length; index++)
            {
                var byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }

        public void Send(string rawData)
        {
            Send(ConvertHexStringToByteArray(rawData));
        }

        public void Send(byte[] data)
        {
            if (!Socket.Connected)
                return;

            byte[] array;

            // This path writes into Writer, so it has to hold the same lock the other Writer users hold.
            // Guarding it with the process-wide lock instead left it racing the Send(NetworkFormat) path,
            // which locks Writer — two sends on one connection could interleave in the same buffer.
            lock (Writer)
            {
                Writer.Position = 0x0;
                Writer.Write(data);

                var packet = Writer.ToPacket();
                if (packet == null)
                    return;

                Encryption.Transform(packet);

                array = packet.ToArray();
            }

            Post(array);
        }

        public void SendMessageBox(byte code, string text)
        {
            Send(new ServerFormat02(code, text));
        }

        private static byte P(NetworkPacket value)
        {
            return (byte)(value.Data[0x1] ^ (byte)(value.Data[0x0] - 0x2D));
        }

        private static void TransFormDialog(NetworkPacket value)
        {
            if (value.Data.Length > 0x2) value.Data[0x2] ^= (byte)(P(value) + 0x73);
            if (value.Data.Length > 0x3) value.Data[0x3] ^= (byte)(P(value) + 0x73);
            if (value.Data.Length > 0x4) value.Data[0x4] ^= (byte)(P(value) + 0x28);
            if (value.Data.Length > 0x5) value.Data[0x5] ^= (byte)(P(value) + 0x29);

            for (var i = value.Data.Length - 0x6 - 0x1; i >= 0x0; i--)
            {
                var index = i + 0x6;

                if (index >= 0x0 && value.Data.Length > index)
                    value.Data[index] ^= (byte)(((byte)(P(value) + 0x28) + i + 0x2) % 0x100);
            }
        }

    }
}