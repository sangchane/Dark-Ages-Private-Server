#region

using System;
using System.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.Threading.Tasks;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Templates;
using Darkages.Types;


#endregion

namespace Darkages
{
    public class PortalSession
    {
        public PortalSession()
        {
            IsMapOpen = false;
        }

        public DateTime DateOpened { get; set; }
        public int FieldNumber { get; set; } = 1;
        public bool IsMapOpen { get; set; }

        [JsonIgnore]
        public WorldMapTemplate Template
            => ServerContext.GlobalWorldMapTemplateCache[FieldNumber];

        public void ShowFieldMap(GameClient client)
        {
            if (client.MapOpen)
                return;

            if (ServerContext.GlobalWorldMapTemplateCache.ContainsKey(client.Aisling.World))
            {
                var portal = ServerContext.GlobalWorldMapTemplateCache[client.Aisling.World];

                if (portal.Portals.Any(ports => !ServerContext.GlobalMapCache.ContainsKey(ports.Destination.AreaId)))
                {
                    ServerContext.Logger("No Valid Configured World Map.");
                    return;
                }
            }

            client.Send(new ServerFormat2E(client.Aisling));


            client.Aisling.PortalSession
                = new PortalSession
                {
                    FieldNumber = client.Aisling.World,
                    IsMapOpen = true,
                    DateOpened = DateTime.UtcNow
                };
        }

        public void TransitionToMap(GameClient client, short x = -1, short y = -1, int destinationMap = 0)
        {
            client.LastWarp = DateTime.UtcNow.AddMilliseconds(100);

            if (destinationMap == 0)
            {
                client.Aisling.EnterAbyss();
                ShowFieldMap(client);
            }
            else
            {
                if (!ServerContext.GlobalMapCache.ContainsKey(destinationMap))
                    return;

                // 괴물이 선 칸에 내려놓으면 겹쳐 서서 그 괴물과는 싸울 수가 없다 — 빈 칸을 찾아 놓는다.
                var landing = ServerContext.GlobalMapCache[destinationMap].FreeSpotNear(new Position(
                    x >= 0 ? x : ServerContext.Config.TransitionPointX,
                    y >= 0 ? y : ServerContext.Config.TransitionPointY));

                // 전 맵 목록에서 먼저 뺀다 — 맵 번호를 먼저 바꾸면 LeaveArea 가 새 맵 목록에서 지우려다 못 지워, 전 맵에 남은
                // 캐릭터가 끊긴 뒤에도 "같은 이름의 다른 접속"으로 남았다(2026-09-27 클라우드, 1초에 150번 저장 · 봇이 옛 캐릭터 곁으로).
                client.LeaveArea(true, true);

                client.Aisling.XPos = landing.X;
                client.Aisling.YPos = landing.Y;

                client.Aisling.CurrentMapId = destinationMap;
                client.EnterArea();
            }

            client.Aisling.PortalSession = null;
        }
    }
}