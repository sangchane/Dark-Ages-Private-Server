#region

using Darkages.Common;
using Darkages.Network;
using Darkages.Network.Game;
using Darkages.Network.Game.Components;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;



using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

using static Darkages.Types.ElementManager;
// ReSharper disable InconsistentNaming

#endregion

namespace Darkages.Types
{

    public abstract class Sprite : ObjectManager, INotifyPropertyChanged, ISprite
    {
        [JsonIgnore] public bool Abyss;
        [JsonIgnore] public Position LastPosition;
        private readonly Random _rnd = new Random();
        public event PropertyChangedEventHandler PropertyChanged;
        public int X;
        public int Y;

        private static readonly int[][] Directions =
        {
            new[] {+0, -1},
            new[] {+1, +0},
            new[] {+0, +1},
            new[] {-1, +0}
        };

        [JsonIgnore]
        private static int[][] DirectionTable { get; } =
        {
            new[] {-1, +3, -1},
            new[] {+0, -1, +2},
            new[] {-1, +1, -1}
        };

        protected Sprite()
        {
            if (this is Aisling)
                EntityType = TileContent.Aisling;
            if (this is Monster)
                EntityType = TileContent.Monster;
            if (this is Mundane)
                EntityType = TileContent.Mundane;
            if (this is Money)
                EntityType = TileContent.None;
            if (this is Item)
                EntityType = TileContent.None;

            Amplified = 0;
            Target = null;
            Buffs = new ConcurrentDictionary<string, Buff>();
            Debuffs = new ConcurrentDictionary<string, Debuff>();
            LastTargetAcquired = DateTime.UtcNow;
            LastMovementChanged = DateTime.UtcNow;
            LastTurnUpdated = DateTime.UtcNow;
            LastUpdated = DateTime.UtcNow;
            LastPosition = new Position(0, 0);
        }

        #region Properties
        [JsonIgnore] public GameClient Client { get; set; }
        public int Serial { get; set; }
        public int CurrentMapId { get; set; }
        public byte _Str { get; set; }
        [JsonIgnore]
        public byte Str
        {
            get
            {
                var tmp = (byte)(_Str + BonusStr).Clamp(1, byte.MaxValue);
                return tmp > 255 ? (byte)255 : tmp;
            }
        }
        public byte _Int { get; set; }
        [JsonIgnore]
        public byte Int
        {
            get
            {
                var tmp = (byte)(_Int + BonusInt).Clamp(1, byte.MaxValue);
                return tmp > 255 ? (byte)255 : tmp;
            }
        }
        public byte _Wis { get; set; }
        [JsonIgnore]
        public byte Wis
        {
            get
            {
                var tmp = (byte)(_Wis + BonusWis).Clamp(1, byte.MaxValue);
                return tmp > 255 ? (byte)255 : tmp;
            }
        }
        public byte _Con { get; set; }
        [JsonIgnore]
        public byte Con
        {
            get
            {
                var tmp = (byte)(_Con + BonusCon).Clamp(1, byte.MaxValue);
                return tmp > 255 ? (byte)255 : tmp;
            }
        }
        public byte _Dex { get; set; }
        [JsonIgnore]
        public byte Dex
        {
            get
            {
                var tmp = (byte)(_Dex + BonusDex).Clamp(1, byte.MaxValue);
                return tmp > 255 ? (byte)255 : tmp;
            }
        }
        public byte _Dmg { get; set; }
        public byte _Hit { get; set; }
        public byte _Mr { get; set; }
        [JsonIgnore]
        public int Ac
        {
            get
            {
                if (BonusAc < -70)
                    return -70;

                return BonusAc;
            }
        }
        public int _Regen { get; set; }
        public int Amplified { get; set; }
        public Element OffenseElement { get; set; }
        public Element DefenseElement { get; set; }
        [JsonIgnore] public bool EmpoweredAssail { get; set; }
        public ConcurrentDictionary<string, Buff> Buffs { get; }
        public ConcurrentDictionary<string, Debuff> Debuffs { get; }
        public int _MaximumHp { get; set; }
        public int CurrentHp { get; set; }
        public int _MaximumMp { get; set; }
        public int CurrentMp { get; set; }
        [JsonIgnore] public DateTime AbandonedDate { get; set; }
        public bool SpellReflect { get; set; }

        [JsonIgnore] private Sprite _target;

