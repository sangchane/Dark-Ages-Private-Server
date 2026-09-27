using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Storage;
using Darkages.Types;

namespace Darkages.Systems
{
    /// <summary>
    /// 5.99 `map_create` 의 개인 사본 — 들어갈 때마다 캐릭터(그룹) 전용으로 원래 맵을 한 벌 더 짓는다(포테의숲 오솔길 · 튜토리얼 …).
    /// 사본은 원래 맵 파일을 그대로 쓰고, 클라이언트에게는 원래 맵 번호와 5.99 가 보여 주는 이름을 알린다(<see cref="Area.ClientNumber" />).
    /// </summary>
    /// <remarks>
    /// 맵·워프 목록은 게임 루프가 쉬지 않고 훑는 그냥 사전·목록이다. 대화 스크립트(다른 스레드)가 그 자리를 고치면 훑던 쪽이 "열거 중 변경"
    /// 으로 터지므로, 고친 사본을 만들어 통째로 바꿔 끼운다 — 훑던 쪽은 옛 것을 끝까지 읽는다.
    /// </remarks>
    public static class Instances
    {
        /// <summary>사본 번호는 여기부터. 맵 번호는 패킷에서 2바이트라 65535 를 넘지 못한다.</summary>
        public const int FirstId = 60000;

        private static readonly object Gate = new object();

        /// <summary>
        /// 이름이 같은 사본이 있으면 그것을 돌려주고(다시 들어온 경우 — 스크립트가 곧이어 괴물·물건을 비우고 새로 세운다), 없으면 짓는다.
        /// </summary>
        public static Area Create(Area original, string name, string shownName, int music, int stage, int subStage)
        {
            lock (Gate)
            {
                var existing = ServerContext.GlobalMapCache.Values.FirstOrDefault(map => map.Name == name);
                if (existing != null)
                {
                    existing.ClientName = shownName;
                    existing.Stage = stage;
                    existing.SubStage = subStage;
                    existing.CreatedAt = DateTime.UtcNow;
                    existing.Kills = 0;
                    return existing;
                }

                var id = FirstId;
                while (ServerContext.GlobalMapCache.ContainsKey(id))
                    id++;

                if (id > ushort.MaxValue)
                    return null;

                var area = new Area
                {
                    Id = id,
                    Name = name,
                    Cols = original.Cols,
                    Rows = original.Rows,
                    Flags = original.Flags,
                    Music = music,
                    ContentName = original.ContentName,
                    Blocks = original.Blocks,
                    ClientNumber = original.ClientNumber > 0 ? original.ClientNumber : original.Id,
                    ClientName = shownName,
                    Stage = stage,
                    SubStage = subStage,
                    CreatedAt = DateTime.UtcNow
                };

                if (!AreaStorage.LoadMap(area, original.FilePath))
                    return null;

                ServerContext.Game?.ObjectFactory.AddMap(id);
                ServerContext.GlobalMapCache = new Dictionary<int, Area>(ServerContext.GlobalMapCache) { [id] = area };
                return area;
            }
        }

        /// <summary>5.99 `warp_create` — 같은 칸에서 나가는 워프가 이미 있으면 바꾼다(다시 들어와 다시 짓는 경우).</summary>
        public static void AddWarp(WarpTemplate warp)
        {
            lock (Gate)
            {
                var from = warp.Activations.Select(a => (a.Location.X, a.Location.Y)).ToHashSet();
                var warps = ServerContext.GlobalWarpTemplateCache
                    .Where(w => w.ActivationMapId != warp.ActivationMapId ||
                                !w.Activations.Any(a => from.Contains((a.Location.X, a.Location.Y))))
                    .ToList();
                warps.Add(warp);
                ServerContext.GlobalWarpTemplateCache = warps;
            }
        }

        /// <summary>맵 안의 것을 치운다 — 5.99 `mob_clear` · `item_clear`.</summary>
        public static int Clear<T>(Area area) where T : Sprite
        {
            var service = ServerContext.Game?.ObjectFactory;
            if (service == null || area == null)
                return 0;

            var found = service.QueryAll<T>(area, sprite => sprite.CurrentMapId == area.Id).ToArray();
            foreach (var sprite in found)
            {
                sprite.Remove();
                service.RemoveGameObject(sprite);
            }

            return found.Length;
        }

        /// <summary>
        /// 사본에서 나가는 마지막 사람이 바닥에 남은 물건·금화를 가방에 챙긴다. 원작처럼 자동 줍기가 없어, 보스를 잡자마자 던전 스크립트가
        /// 5초 뒤 내보내면 떨어진 반지가 사본과 함께 사라졌다(2026-09-27 사용자 신고). 남은 사람이 있으면 그 사람 몫으로 둔다 — 누가 잡은
        /// 괴물의 것인지 적는 <see cref="Item.AuthenticatedAislings" /> 는 맵이 곧 지워 버려 믿을 수 없다.
        /// </summary>
        /// <returns>가방·지갑이 차서 못 넣은 것 — 부르는 쪽이 나간 자리 발밑에 내려놓는다.</returns>
        public static List<Sprite> HandOver(Aisling person, Area area)
        {
            var left = new List<Sprite>();
            var service = ServerContext.Game?.ObjectFactory;
            if (service == null || person == null || area == null || area.Id < FirstId)
                return left;

            if (service.QueryAll<Aisling>(area, other => other.CurrentMapId == area.Id && other.Serial != person.Serial).Any())
                return left;

            foreach (var item in service.QueryAll<Item>(area, i => i.CurrentMapId == area.Id).ToArray())
            {
                if (item.Template == null || item.Template.Flags.HasFlag(ItemFlags.Trap))
                    continue;
                if (!item.GiveTo(person))
                    left.Add(item);
                item.Remove();
            }

            foreach (var money in service.QueryAll<Money>(area, m => m.CurrentMapId == area.Id).ToArray())
            {
                if (person.GoldPoints + money.Amount < ServerContext.Config.MaxCarryGold)
                    money.GiveTo(money.Amount, person);
                else
                {
                    left.Add(money);
                    money.Remove();
                }
            }

            return left;
        }

        /// <summary><see cref="HandOver" /> 가 못 넣은 것을 그 사람 발밑에 내려놓는다.</summary>
        public static void DropAtFeet(Aisling person, List<Sprite> left)
        {
            foreach (var sprite in left)
            {
                if (sprite is Item item)
                    item.Release(person, person.Position);
                else if (sprite is Money money)
                    Money.Create(person, money.Amount, person.Position);
            }

            if (left.Count > 0)
                person.Client?.SendMessage(0x02, "가방이 가득 차 던전에 남은 물건을 발밑에 내려놓았습니다.");
        }

        /// <summary>빈 사본을 없앤다 — 그 맵에서 나가는 워프 · 괴물 · 물건 · NPC 까지.</summary>
        public static void Remove(Area area)
        {
            lock (Gate)
            {
                if (area == null || area.Id < FirstId)
                    return;

                ServerContext.GlobalWarpTemplateCache = ServerContext.GlobalWarpTemplateCache
                    .Where(w => w.ActivationMapId != area.Id).ToList();

                Clear<Monster>(area);
                Clear<Item>(area);
                Clear<Money>(area);
                Clear<Mundane>(area);

                var maps = new Dictionary<int, Area>(ServerContext.GlobalMapCache);
                maps.Remove(area.Id);
                ServerContext.GlobalMapCache = maps;
                ServerContext.Game?.ObjectFactory.RemoveMap(area.Id);
            }
        }
    }
}
