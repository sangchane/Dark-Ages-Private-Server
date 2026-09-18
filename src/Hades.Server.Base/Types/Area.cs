#region

using Darkages.Storage;
using Darkages.Types;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darkages.Scripting;
using Newtonsoft.Json;

#endregion

namespace Darkages
{
    public class Area : Map
    {
        [JsonIgnore] private static readonly byte[] Sotp = File.ReadAllBytes(ServerContext.StoragePath + "/static/sotp.dat");
        [JsonIgnore] public byte[] Data;
        [JsonIgnore] public ushort Hash;
        [JsonIgnore] public bool Ready;
        [JsonIgnore] public TileGrid[,] ObjectGrid { get; set; }
        [JsonIgnore] public TileContent[,] Tile { get; set; }
        [JsonIgnore] public Dictionary<string, AreaScript> Scripts { get; set; } = new Dictionary<string, AreaScript>();

        public string FilePath { get; set; }

        // ── 5.99 map_create 의 개인 사본(Systems/Instances) ─────────────────────────────
        /// <summary>클라이언트에게 알리는 맵 번호. 사본은 원래 맵 번호를 알려 그 맵 파일·그림을 쓰게 한다. 0 이면 Id.</summary>
        [JsonIgnore] public int ClientNumber { get; set; }

        /// <summary>클라이언트에게 알리는 이름. 사본의 Name 은 캐릭터 이름이 붙은 고유 이름이라 5.99 가 보여 주는 이름을 따로 둔다.</summary>
        [JsonIgnore] public string ClientName { get; set; }

        /// <summary>5.99 `get_map_stage` · `get_map_sub_stage` — 던전 스크립트(Dungeon__Script)가 어느 던전 몇 번째 방인지 가린다.</summary>
        [JsonIgnore] public int Stage { get; set; }
        [JsonIgnore] public int SubStage { get; set; }

        /// <summary>사본을 만든 때와 그 안에서 잡힌 괴물 수 — 5.99 `get_clear_time` · `get_kill_mob`.</summary>
        [JsonIgnore] public DateTime CreatedAt { get; set; }
        [JsonIgnore] public int Kills;

        public int NumberOfAislings() =>
            GetObjects<Aisling>(this, n => n?.Map != null && n.Map.Ready && n.CurrentMapId == Id).Count();

        [JsonIgnore]
        public string ActiveMap => NumberOfAislings() > 0 ? $"{Name}({NumberOfAislings()})" : null;

        public byte[] GetRowData(int row)
        {
            var buffer = new byte[Cols * 6];
            var bPos = 0;
            var dPos = row * Cols * 6;

            lock (ServerContext.SyncLock)
            {
                for (var i = 0; i < Cols; i++, bPos += 6, dPos += 6)
                {
                    buffer[bPos + 0] = Data[dPos + 1];

                    buffer[bPos + 1] = Data[dPos + 0];

                    buffer[bPos + 2] = Data[dPos + 3];

                    buffer[bPos + 3] = Data[dPos + 2];

                    buffer[bPos + 4] = Data[dPos + 5];

                    buffer[bPos + 5] = Data[dPos + 4];
                }
            }

            return buffer;
        }

        public bool IsWall(int x, int y)
        {
            if (x < 0 || x >= Cols) return true;

            if (y < 0 || y >= Rows) return true;

            var isWall = Tile[x, y] == TileContent.Wall;
            return isWall;
        }

        /// <summary>
        /// 워프·순간이동이 사람을 내려놓을 자리. 목적지는 고정된 한 칸이라 괴물이 이미 서 있을 수 있는데,
        /// 겹쳐 서면 그 괴물과는 싸울 수가 없다 — 평타는 앞 칸만 훑어(<c>Sprite.GetInfront</c>) 발밑에는 닿지
        /// 않는다. 그래서 한 자리에서 20분을 허공만 친 일이 있었다(2026-09-18).
        /// 차 있으면 둘레로 세 칸까지 넓혀 가며 빈 칸을 찾고, 그래도 없으면 원래 자리에 내려놓는다.
        /// </summary>
        public Position FreeSpotNear(Position wanted)
        {
            if (wanted == null || IsFreeSpot(wanted.X, wanted.Y))
                return wanted;

            for (var ring = 1; ring <= 3; ring++)
                for (var x = wanted.X - ring; x <= wanted.X + ring; x++)
                for (var y = wanted.Y - ring; y <= wanted.Y + ring; y++)
                {
                    if (Math.Max(Math.Abs(x - wanted.X), Math.Abs(y - wanted.Y)) != ring)
                        continue;

                    if (IsFreeSpot(x, y))
                        return new Position(x, y);
                }

            return wanted;
        }

        /// <summary>Whether somebody can be put down here — inside the map, not a wall, and nobody standing on it.</summary>
        private bool IsFreeSpot(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Cols || y >= Rows)
                return false;

            if (IsWall(x, y))
                return false;

            if (ObjectGrid == null || ObjectGrid[x, y] == null)
                return false;

            return !ObjectGrid[x, y].Sprites.Any(one => one is Monster || one is Mundane);
        }