        [JsonIgnore]
        public Sprite Target
        {
            get => _target;
            set => _target = value;
        }
        [JsonIgnore]
        public int XPos
        {
            get => X;
            set
            {
                if (X == value)
                    return;

                X = value;
                NotifyPropertyChanged();
            }
        }
        [JsonIgnore]
        public int YPos
        {
            get => Y;
            set
            {
                if (Y == value)
                    return;

                Y = value;

                NotifyPropertyChanged();
            }
        }
        [JsonIgnore] public int BonusAc { get; set; }
        [JsonIgnore] public int BonusCon { get; set; }
        [JsonIgnore] public int BonusDex { get; set; }
        [JsonIgnore] public byte BonusDmg { get; set; }
        [JsonIgnore] public byte BonusHit { get; set; }
        [JsonIgnore] public int BonusHp { get; set; }
        [JsonIgnore] public int BonusInt { get; set; }
        [JsonIgnore] public int BonusMp { get; set; }
        [JsonIgnore] public byte BonusMr { get; set; }
        [JsonIgnore] public int BonusRegen { get; set; }
        [JsonIgnore] public int BonusStr { get; set; }
        [JsonIgnore] public int BonusWis { get; set; }
        [JsonIgnore] public TileContent EntityType { get; protected set; }
        [JsonIgnore] public int GroupId { get; set; }
        public byte Direction { get; set; }
        public bool Immunity { get; set; }
        [JsonIgnore] private int PendingX { get; set; }
        [JsonIgnore] private int PendingY { get; set; }
        [JsonIgnore] public DateTime LastMenuInvoked { get; set; } = DateTime.UtcNow;
        [JsonIgnore] private DateTime LastMovementChanged { get; set; }
        [JsonIgnore] private DateTime LastTargetAcquired { get; set; }
        private DateTime LastTurnUpdated { get; set; }
        [JsonIgnore] public DateTime LastUpdated { get; set; }
        public PrimaryStat MajorAttribute { get; set; }
        [JsonIgnore] public bool Alive => CurrentHp > 0;
        /// <summary>
        /// 때릴 수 있는 것. **NPC 는 아니다** — 마을 상인·사범을 밤새 때리고 있을 수 있었다(사용자,
        /// 2026-09-18). 원작도 주민은 못 때린다. 괴물과 사람만 남긴다.
        /// </summary>
        [JsonIgnore] public bool Attackable => this is Monster || this is Aisling;
        [JsonIgnore] public bool CanCast => !(IsFrozen || IsSleeping);
        [JsonIgnore] public bool CanMove => !(IsFrozen || IsSleeping || IsParalyzed);
        [JsonIgnore] public byte Dmg => (byte)(_Dmg + BonusDmg).Clamp(0, byte.MaxValue);
        [JsonIgnore] public byte Hit => (byte)(_Hit + BonusHit).Clamp(0, byte.MaxValue);
        [JsonIgnore] public bool IsAited => HasBuff("aite");
        [JsonIgnore] public bool IsBleeding => HasDebuff("bleeding");
        [JsonIgnore] public bool IsBlind => HasDebuff("blind");
        [JsonIgnore] public bool IsConfused => HasDebuff("confused");
        [JsonIgnore] public bool IsCursed => HasDebuff(i => i.Name.ToLower().Contains("cradh"));
        [JsonIgnore] public bool IsFrozen => HasDebuff("frozen");
        [JsonIgnore] public bool IsParalyzed => HasDebuff("paralyze") || HasDebuff(i => i.Name.ToLower().Contains("beag suain"));
        [JsonIgnore] public bool IsPoisoned => HasDebuff(i => i.Name.ToLower().Contains("puinsein"));
        [JsonIgnore] public bool IsSleeping => HasDebuff("sleep");
        [JsonIgnore]
        public Area Map => ServerContext.GlobalMapCache.ContainsKey(CurrentMapId)
            ? ServerContext.GlobalMapCache[CurrentMapId]
            : null;
        [JsonIgnore] public int MaximumHp => _MaximumHp + BonusHp;
        [JsonIgnore] public int MaximumMp => _MaximumMp + BonusMp;
        [JsonIgnore] public byte Mr => (byte)(_Mr + BonusMr).Clamp(0, 70);
        [JsonIgnore] public Position Position => new Position(XPos, YPos);
        [JsonIgnore] public int Regen => (_Regen + BonusRegen).Clamp(0, 300);
        [JsonIgnore]
        public int Level => EntityType == TileContent.Aisling ? ((Aisling)this).ExpLevel
            : EntityType == TileContent.Monster ? ((Monster)this).Template.Level
            : EntityType == TileContent.Mundane ? ((Mundane)this).Template.Level
            : EntityType == TileContent.Item ? ((Item)this).Template.LevelRequired : 0;


        #endregion

        public static Aisling Aisling(Sprite obj)
        {
            if (obj is Aisling aisling)
                return aisling;

            return null;
        }

        private bool CanBeAttackedHere(Sprite source)
        {
            if (!(source is Aisling) || !(this is Aisling))
                return true;

            if (CurrentMapId <= 0 || !ServerContext.GlobalMapCache.ContainsKey(CurrentMapId))
                return true;

            return ServerContext.GlobalMapCache[CurrentMapId].Flags.HasFlag(MapFlags.PlayerKill);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Identification & Position
        public void Show<T>(Scope op, T format, IEnumerable<Sprite> definer = null) where T : NetworkFormat
        {
            if (Map == null)
                return;

            try
            {
                switch (op)
                {
                    case Scope.Self:
                        Client?.Send(format);
                        break;
                    case Scope.NearbyAislingsExludingSelf:
                        {
                            foreach (var gc in GetObjects<Aisling>(Map, that => that != null && WithinRangeOf(that)))
                                if (gc != null && gc.Serial != Serial)
                                    if (gc.Client != null)
                                        if (format != null)
                                            gc.Client.Send(format);
                            break;
                        }
                    case Scope.NearbyAislings:
                        {
                            foreach (var gc in GetObjects<Aisling>(Map, that => WithinRangeOf(that)))
                            {
                                gc?.Client.Send(format);
                            }

                            break;
                        }
                    case Scope.Clan:
                        {
                            foreach (var gc in GetObjects<Aisling>(null,
                                that => that != null &&
                                        !that.Abyss &&
                                        !string.IsNullOrEmpty(that.Clan) &&
                                        string.Equals(that.Clan, Aisling(this).Clan, StringComparison.CurrentCultureIgnoreCase)))
                            {
                                gc?.Client.Send(format);
                            }

                            break;
                        }
                    case Scope.VeryNearbyAislings:
                        {
                            foreach (var gc in GetObjects<Aisling>(Map, that =>
                                WithinRangeOf(that, ServerContext.Config.VeryNearByProximity)))
                            {
                                gc?.Client.Send(format);
                            }

                            break;
                        }
                    case Scope.AislingsOnSameMap:
                        {
                            foreach (var gc in GetObjects<Aisling>(Map, that => CurrentMapId == that.CurrentMapId))
                            {
                                gc?.Client.Send(format);
                            }

                            break;
                        }
                    case Scope.GroupMembers when !(this is Aisling):
                        return;
                    case Scope.GroupMembers:
                        {
                            foreach (var gc in GetObjects<Aisling>(Map, that => ((Aisling)this).GroupParty.Has(that)))
                            {
                                gc?.Client.Send(format);
                            }

                            break;
                        }
                    case Scope.NearbyGroupMembersExcludingSelf when !(this is Aisling):
                        return;
                    case Scope.NearbyGroupMembersExcludingSelf:
                        {
                            foreach (var gc in GetObjects<Aisling>(Map,
                                that => that.WithinRangeOf(this) && ((Aisling)this).GroupParty.Has(that)))
                            {
                                gc?.Client.Send(format);
                            }

                            break;
                        }
                    case Scope.NearbyGroupMembers when !(this is Aisling):
                        return;
                    case Scope.NearbyGroupMembers:
                        {
                            foreach (var gc in GetObjects<Aisling>(Map,
                                that => that.WithinRangeOf(this) && ((Aisling)this).GroupParty.Has(that)))
                            {
                                gc?.Client.Send(format);
                            }

                            break;
                        }
                    case Scope.DefinedAislings when definer == null:
                        return;
                    case Scope.DefinedAislings:
                        {
                            foreach (var gc in definer)
                            {
                                (gc as Aisling)?.Client.Send(format);
                            }

                            break;
                        }
                    case Scope.All:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(op), op, null);
                }
            }
            catch (Exception ex)
            {
                ServerContext.Logger(ex.Message, Microsoft.Extensions.Logging.LogLevel.Error);
                ServerContext.Logger(ex.StackTrace, Microsoft.Extensions.Logging.LogLevel.Error);
            }
        }

