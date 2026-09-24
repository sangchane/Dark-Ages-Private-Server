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

        /// <summary>넓이에 맞춰 늘린 마릿수 중 실제로 세우는 몫. 2026-09-24 사용자 결정으로 30% 줄였다.</summary>
        private const double Thinned = 0.7;

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

                    // 마릿수도 넓이에 맞춘다. 정의가 적은 SpawnMax 는 20x20 방 기준이라, 3,600칸짜리
                    // 사냥터에 그대로 쓰면 스무 칸에 한 마리도 안 선다(사용자, 2026-09-18). 넓이에 비례해
                    // 늘리되 넓이의 제곱근만큼만 — 그대로 곱하면 우드랜드1-1 에 450마리가 선다.
                    // 그렇게 늘린 수에서 다시 30% 를 덜어 낸다 — 아이폰으로 돌아본 사용자가 "많다"고 했다
                    // (2026-09-24, 사용자 결정). 1마리짜리 정의는 반올림으로 1마리가 남는다.
                    // 경험치가 0 인 정의는 사냥감이 아니라 꾸밈이다(노비스마을의 노비스주민1·2 — 5.99 Npc_Spawn 에
                    // 한 명씩 적힌 걸어 다니는 주민). 넓이로 늘리면 죽지 않는 "괴물" 이 마을에 여섯 선다.
                    var most = template.Exp == 0
                        ? template.SpawnMax
                        : (int) Math.Round(template.SpawnMax * Math.Sqrt(spread) * Thinned);

                    if (count < most)
                        if (count < map.Rows * map.Cols / 40)
                            CreateFromTemplate(template, map);
                }
            }
        }
    }
}