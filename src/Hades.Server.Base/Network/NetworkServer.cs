#region

using Darkages.Common;
using Darkages.Network.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Darkages.Network.ClientFormats;

#endregion

namespace Darkages.Network
{
    public abstract partial class NetworkServer<TClient> : ObjectManager
        where TClient : NetworkClient, new()
    {
        public Dictionary<int, TClient> ConnectedClients;

        // The dictionary is mutated from socket completion callbacks on several threads at once.
        private readonly object _clientsLock = new object();
        private readonly MethodInfo[] _handlers;
        private Socket _listener;
        private bool _listening;
        private System.Threading.Timer _stalledFrameSweep;

        private static readonly TimeSpan SweepInterval = TimeSpan.FromSeconds(1);
        private const double DefaultIncompleteFrameTimeoutSeconds = 15;

        protected NetworkServer(int capacity = 2048)
        {
            var type = typeof(NetworkServer<TClient>);

            Address = ServerContext.IpAddress;
            ConnectedClients = new Dictionary<int, TClient>(capacity);

            _handlers = new MethodInfo[256];

            for (var i = 0; i < _handlers.Length; i++)
                _handlers[i] = type.GetMethod($"Format{i:X2}Handler", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public IPAddress Address { get; }

        /// <summary>
        /// A snapshot of the connected clients. Callers used to lock this property's result, which is a new
        /// list on every access and therefore guarded nothing; the snapshot is taken under the real lock now.
        /// </summary>
        public List<TClient> Clients
        {
            get
            {
                lock (_clientsLock)
                    return ConnectedClients.Values.ToList();
            }
        }

        public virtual void Abort()
        {
            _listening = false;

            if (_stalledFrameSweep != null)
            {
                _stalledFrameSweep.Dispose();
                _stalledFrameSweep = null;
            }

            if (_listener != null)
            {
                _listener.Close();
                _listener = null;
            }

            // Clients is already a snapshot taken under the lock, so it is walked without holding it:
            // ClientDisconnected takes the same lock on its way to RemoveClient.
            foreach (var client in Clients.Where(client => client != null))
                ClientDisconnected(client);
        }

        /// <summary>
        /// Drops connections that began a frame and stopped. A half sent frame cannot be told from a slow
        /// sender by its shape, so a time limit is the only thing that clears it.
        /// </summary>
        private void DisconnectStalledClients()
        {
            try
            {
                var limit = TimeSpan.FromSeconds(
                    ServerContext.Config?.IncompleteFrameTimeoutSeconds ?? DefaultIncompleteFrameTimeoutSeconds);

                foreach (var client in Clients)
                {
                    if (client?.State == null)
                        continue;

                    if (client.State.HasPartialFrame && DateTime.UtcNow - client.State.LastReceivedUtc > limit)
                        ClientDisconnected(client);
                }
            }
            catch (Exception e)
            {
                ServerContext.Error(e);
            }
        }

        public virtual bool AddClient(TClient client)
        {
            lock (_clientsLock)
            {
                if (!ConnectedClients.ContainsKey(client.Serial))
                    ConnectedClients.Add(client.Serial, client);
            }

            return true;
        }

        public virtual void ClientConnected(TClient client)
        {
            if (ServerContext.Game == null)
                return;

            ServerContext.Logger($"Connection From {0} Established. {client.Socket.RemoteEndPoint}");
        }

        public virtual void ClientDataReceived(TClient client, NetworkPacket packet)
        {
            var format = NetworkFormatManager.GetClientFormat(packet.Command);

            if (format == null)
                return;

            try
            {
                if (!Clients.Exists(i => i.Serial == client.Serial))
                    return;

                if (client.MapOpen && !(format is ClientFormat3F))
                    return;

                client.Read(packet, format);
                client.LastMessageFromClient = DateTime.UtcNow;

                if (_handlers[format.Command] != null)
                    _handlers[format.Command].Invoke(this,
                        new object[]
                        {
                            client,
                            format
                        });
            }
            catch (Exception e)
            {
                ServerContext.Error(e);
            }
        }

        public virtual void ClientDisconnected(TClient client)
        {
            if (client == null)
                return;

            if (client.Socket.Connected)
                client.Socket.Disconnect(false);

            RemoveClient(client);
        }

        public void RemoveClient(TClient client)
        {
            lock (_clientsLock)
            {
                if (client != null && ConnectedClients != null && ConnectedClients.ContainsKey(client.Serial))
                    ConnectedClients.Remove(client.Serial);
            }
        }

        public virtual void Start(int port)
        {
            if (_listening)
                return;

            _listening = true;
            _stalledFrameSweep = new System.Threading.Timer(
                _ => DisconnectStalledClients(), null, SweepInterval, SweepInterval);
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, port));
            _listener.Listen(ServerContext.Config?.ConnectionCapacity ?? 1000);
            _listener.BeginAccept(EndConnectClient, _listener);
        }

        private void EndConnectClient(IAsyncResult result)
        {
            try
            {
                if (_listener == null || !_listening) return;

                var client = new TClient
                {
                    State = new NetworkSocket(_listener.EndAccept(result))
                };

                if (client.Socket.Connected)
                {
                    lock (Generator.Random)
                    {
                        client.Serial = Generator.GenerateNumber();
                    }

                    if (AddClient(client))
                    {
                        ClientConnected(client);

                        client.State.BeginReceiveHeader(EndReceiveHeader, out var error, client);

                        if (error != SocketError.IOPending && error != SocketError.Success)
                            ClientDisconnected(client);
                    }
                    else
                    {
                        ServerContext.Logger("Client could not be added.");
                        ClientDisconnected(client);
                    }
                }

                if (_listening)
                    _listener.BeginAccept(EndConnectClient, null);
            }
            catch (Exception e)
            {
                ServerContext.Error(e);
            }
        }

        private void EndReceiveHeader(IAsyncResult result)
        {
            try
            {
                if (!(result.AsyncState is TClient client))
                    return;

                var bytes = client.State.EndReceiveHeader(result, out var error);

                if (bytes == 0 ||
                    error != SocketError.Success)
                {
                    ClientDisconnected(client);
                    return;
                }

                if (client.State.HeaderComplete)
                    client.State.BeginReceivePacket(EndReceivePacket, out error, client);
                else
                    client.State.BeginReceiveHeader(EndReceiveHeader, out error, client);
            }
            catch (Exception e)
            {
                ServerContext.Error(e);
            }
        }

        private void EndReceivePacket(IAsyncResult result)
        {
            try
            {
                if (result.AsyncState is TClient client)
                {
                    var bytes = client.State.EndReceivePacket(result, out var error);

                    if (bytes == 0 ||
                        error != SocketError.Success)
                    {
                        ClientDisconnected(client);
                        return;
                    }

                    if (client.State.PacketComplete)
                    {
                        ClientDataReceived(client, client.State.ToPacket());
                        client.State.BeginReceiveHeader(EndReceiveHeader, out error, client);
                    }
                    else
                    {
                        client.State.BeginReceivePacket(EndReceivePacket, out error, client);
                    }
                }
            }
            catch (Exception e)
            {
                ServerContext.Error(e);
            }
        }
    }
}