        public void ShowTo(Aisling nearbyAisling)
        {
            if (nearbyAisling == null) return;
            if (this is Aisling aisling)
            {
                nearbyAisling.Show(Scope.Self, new ServerFormat33(aisling));
            }
            else
                nearbyAisling.Show(Scope.Self, new ServerFormat07(new[] { this }));
        }

        public Aisling[] AislingsNearby()
        {
            return GetObjects<Aisling>(Map, i => i != null && i.WithinRangeOf(this)).ToArray();
        }

        private IEnumerable<Sprite> GetInFrontToSide(int tileCount = 1)
        {
            var results = new List<Sprite>();

            switch (Direction)
            {
                case 0:
                    results.AddRange(GetSprites(XPos, YPos - tileCount));
                    results.AddRange(GetSprites(XPos + tileCount, YPos - tileCount));
                    results.AddRange(GetSprites(XPos - tileCount, YPos - tileCount));
                    results.AddRange(GetSprites(XPos + tileCount, YPos));
                    results.AddRange(GetSprites(XPos - tileCount, YPos));
                    break;

                case 1:
                    results.AddRange(GetSprites(XPos + tileCount, YPos));
                    results.AddRange(GetSprites(XPos + tileCount, YPos + tileCount));
                    results.AddRange(GetSprites(XPos + tileCount, YPos - tileCount));
                    results.AddRange(GetSprites(XPos, YPos + tileCount));
                    results.AddRange(GetSprites(XPos, YPos - tileCount));
                    break;

                case 2:
                    results.AddRange(GetSprites(XPos, YPos + tileCount));
                    results.AddRange(GetSprites(XPos + tileCount, YPos + tileCount));
                    results.AddRange(GetSprites(XPos - tileCount, YPos + tileCount));
                    results.AddRange(GetSprites(XPos + tileCount, YPos));
                    results.AddRange(GetSprites(XPos - tileCount, YPos));
                    break;

                case 3:
                    results.AddRange(GetSprites(XPos - tileCount, YPos));
                    results.AddRange(GetSprites(XPos - tileCount, YPos + tileCount));
                    results.AddRange(GetSprites(XPos - tileCount, YPos - tileCount));
                    results.AddRange(GetSprites(XPos, YPos + tileCount));
                    results.AddRange(GetSprites(XPos, YPos - tileCount));
                    break;
            }

            return results;
        }

        private IEnumerable<Sprite> GetInfront(int tileCount = 1)
        {
            var results = new List<Sprite>();

            for (var i = 1; i <= tileCount; i++)
                switch (Direction)
                {
                    case 0:
                        results.AddRange(GetSprites(XPos, YPos - i));
                        break;

                    case 1:
                        results.AddRange(GetSprites(XPos + i, YPos));
                        break;

                    case 2:
                        results.AddRange(GetSprites(XPos, YPos + i));
                        break;

                    case 3:
                        results.AddRange(GetSprites(XPos - i, YPos));
                        break;
                }

            return results;
        }

        public List<Sprite> GetInfront(Sprite sprite, int tileCount = 1)
        {
            return GetInfront(tileCount).Where(i => i != null && i.Serial != sprite.Serial).ToList();
        }

        public List<Sprite> GetInfront(int tileCount = 1, bool intersect = false)
        {
            return GetInfront(tileCount).ToList();
        }

        public List<Sprite> GetInFrontToSide(Sprite sprite, int tileCount = 1)
        {
            return GetInFrontToSide(tileCount).Where(i => i != null && i.Serial != sprite.Serial).ToList();
        }

        public List<Sprite> GetInFrontToSide(int tileCount = 1, bool intersect = false)
        {
            return GetInFrontToSide(tileCount).ToList();
        }

        public Position GetPendingWalkPosition()
        {
            var pendingX = X;
            var pendingY = Y;

            if (Direction == 0)
                pendingY--;

            if (Direction == 1)
                pendingX++;

            if (Direction == 2)
                pendingY++;

            if (Direction == 3)
                pendingX--;

            return new Position(pendingX, pendingY);
        }

        private IEnumerable<Sprite> GetSprites(int x, int y)
        {
            return GetObjects(Map, i => i.XPos == x && i.YPos == y, Get.All);
        }

        public bool WithinRangeOf(Sprite other, bool checkMap = true)
        {
            return other != null && WithinRangeOf(other, ServerContext.Config.WithinRangeProximity, checkMap);
        }

        public bool WithinRangeOf(Sprite other, int distance, bool checkMap = true)
        {
            if (other == null)
                return false;

            if (!checkMap)
                return WithinRangeOf(other.XPos, other.YPos, distance);

            return CurrentMapId == other.CurrentMapId && WithinRangeOf(other.XPos, other.YPos, distance);
        }

        private bool WithinRangeOf(int x, int y, int subjectLength)
        {
            return DistanceFrom(x, y) < subjectLength;
        }

        public bool TrapsAreNearby()
        {
            return Trap.Traps.Select(i => i.Value).Any(i => i.CurrentMapId == CurrentMapId);
        }

        public Monster Monster(Sprite obj)
        {
            if (obj is Monster monster)
                return monster;

            return null;
        }

