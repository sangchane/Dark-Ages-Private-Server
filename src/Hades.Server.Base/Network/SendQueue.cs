using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace Darkages.Network
{
    /// <summary>
    /// One writer per connection. Packets are handed over and written by a thread belonging to that
    /// connection alone, so a client that has stopped reading holds up nobody but itself.
    /// </summary>
    /// <remarks>
    /// Two things a socket does that the obvious code does not expect. Send may take fewer bytes than it is
    /// offered, and the rest have to be offered again or the packet arrives cut in half — which desynchronises
    /// the stream, because the far side is reading a length and then that many bytes. And a socket whose far
    /// side has stopped reading eventually stops accepting anything at all, so a sender that simply waits
    /// stops serving everyone else.
    /// </remarks>
    public sealed class SendQueue : IDisposable
    {
        private readonly BlockingCollection<byte[]> _pending;
        private readonly Socket _socket;
        private readonly Action _failed;
        private readonly Thread _writer;

        private int _disposed;

        /// <param name="socket">The connection to write to.</param>
        /// <param name="capacity">
        /// How many packets may wait. Past this the client is not keeping up and the connection is given up
        /// rather than allowed to grow without bound.
        /// </param>
        /// <param name="failed">Called once when the connection can no longer be written to.</param>
        public SendQueue(Socket socket, int capacity, Action failed)
        {
            _socket = socket ?? throw new ArgumentNullException(nameof(socket));
            _failed = failed ?? throw new ArgumentNullException(nameof(failed));
            _pending = new BlockingCollection<byte[]>(capacity);

            _writer = new Thread(Drain)
            {
                IsBackground = true,
                Name = "hades-send"
            };

            _writer.Start();
        }

        /// <summary>How many packets are waiting to go out. Zero when the client is keeping up.</summary>
        public int Waiting => _pending.Count;

        /// <summary>
        /// Hands a packet over. Returns false when the queue is full or the connection is finished, and the
        /// caller should drop the connection rather than keep offering.
        /// </summary>
        public bool Enqueue(byte[] packet)
        {
            if (packet == null || packet.Length == 0)
            {
                return true;
            }

            if (Volatile.Read(ref _disposed) != 0)
            {
                return false;
            }

            try
            {
                return _pending.TryAdd(packet);
            }
            catch (InvalidOperationException)
            {
                // Completed while we were adding.
                return false;
            }
        }

        private void Drain()
        {
            try
            {
                foreach (var packet in _pending.GetConsumingEnumerable())
                {
                    SendAll(_socket, packet);
                }
            }
            catch (Exception)
            {
                // A socket that cannot be written to is a connection that is over. Whoever owns it decides
                // what to do about that.
                Fail();
            }
        }

        private void Fail()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _pending.CompleteAdding();
            _failed();
        }

        /// <summary>
        /// Writes every byte. Send reports how many it took, and on a busy connection that is fewer than it
        /// was given, so what is left has to be offered again.
        /// </summary>
        public static void SendAll(Socket socket, byte[] buffer)
        {
            var sent = 0;

            while (sent < buffer.Length)
            {
                var wrote = socket.Send(buffer, sent, buffer.Length - sent, SocketFlags.None);

                if (wrote <= 0)
                {
                    throw new SocketException((int) SocketError.ConnectionReset);
                }

                sent += wrote;
            }
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0)
            {
                return;
            }

            _pending.CompleteAdding();
        }
    }
}