        public bool OnLoaded()
        {
            var delete = false;
            lock (ServerContext.SyncLock)
            {
                Tile = new TileContent[Cols, Rows];
                ObjectGrid = new TileGrid[Cols, Rows];

                var stream = new MemoryStream(Data);
                var reader = new BinaryReader(stream);

                try
                {

                    reader.BaseStream.Seek(0, SeekOrigin.Begin);

                    for (var y = 0; y < Rows; y++)
                    {
                        for (var x = 0; x < Cols; x++)
                        {
                            ObjectGrid[x, y] = new TileGrid(this, x, y);

                            reader.BaseStream.Seek(2, SeekOrigin.Current);

                            if (reader.BaseStream.Position < reader.BaseStream.Length)
                            {
                                var a = reader.ReadInt16();
                                var b = reader.ReadInt16();

                                if (ParseMapWalls(a, b))
                                    Tile[x, y] = TileContent.Wall;
                                else
                                    Tile[x, y] = TileContent.None;
                            }
                            else
                            {
                                Tile[x, y] = TileContent.Wall;
                            }
                        }
                    }

                    foreach (var block in Blocks)
                    {
                        Tile[block.X, block.Y] = TileContent.Wall;
                    }

                    Ready = true;
                }
                catch (Exception ex)
                {
                    ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                    ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);

                    //Ignore
                    delete = true;
                }
                finally
                {
                    reader.Close();
                    stream.Close();
                }

                if (!delete)
                    return true;

            }

            return Ready;
        }

        public bool ParseMapWalls(short lWall, short rWall)
        {
            if (lWall == 0 && rWall == 0)
                return false;

            if (lWall == 0)
                return Sotp[rWall - 1] == 0x0F;

            if (rWall == 0)
                return Sotp[lWall - 1] == 0x0F;

            var left = Sotp[lWall - 1];
            var right = Sotp[rWall - 1];

            return left == 0x0F || right == 0x0F;
        }

        public void Update(in TimeSpan elapsedTime)
        {
            lock (ServerContext.SyncLock)
            {
                if (Scripts != null)
                    foreach (var script in Scripts.Values)
                        script.Update(elapsedTime);

            }

            try
            {
                UpdateAreaObjects(elapsedTime);
            }
            catch
            {
                 // Ignore
            }
        }

        public void UpdateAreaObjects(TimeSpan elapsedTime)
        {
            void UpdateKillCounters(Monster monster)
            {
                if (monster.Target == null || !(monster.Target is Aisling aisling))
                    return;

                if (!aisling.MonsterKillCounters.ContainsKey(monster.Template.BaseName))
                {
                    aisling.MonsterKillCounters[monster.Template.BaseName] =
                        new KillRecord
                        {
                            TotalKills = 1,
                            MonsterLevel = monster.Template.Level,
                            TimeKilled = DateTime.UtcNow
                        };
                }
                else
                {
                    aisling.MonsterKillCounters[monster.Template.BaseName].TotalKills++;
                    aisling.MonsterKillCounters[monster.Template.BaseName].TimeKilled = DateTime.UtcNow;
                    aisling.MonsterKillCounters[monster.Template.BaseName].MonsterLevel = monster.Template.Level;
                }
            }

            var objectCache = GetObjects(this, sprite => sprite.AislingsNearby().Any(), Get.All);

            lock (ServerContext.SyncLock)
            {
                foreach (var obj in objectCache)
                {
                    if (obj != null)
                    {
                        switch (obj)
                        {
                            case Monster monster when monster.Map == null || monster.Scripts == null:
                                continue;
                            case Monster monster:
                                {
                                    if (obj.CurrentHp <= 0x0 && obj.Target != null && !monster.Skulled)
                                    {
                                        foreach (var script in monster.Scripts.Values.Where(
                                            script => obj.Target?.Client != null))
                                        {
                                            script?.OnDeath(obj.Target.Client);
                                        }

                                        UpdateKillCounters(monster);
                                        Kills++;


                                        monster.Skulled = true;
                                    }

                                    if (monster.Scripts != null)
                                    {
                                        foreach (var script in monster.Scripts.Values)
                                            script?.Update(elapsedTime);
                                    }

                                    if (obj.TrapsAreNearby())
                                    {
                                        var nextTrap = Trap.Traps.Select(i => i.Value)
                                            .FirstOrDefault(i => i.Location.X == obj.X && i.Location.Y == obj.Y);

                                        if (nextTrap != null)
                                            Trap.Activate(nextTrap, obj);
                                    }

                                    monster.UpdateBuffs(elapsedTime);
                                    monster.UpdateDebuffs(elapsedTime);
                                    break;
                                }
                            case Item item:
                                {
                                    var stale = !((DateTime.UtcNow - item.AbandonedDate).TotalMinutes > 3);

                                    if (item.Cursed && stale)
                                    {
                                        item.AuthenticatedAislings = null;
                                        item.Cursed = false;
                                    }

                                    break;
                                }
                            case Mundane mundane:
                                {
                                    if (mundane.CurrentHp <= 0)
                                        mundane.CurrentHp = mundane.Template.MaximumHp;

                                    mundane.UpdateBuffs(elapsedTime);
                                    mundane.UpdateDebuffs(elapsedTime);
                                    mundane.Update(elapsedTime);
                                    break;
                                }
                        }

                        obj.LastUpdated = DateTime.UtcNow;
                    }
                }
            }
        }

        public void AddBlock(Position position)
        {
            lock (ServerContext.SyncLock)
            {
                Blocks?.Add(position);
            }

            StorageManager.AreaBucket.Save(this);
        }
    }
}