        public IEnumerable<Monster> MonstersNearby()
        {
            return GetObjects<Monster>(Map, i => i != null && i.WithinRangeOf(this)).ToArray();
        }

        public IEnumerable<Mundane> MundanesNearby()
        {
            return GetObjects<Mundane>(Map, i => i != null && i.WithinRangeOf(this)).ToArray();
        }

        public bool NextTo(int x, int y)
        {
            var xDist = Math.Abs(x - X);
            var yDist = Math.Abs(y - Y);

            return xDist + yDist == 1;
        }

        private int DistanceFrom(int x, int y)
        {
            return Math.Abs(X - x) + Math.Abs(Y - y);
        }

        protected bool Facing(Sprite other, out int direction)
        {
            return Facing(other.XPos, other.YPos, out direction);
        }

        public bool Facing(int x, int y, out int direction)
        {
            var xDist = (x - XPos).Clamp(-1, +1);
            var yDist = (y - YPos).Clamp(-1, +1);

            direction = DirectionTable[xDist + 1][yDist + 1];
            return Direction == direction;
        }

        private bool CanTag(Aisling attackingPlayer, bool force = false)
        {
            var canTag = false;

            if (!(this is Monster monster))
                return false;

            if (monster.TaggedAislings.Any(i => i == attackingPlayer.Serial))
                canTag = true;

            if (monster.TaggedAislings.Count == 0)
                canTag = true;

            var tagstoRemove = new List<int>();
            foreach (var userId in monster.TaggedAislings.Where(i => i != attackingPlayer.Serial))
            {
                var taggedUser = GetObject<Aisling>(Map, i => i.Serial == userId);

                if (taggedUser == null) continue;
                if (taggedUser.WithinRangeOf(this))
                {
                    canTag = attackingPlayer.GroupId == taggedUser.GroupId;
                }
                else
                {
                    tagstoRemove.Add(taggedUser.Serial);
                    canTag = true;
                }
            }

            var lostTags = monster.AislingsNearby().Where(i => monster.TaggedAislings.Contains(i.Serial));

            if (!lostTags.Any()) canTag = true;

            monster.TaggedAislings.RemoveWhere(n => tagstoRemove.Contains(n));

            if (canTag)
            {
                monster.AppendTags(attackingPlayer);

                if (monster.Target == null)
                    monster.Target = attackingPlayer;
            }

            if (force) canTag = false;

            return canTag;
        }
        #endregion

        #region Movement
        public bool Walk()
        {
            void Step(int i, int savedY1)
            {
                var response = new ServerFormat0C
                {
                    Direction = Direction,
                    Serial = Serial,
                    X = (short)i,
                    Y = (short)savedY1
                };

                X = PendingX;
                Y = PendingY;

                Show(Scope.NearbyAislingsExludingSelf, response);
                {
                    LastMovementChanged = DateTime.UtcNow;
                    LastPosition = new Position(i, savedY1);
                }
            }

            //update all objects nearby before we take a step.
            foreach (var obj in AislingsNearby())
                ObjectComponent.UpdateClientObjects(obj);

            var savedX = X;
            var savedY = Y;

            PendingX = X;
            PendingY = Y;

            var allowGhostWalk = false;

            //only gms can ghost walk, and only aislings can be gms.
            if (this is Aisling aisling)
                if (aisling.GameMaster)
                    allowGhostWalk = true;


            if (this is Monster monster && monster.Template != null)
                allowGhostWalk = monster.Template.IgnoreCollision;

            //check position before we take a step.
            if (!allowGhostWalk)
            {
                if (Map?.IsWall(savedX, savedY) ?? false)
                    return false;

                if (!Map?.ObjectGrid[savedX, savedY].IsPassable(this, this is Aisling) ?? false)
                    return false;
            }

            if (Direction == 0)
                PendingY--;
            else if (Direction == 1)
                PendingX++;
            else if (Direction == 2)
                PendingY++;
            else if (Direction == 3)
                PendingX--;

            // 맵 밖은 누구도 나갈 수 없다 — 운영자도. 벽을 지나가는 것과 맵을 벗어나는 것은 다르다:
            // 밖에는 바닥도 괴물도 없어 화면이 검게 남고, 걸어서 돌아오기 전에는 손쓸 길이 없다
            // (2026-09-18, 시험 캐릭터가 -23,42 까지 걸어 나갔다).
            if (Map != null && (PendingX < 0 || PendingY < 0 || PendingX >= Map.Cols || PendingY >= Map.Rows))
                return false;

            //check position after we take a step.
            if (!allowGhostWalk)
            {
                if (Map != null && Map.IsWall(PendingX, PendingY))
                    return false;
            }

            // **남이 선 칸에는 들어가지 않는다 — 벽을 지나가는 것과는 다른 이야기다.** 운영자의 통과걷기도
            // 여기는 못 지난다: 괴물 위에 겹쳐 서면 그 괴물을 평생 못 때린다(평타는 앞 칸만 훑는다).
            // 겹치는 것이 허락되는 때는 젠 뿐이다(사용자, 2026-09-19).
            if (Map != null && !Map.ObjectGrid[PendingX, PendingY].IsPassable(this, this is Aisling))
                return false;

            //commit.
            Step(savedX, savedY);

            //reset our PendingX, PendingY back to our previous step.
            PendingX = savedX;
            PendingY = savedY;

            //update all objects nearby after we take a step.
            foreach (var obj in AislingsNearby())
                ObjectComponent.UpdateClientObjects(obj);

            return true;
        }

        public bool WalkTo(int x, int y)
        {
            return WalkTo(x, y, false);
        }

