#region

using System;
using System.Linq;
using Darkages.Types;

#endregion

namespace Darkages.Network.Game.Components
{
    public class MonolithComponent : GameServerComponent
    {
        /// <summary>
        /// 젠 간격이 맞춰져 있는 맵 넓이. 하데스가 싣는 사냥방이 20x20 이고, 이보다 넓은 맵은 그 배수만큼
        /// 자주 세워야 같은 밀도가 된다. 이보다 좁은 맵은 배수가 1 이라 정의가 적은 간격 그대로다.
        /// </summary>
        private const int RoomTiles = 20 * 20;

        private readonly GameServerTimer _timer;

        public MonolithComponent(GameServer server)
            : base(server)
        {
            _timer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.GlobalSpawnTimer));
        }

        public void CreateFromTemplate(MonsterTemplate template, Area map)
        {
            var newObj = Monster.Create(template, map);

            if (newObj != null)
                AddObject(newObj);
        }

        protected internal override void Update(TimeSpan elapsedTime)
        {
            if (_timer.Update(elapsedTime))
                Lorule.Update(ManageSpawns);
        }

        private void ManageSpawns()
        {
            var templates = ServerContext.GlobalMonsterTemplateCache;
            if (templates.Count == 0)
                return;

            foreach (var map in ServerContext.GlobalMapCache.Values)
            {
                // 크기를 모르는 맵은 이 맵만 건너뛴다. 전에는 return 이라, 그런 맵이 하나 섞이면
                // 그 뒤에 오는 모든 맵의 젠이 통째로 멈췄다 — 조용히, 빈 사냥터로.
                if (map == null || map.Rows == 0 || map.Cols == 0)
                    continue;

                // 정의가 적은 SpawnRate 는 방 한 칸을 염두에 둔 값이다 — 하데스가 싣는 사냥방이 20x20 이고
                // 거기서는 잘 맞는다. 우드랜드1-1 은 3,600칸인데 사람이 보는 것은 열두 칸 안이라, 정의
                // 다섯이 50초에 하나씩 세우면 다 차기까지 8분이 넘고 그 전에 사람이 지나가 버린다.
                // 그래서 넓이에 비례해 **간격만** 줄인다. 최대 마릿수(SpawnMax)는 정의가 적은 그대로다 —
                // 그쪽까지 넓이로 곱하면 우드랜드1-1 에 450마리가 서서 밀도가 원작과 달라진다.
                var spread = Math.Max(1, map.Rows * map.Cols / RoomTiles);

                var temps = templates.Where(i => i.AreaID == map.Id);

                foreach (var template in temps)
                {
                    var count = GetObjects<Monster>(map, i =>
                        i.Template != null && i.Template.Name == template.Name
                                           && i.Template.AreaID == map.Id).Count();

                    if (!template.ReadyToSpawn(template.SpawnRate / (double) spread))
                        continue;

                    if (count < template.SpawnMax)
                        if (count < map.Rows * map.Cols / 6)
                            CreateFromTemplate(template, map);
                }
            }
        }
    }
}