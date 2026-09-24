#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Darkages.Network.Game.Components;
using Darkages.Network.Object;
using Darkages.Server.Network.Game.Components;
using Darkages.Server.Network.WS;
using Darkages.Types;

#endregion

namespace Darkages.Network.Game
{
    public partial class GameServer
    {
        public ObjectService ObjectFactory = new ObjectService();
        public Dictionary<Type, GameServerComponent> ServerComponents;

        private DateTime _previousGameTime;

        public GameServer(int capacity) : base(capacity)
        {
            InitializeGameServer();
        }

        public override void ClientDisconnected(GameClient client)
        {
            if (client == null) throw new ArgumentNullException(nameof(client));

            if (client.Aisling == null)
                return;

            try
            {
                Party.RemovePartyMember(client.Aisling);

                client.Aisling.LastLogged = DateTime.UtcNow;
                client.Aisling.ActiveReactor = null;
                client.Aisling.ActiveSequence = null;
                client.CloseDialog();
                client.DlgSession = null;
                client.MenuInterpter = null;
                client.Aisling.CancelExchange();
                client.Aisling.Remove(true);

                if ((DateTime.UtcNow - client.LastSave).TotalSeconds > 2)
                    client.Save();
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
            }
            finally
            {
                base.ClientDisconnected(client);
            }
        }

        public void InitializeGameServer()
        {
            RegisterServerComponents();
        }

        public override void Start(int port)
        {
            base.Start(port);

            //Start our Object Server Websocket.
            // Was written in here, which is why two servers could not share a machine.
            ObjectServer.Start($"http://localhost:{ServerContext.Config.ObjectServerPort}/");

            try
            {
                ServerContext.Running = true;
                UpdateServer();
            }
            catch (ThreadAbortException ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Running = false;
            }
        }

        /// <summary>
        /// 이만큼 아무 말이 없으면 접속이 죽은 것으로 보고 캐릭터를 뺀다. 심장박동 세 번(PingInterval 10초)을 거른 셈이다 —
        /// 한 번 늦은 답으로는 빼지 않는다.
        /// </summary>
        public static readonly TimeSpan IdleLimit = TimeSpan.FromSeconds(30);

        public void UpdateClients(TimeSpan elapsedTime)
        {
            lock (Clients)
            {
                foreach (var client in Clients.Where(client => client?.Aisling != null))
                {
                    try
                    {
                        // 조용해진 접속은 곧 세계에서 뺀다 — 소켓을 쥔 채 멈춘 앱(iOS 가 뒤로 보낸 것)·끊긴 망은 0 바이트 읽기가
                        // 오지 않아 서버가 모른다. 심장박동(0x3B, PingInterval 10초)에 답이 없으면 그런 것이다. 전에는 120초였고
                        // 월드맵·워프 중인 접속은 이 검사(GameClient.Update)를 아예 건너뛰어 괴물의 과녁으로 남았다.
                        if (DateTime.UtcNow - client.LastMessageFromClient > IdleLimit)
                        {
                            ClientDisconnected(client);
                            continue;
                        }

                        if (client != null && !client.Aisling.LoggedIn)
                        {
                            continue;
                        }

                        //This logic here is used to prevent clients updating state, when during a warp transition.
                        //This prevents any de-syncing from occuring.
                        if (client != null && !client.IsWarping && !client.MapOpen)
                        {
                            Pulse(elapsedTime, client);
                        } 
                        
                        //This prevents any de-syncing from occuring, but allows for Map Transitions and Warping to occur, in PVP maps.
                        //This prevents any de-syncing from occuring,
                        //during pvp warpings that may happen when a client interacts with a dialog during a warp transition.
                        else if (client != null && client.IsWarping &&
                                 client.CanSendLocation && !client.IsRefreshing &&
                                 client.Aisling.CurrentMapId == ServerContext.Config.PVPMap)
                        {
                            client.SendLocation();
                        }
                    }
                    catch (Exception ex)
                    {
                        ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                        ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
                        // ignored
                    }
                }
            }
        }

        private static void Pulse(TimeSpan elapsedTime, IGameClient client)
        {
            if (client?.Aisling != null)
                ObjectComponent.UpdateClientObjects(client.Aisling);

            client?.Update(elapsedTime);
        }

        private static void AutoSave(IGameClient client)
        {
            if ((DateTime.UtcNow - client.LastSave).TotalSeconds > ServerContext.Config.SaveRate) client.Save();
        }

        private void RegisterServerComponents()
        {
            lock (ServerContext.SyncLock)
            {
                ServerComponents = new Dictionary<Type, GameServerComponent>
                {
                    [typeof(DayLightComponent)] = new DayLightComponent(this),
                    [typeof(SaveComponent)] = new SaveComponent(this),
                    [typeof(ObjectComponent)] = new ObjectComponent(this),
                    [typeof(MonolithComponent)] = new MonolithComponent(this),
                    [typeof(MundaneComponent)] = new MundaneComponent(this),
                    [typeof(MessageComponent)] = new MessageComponent(this),
                    [typeof(PingComponent)] = new PingComponent(this)
                };
            }


            ServerContext.Logger($"Server Components Loaded: {ServerComponents.Count}");
        }

        private async void UpdateServer()
        {
            _previousGameTime = DateTime.UtcNow;

            while (ServerContext.Running)
            {
                var gameTime = DateTime.UtcNow - _previousGameTime;

                Lorule.Update(() =>
                {
                    UpdateClients(gameTime);
                    UpdateComponents(gameTime);

                    foreach (var map in ServerContext.GlobalMapCache) map.Value?.Update(gameTime);
                });

                _previousGameTime += gameTime;

                await Task.Delay(8);
            }
        }

        protected void UpdateComponents(TimeSpan elapsedTime)
        {
            try
            {
                var components = ServerComponents.Select(i => i.Value);

                foreach (var component in components)
                    try
                    {
                        component?.Update(elapsedTime);
                    }
                    catch (Exception ex)
                    {
                        ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                        ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
                    }
            }
            catch (Exception e)
            {
                ServerContext.Error(e);
            }
        }
    }
}