        protected bool WalkTo(int x, int y, bool ignoreWalls = false)
        {
            var buffer = new byte[2];
            var length = float.PositiveInfinity;
            var offset = 0;

            for (byte i = 0; i < 4; i++)
            {
                var newX = X + Directions[i][0];
                var newY = Y + Directions[i][1];

                if (newX == x &&
                    newY == y)
                    return false;

                if (Map.IsWall(newX, newY))
                    continue;

                if (GetObjects(Map, n => n.Serial == Serial && n.X == newX && n.Y == newY,
                    Get.Monsters | Get.Aislings | Get.Mundanes).Any())
                    continue;

                var xDist = x - newX;
                var yDist = y - newY;
                var tDist = Sqrt(xDist * xDist + yDist * yDist);

                if (length < tDist)
                    continue;

                if (length > tDist)
                {
                    length = tDist;
                    offset = 0;
                }

                if (offset < buffer.Length)
                    buffer[offset] = i;

                offset++;
            }

            if (offset == 0)
                return false;

            lock (Generator.Random)
            {
                var pendingDirection = buffer[Generator.Random.Next(0, offset) % buffer.Length];
                Direction = pendingDirection;

                return Walk();
            }
        }

        public void Wander()
        {
            if (!CanUpdate())
                return;

            var savedDirection = Direction;
            var update = false;

            lock (_rnd)
            {
                Direction = (byte)_rnd.Next(0, 4);

                if (Direction != savedDirection) update = true;
            }

            if (!Walk() && update)
                Show(Scope.NearbyAislings, new ServerFormat11
                {
                    Direction = Direction,
                    Serial = Serial
                });
        }

        public void Turn()
        {
            if (!CanUpdate())
                return;

            Show(Scope.NearbyAislings, new ServerFormat11
            {
                Direction = Direction,
                Serial = Serial
            });

            LastTurnUpdated = DateTime.UtcNow;
        }
        #endregion

        #region Attributes
        private int ComputeDmgFromAc(int dmg)
        {
            var script = ScriptManager.Load<FormulaScript>(ServerContext.Config.ACFormulaScript, this);

            return script?.Aggregate(dmg, (current, s) => s.Value.Calculate(this, current)) ?? dmg;
        }

        public static Element CheckRandomElement(Element element)
        {
            if (element == Element.Random)
                element = Generator.RandomEnumValue<Element>();

            return element;
        }

        public Sprite ApplyBuff(string buffName)
        {
            if (!ServerContext.GlobalBuffCache.ContainsKey(buffName)) return this;
            var buff = Clone<Buff>(ServerContext.GlobalBuffCache[buffName]);

            if (buff == null || string.IsNullOrEmpty(buff.Name))
                return null;

            if (!HasBuff(buff.Name))
                buff.OnApplied(this, buff);

            return this;
        }

        /// <summary>방어를 거친 뒤에 곱하는 배수. <see cref="ApplyDamageAfterArmour" /> 가 한 번의 피해 동안만 바꾼다.</summary>
        [JsonIgnore] private double _afterArmour = 1;

        /// <summary>
        /// 등 뒤에서 친 한 방. 5.99 서버(Novaonline.exe)도 등 뒤 배수를 2 로 쓴다 — 세 군데에 같은 모양으로
        /// 있다(평타→괴물 0x416331 · 평타→사람 0x4168d8 · 여러 대상 무기 공격 0x415f5d, 셋 다 `shl` 한 번).
        /// 다만 <b>그 빌드에서는 한 번도 걸리지 않는다</b>: 앞의 둘은 만든 배수를 읽지 않고 버리고(`[ebp-40]`
        /// 읽는 곳 없음), 셋째는 호출자가 없다. 원작 의도대로 살려 쓰기로 했다(사용자, 2026-09-18).
        /// </summary>
        private const double FromBehind = 2.0;

        /// <summary>
        /// 옆에서 친 한 방. <b>근거 없음</b> — 사용자가 정한 값(2026-09-23)이다. 5.99 실행 파일에는 옆 배수가
        /// 없다: 1.5 짜리 부동소수 상수가 아예 없고, 정수로 만든 ×3/2(0x42442e `imul 3` + `shr`)는 방향이
        /// 아니라 걸린 버프(캐릭터 +0x15E == 1, `sokup_delay` 가 켠다)를 본다.
        /// </summary>
        private const double FromTheSide = 1.5;

        /// <summary>정면에서 친 한 방. 배수 없음.</summary>
        private const double FromInFront = 1.0;

        /// <summary>방향 0~3(북·동·남·서)이 보는 칸. <see cref="GetInfront" /> 의 표와 같다.</summary>
        private static readonly int[] FacingX = {0, +1, 0, -1};

        private static readonly int[] FacingY = {-1, 0, +1, 0};

        /// <summary>마법을 쓰는 동안만 0 보다 크다. 겹쳐 쓸 수 있으므로 센다.</summary>
        [JsonIgnore] private int _casting;

        /// <summary>
        /// 이 사람·괴물이 지금 마법을 쓰는 중인가. 방향 배수는 <b>때리는 것</b>에만 걸고 마법은 그대로 두기로
        /// 했으므로(사용자, 2026-09-23) 마법을 쓰는 동안만 이 표시를 올려 그 한 길에서 갈라낸다.
        /// </summary>
        [JsonIgnore] public bool Casting => _casting > 0;

        /// <summary>
        /// 마법 스크립트를 <see cref="Casting" /> 표시를 올린 채 돌린다. 마법을 거는 곳은 몇 군데뿐이므로
        /// (사람 <c>Aisling.CastSpell</c> · NPC <c>Mundane</c> · 괴물 <c>CommonMonster.CastSpell</c> ·
        /// 애완 <c>CommonPet</c> · API <c>GameClient.CastSpell</c>) 거기서만 감싸면 된다. 기술은 감싸지 않으므로
        /// 새로 만드는 기술도 따로 적을 것 없이 방향 배수를 받는다.
        /// </summary>
        public void CastingSpell(Action cast)
        {
            Interlocked.Increment(ref _casting);
            try
            {
                cast();
            }
            finally
            {
                Interlocked.Decrement(ref _casting);
            }
        }

        /// <summary>
        /// 때린 자리에 따른 배수 — 등 뒤 ×2 · 옆 ×1.5 · 정면 ×1.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <b>자리로 잰다.</b> 맞는 쪽이 보는 방향과, 때린 쪽이 서 있는 자리를 견준다. 두 사람이 보는 방향끼리
        /// 견주는 방식(5.99 가 그렇게 한다 — <c>0x416331</c>)은 때리는 쪽이 대상을 안 보고 치는 기술
        /// (둘레를 치는 선풍각·파천각, 멀리 나가는 기술)에서 틀린다.
        /// </para>
        /// <para>
        /// 대상이 보는 쪽 단위벡터를 F 라 하고 때린 쪽이 대상에서 떨어진 만큼을 (dx, dy) 라 할 때
        /// 앞쪽 = dx·Fx + dy·Fy, 옆쪽 = |dx·Fy − dy·Fx| 이다. 앞쪽이 옆쪽보다 크면 정면, 뒤쪽이 옆쪽보다
        /// 크면 등 뒤, 나머지는 옆이다. 그래서 <b>대각선은 옆</b>(앞쪽 = 옆쪽 = 1)이고, 여러 칸 떨어진
        /// 기술도 같은 잣대로 갈린다(비율만 보므로 거리가 늘어도 답이 안 바뀐다). <b>같은 칸</b>은
        /// (dx, dy) 가 0 이라 방향을 말할 수 없으므로 덤을 주지 않고 정면으로 본다.
        /// </para>
        /// <para>
        /// <b>때리는 쪽이 사람일 때만 걸린다.</b> 5.99 도 그렇다 — 방향을 견주는 자리는 실행 파일을 통틀어
        /// 셋뿐이고(0x415f5d · 0x416331 · 0x4168d8) 셋 다 사람이 휘두르는 길이다. 괴물이 사람을 치는 길
        /// (0x4258a4 → 0x425dc3 → 0x415341)에는 사람 방향 바이트(+0x9E)를 읽는 명령이 하나도 없고 배수
        /// 인자가 늘 1 이다. 그러니 사람이 사람을 칠 때는 걸리고(0x4168d8), 괴물이 사람을 칠 때는 안 걸린다.
        /// </para>
        /// <para>
        /// <b>마법은 빠진다</b>(사용자, 2026-09-23 — <see cref="Casting" /> 참고). 5.99 도 같다: 마법·기술이
        /// 쓰는 내장함수(`damaged` 0x449b3c · `char_damaged*` 0x44e015~0x44e4d5 · `group_damaged*`)는 모두
        /// 배수 인자로 상수 1 을 넘긴다(`6a 01 push 1`).
        /// </para>
        /// </remarks>
        private double BlowFacing(Sprite attacker)
        {
            if (!(attacker is Aisling) || attacker.Serial == Serial || attacker.Casting)
                return FromInFront;

            if (Direction > 3)
                return FromInFront;

            var dx = attacker.XPos - XPos;
            var dy = attacker.YPos - YPos;

            if (dx == 0 && dy == 0)
                return FromInFront;

            var facingX = FacingX[Direction];
            var facingY = FacingY[Direction];

            var front = dx * facingX + dy * facingY;
            var side = Math.Abs(dx * facingY - dy * facingX);

            if (front > side)
                return FromInFront;

            return -front > side ? FromBehind : FromTheSide;
        }

        /// <summary>
        /// 방어를 거친 한 방에 <paramref name="afterArmour" /> 를 더 곱해 넣는다. 5.99 괴물 평타가 이 순서다 —
        /// 굴린 공격력을 사람 방어로 먼저 거르고(Novaonline.exe 0x425d6e → 0x415173) 그 뒤 공격속성으로
        /// ×1.3 한다(0x425dc3 → 0x415cff). 방어 앞에서 곱하면 작은 한 방이 버림에 깎여 5.99 보다 모자란다.
        /// </summary>
        public void ApplyDamageAfterArmour(Sprite source, int dmg, double afterArmour, byte sound = 1)
        {
            _afterArmour = afterArmour;
            try
            {
                ApplyDamage(source, dmg, sound);
            }
            finally
            {
                _afterArmour = 1;
            }
        }

        public void ApplyDamage(Sprite source, int dmg, Element element, byte sound = 1)
        {
            element = CheckRandomElement(element);

            var saved = source.OffenseElement;
            {
                source.OffenseElement = element;
                ApplyDamage(source, dmg, sound);
                source.OffenseElement = saved;
            }
        }

        public void ApplyDamage(Sprite damageDealingSprite, int dmg, byte sound = 1,
            Action<int> dmgcb = null, bool forceTarget = false)
        {

            int ApplyPVPMod()
            {
                if (Map.Flags.HasFlag(MapFlags.PlayerKill))
                    return dmg = (int)(dmg * 0.75);

                return dmg;
            }

            if (!WithinRangeOf(damageDealingSprite))
                return;

            if (!Attackable)
                return;

            if (!CanBeAttackedHere(damageDealingSprite))
                return;

            dmg = ApplyPVPMod();
            dmg = ApplyWeaponBonuses(damageDealingSprite, dmg);

            if (dmg > 0)
                ApplyEquipmentDurability(dmg);

            if (!DamageTarget(damageDealingSprite, ref dmg, sound, dmgcb, forceTarget))
                return;

            {
                if (damageDealingSprite is Aisling aisling)
                    if (aisling.GameMaster)
                        dmg *= 200;
            }

            OnDamaged(damageDealingSprite, dmg);
        }

        public int CompleteDamageApplication(int dmg, byte sound, Action<int> dmgcb, double amplifier)
        {
            if (dmg <= 0)
                dmg = 1;

            if (CurrentHp > MaximumHp)
                CurrentHp = MaximumHp;

            var dmgApplied = (int)Math.Abs(dmg * amplifier);

            CurrentHp -= dmgApplied;

            if (CurrentHp < 0)
                CurrentHp = 0;

            var hpBar = new ServerFormat13
            {
                Serial = Serial,
                Health = (ushort)((double)100 * CurrentHp / MaximumHp),
                Sound = sound
            };

            Show(Scope.VeryNearbyAislings, hpBar);
            {
                dmgcb?.Invoke(dmgApplied);
            }

            return dmgApplied;
        }

        public bool DamageTarget(Sprite damageDealingSprite,
            ref int dmg, byte sound,
            Action<int> dmgcb, bool forced)
        {
            if (this is Monster)
            {
                if (damageDealingSprite is Aisling aisling)
                    if (!CanTag(aisling, forced))
                    {
                        aisling.Client.SendMessage(0x02, ServerContext.Config.CantAttack);
                        return false;
                    }
            }

            if (Immunity)
            {
                var empty = new ServerFormat13
                {
                    Serial = Serial,
                    Health = byte.MaxValue,
                    Sound = sound
                };

                Show(Scope.VeryNearbyAislings, empty);
                return false;
            }

            if (HasDebuff("sleep"))
            {
                if (ServerContext.Config.SleepProcsDoubleDmg)
                {
                    dmg <<= 1;
                }

                RemoveDebuff("sleep");
            }

            if (IsAited && dmg > 5)
                dmg /= ServerContext.Config.AiteDamageReductionMod;

            // 방어를 거친 뒤에 곱하는 것들은 여기 한 줄에 모인다 — 속성 · 괴물 평타 ×1.3 · 때린 자리.
            // 방어 앞에서 곱하면 작은 한 방이 버림에 깎여 1 : 1.5 : 2 가 어긋난다(ApplyDamageAfterArmour 와 같은 이유).
            // 곱셈이므로 이 셋 사이의 순서는 값에 영향이 없다.
            var amplifier = GetElementalModifier(damageDealingSprite) * _afterArmour * BlowFacing(damageDealingSprite);
            {
                dmg = ComputeDmgFromAc(dmg);
                dmg = CompleteDamageApplication(dmg, sound, dmgcb, amplifier);
            }

            return true;
        }

        public Sprite ApplyDebuff(string debuffName)
        {
            if (!ServerContext.GlobalDeBuffCache.ContainsKey(debuffName)) return this;
            var debuff = Clone<Debuff>(ServerContext.GlobalDeBuffCache[debuffName]);
            if (!HasDebuff(debuff.Name))
                debuff.OnApplied(this, debuff);

            return this;
        }

        public void ApplyEquipmentDurability(int dmg)
        {
            if (this is Aisling aisling && aisling.DamageCounter++ % 2 == 0 && dmg > 0)
                aisling.EquipmentManager.DecreaseDurability();
        }

        public int ApplyWeaponBonuses(Sprite source, int dmg)
        {
            if (!(source is Aisling aisling))
                return dmg;

            if (aisling.EquipmentManager != null && (aisling.EquipmentManager.Weapon?.Item == null || aisling.Weapon <= 0))
                return dmg;

            if (aisling.EquipmentManager == null) return dmg;
            var weapon = aisling.EquipmentManager.Weapon.Item;

            lock (Generator.Random)
            {
                dmg += Generator.Random.Next(
                    weapon.Template.DmgMin + aisling.BonusDmg * 1,
                    weapon.Template.DmgMax + aisling.BonusDmg * 5);
            }

            return dmg;
        }

        public double CalculateElementalDamageMod(Element element)
        {
            var script = ScriptManager.Load<ElementFormulaScript>(ServerContext.Config.ElementTableScript, this);

            return script?.Values.Sum(s => s.Calculate(this, element)) ?? 0.0;
        }

        public int GetBaseDamage(Sprite target, MonsterDamageType type)
        {
            var script = ScriptManager.Load<DamageFormulaScript>(ServerContext.Config.BaseDamageScript, this, target, type);
            return script?.Values.Sum(s => s.Calculate(this, target, type)) ?? 1;
        }

        public string GetDebuffName(Func<Debuff, bool> p)
        {
            if (Debuffs == null || Debuffs.Count == 0)
                return string.Empty;

            return Debuffs.Select(i => i.Value)
                .FirstOrDefault(p)
                ?.Name;
        }

        public double GetElementalModifier(Sprite damageDealingSprite)
        {
            if (damageDealingSprite == null)
                return 1;

            var element = CheckRandomElement(damageDealingSprite.OffenseElement);
            var saved = DefenseElement;

            var amplifier = CalculateElementalDamageMod(element);
            {
                DefenseElement = saved;
            }

            if (damageDealingSprite.Amplified == 0)
                return amplifier;

            amplifier *= Amplified == 1
                ? ServerContext.Config.FasNadurStrength
                : ServerContext.Config.MorFasNadurStrength;

            return amplifier;
        }

        public void OnDamaged(Sprite source, int dmg)
        {
            (this as Aisling)?.Client.SendStats(StatusFlags.StructB);
            (source as Aisling)?.Client.SendStats(StatusFlags.StructB);

            if (!(this is Monster))
                return;

            FaceWhoeverHit(source);

            if (!(source is Aisling aisling))
                return;

            var monsterScripts = (this as Monster)?.Scripts;

            if (monsterScripts == null)
                return;

            foreach (var script in monsterScripts.Values)
                script?.OnDamaged(aisling?.Client, dmg, source);
        }

        /// <summary>
        /// 맞은 괴물은 때린 쪽을 바라본다 — 원작이 그렇다(`docs/monster-behaviour.md`).
        /// </summary>
        /// <remarks>
        /// <para>
        /// 이걸 안 하면 등 뒤 배수가 거저 나온다. 괴물은 방향 0(북)으로 서므로(아무 데서도 정해 주지 않는다)
        /// 북쪽 문으로 들어와 남쪽에서 치면 첫 타가 늘 등 뒤다. 하데스는 괴물이 <c>CommonMonster</c> 의
        /// 돌아가는 차례에서만 돌아섰고, 그 차례는 <c>Monster.CanMove</c> 가 막으면 오지 않으므로
        /// <b>제자리 고정 괴물은 영영 안 돌아섰다</b>. 여기는 걸음과 무관하게 돈다.
        /// </para>
        /// <para>
        /// 한 방이 다 들어간 뒤에 돈다(<see cref="ApplyDamage" /> 가 <see cref="DamageTarget" /> 다음에
        /// <see cref="OnDamaged" /> 를 부른다). 그래서 등 뒤에서 친 첫 타는 ×2 를 받고, 다음 타부터 정면이다.
        /// 대각선(|dx| = |dy|)은 가로를 먼저 본다 — <see cref="Facing" /> 의 표는 대각선을 −1 로 내놓아
        /// 그대로 쓰면 방향이 255 가 된다.
        /// </para>
        /// <para>
        /// 5.99 는 여기서 돌지 않고 <b>표적만 적어 둔다</b>(0x4242ab `M+0x18 = 1` · 0x4242bb
        /// `M+0x1C = 때린 사람`). 몸은 다음 AI 차례에 돌고(0x425de6 · 0x425e88 · 0x425f0b · 0x425f8e),
        /// 도는 차례에는 때리지 않는다. 그 판에서는 AI 차례보다 빨리 돌면 등 뒤를 계속 잡을 수 있다는
        /// 뜻인데, 5.99 는 등 뒤 배수 자체가 죽은 코드라 아무도 그걸 겪지 않았다. 여기서는 맞는 즉시 돌린다 —
        /// 사용자가 말한 대로이고, 빨리 치는 것만으로 등 뒤 ×2 를 계속 받는 길을 막는다.
        /// </para>
        /// </remarks>
        private void FaceWhoeverHit(Sprite source)
        {
            if (source == null || source.Serial == Serial)
                return;

            var dx = source.XPos - XPos;
            var dy = source.YPos - YPos;

            if (dx == 0 && dy == 0)
                return;

            var facing = Math.Abs(dx) >= Math.Abs(dy)
                ? dx > 0 ? (byte) 1 : (byte) 3
                : dy > 0 ? (byte) 2 : (byte) 0;

            if (Direction == facing)
                return;

            Direction = facing;
            Turn();
        }

        public bool HasBuff(string buff)
        {
            if (Buffs == null || Buffs.Count == 0)
                return false;

            return Buffs.ContainsKey(buff);
        }

        public bool HasDebuff(string debuff)
        {
            if (Debuffs == null || Debuffs.Count == 0)
                return false;

            return Debuffs.ContainsKey(debuff);
        }

        private bool HasDebuff(Func<Debuff, bool> p)
        {
            if (Debuffs == null || Debuffs.Count == 0)
                return false;

            return Debuffs.Select(i => i.Value).FirstOrDefault(p) != null;
        }

        private void RemoveAllBuffs()
        {
            if (Buffs == null)
                return;

            foreach (var buff in Buffs)
                RemoveBuff(buff.Key);
        }

        private void RemoveAllDebuffs()
        {
            if (Debuffs == null)
                return;

            foreach (var debuff in Debuffs)
                RemoveDebuff(debuff.Key);
        }

        private bool RemoveBuff(string buff)
        {
            if (!HasBuff(buff)) return false;
            var buffObj = Buffs[buff];
            buffObj?.OnEnded(this, buffObj);

            return true;
        }

        public void RemoveBuffsAndDebuffs()
        {
            RemoveAllBuffs();
            RemoveAllDebuffs();
        }

        public bool RemoveDebuff(string debuff, bool cancelled = false)
        {
            if (!cancelled && debuff == "skulled")
                return true;

            if (!HasDebuff(debuff)) return false;
            var buffObj = Debuffs[debuff];

            if (buffObj == null) return false;
            buffObj.Cancelled = cancelled;
            buffObj.OnEnded(this, buffObj);

            return true;
        }
        #endregion

        #region Status
        public void Update()
        {
            Show(Scope.NearbyAislings, new ServerFormat0E(Serial));
            Show(Scope.NearbyAislings, new ServerFormat07(new[] { this }));
        }

        public void UpdateBuffs(TimeSpan elapsedTime)
        {
            foreach (var buff in Buffs) buff.Value?.Update(this, elapsedTime);
        }

        public void UpdateDebuffs(TimeSpan elapsedTime)
        {
            foreach (var debuff in Debuffs) debuff.Value?.Update(this, elapsedTime);
        }

        private bool CanUpdate()
        {
            if (IsSleeping || IsFrozen || IsBlind)
                return false;

            if (this is Monster || this is Mundane)
                if (CurrentHp == 0)
                    return false;

            if (ServerContext.Config.CanMoveDuringReap)
                return true;

            if (!(this is Aisling aisling))
                return true;

            if (!aisling.Skulled)
                return true;

            aisling.Client.SystemMessage(ServerContext.Config.ReapMessageDuringAction);
            return false;
        }
        #endregion

        #region Sprite Methods
        public TSprite Cast<TSprite>() where TSprite : Sprite
        {
            return this as TSprite;
        }

        public void Remove()
        {
            var nearby = GetObjects<Aisling>(null, i => i != null && i.LoggedIn);
            var response = new ServerFormat0E(Serial);

            foreach (var o in nearby)
                for (var i = 0; i < 2; i++)
                    o?.Client?.Send(response);

            DeleteObject();
        }

        public void HideFrom(Aisling nearbyAisling)
        {
            nearbyAisling?.Show(Scope.Self, new ServerFormat0E(Serial));
        }

        public void Animate(ushort animation, byte speed = 100)
        {
            Show(Scope.NearbyAislings, new ServerFormat29((uint)Serial, (uint)Serial, animation, animation, speed));
        }

        public Aisling SendAnimation(ushort animation, Sprite to, Sprite from, byte speed = 100)
        {
            var format = new ServerFormat29((uint)from.Serial, (uint)to.Serial, animation, 0, speed);
            {
                Show(Scope.NearbyAislings, format);
            }

            return Aisling(this);
        }

        public void SendAnimation(ushort v, Position position)
        {
            Show(Scope.NearbyAislings, new ServerFormat29(v, position.X, position.Y));
        }

        private void DeleteObject()
        {
            if (this is Monster)
                DelObject(this as Monster);
            if (this is Aisling)
                DelObject(this as Aisling);
            if (this is Money)
                DelObject(this as Money);
            if (this is Item)
                DelObject(this as Item);
            if (this is Mundane)
                DelObject(this as Mundane);
        }

        private static float Sqrt(float number)
        {
            var x = number * 0.5f;
            var y = number;

            unsafe
            {
                var i = *(ulong*)&y;
                i = 0x5F3759DF - (i >> 1);
                y = *(float*)&i;
                y *= 1.5f - x * y * y;
                y *= 1.5f - x * y * y;
            }

            return number * y;
        }
        #endregion
    }
}