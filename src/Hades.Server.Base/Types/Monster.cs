#region

using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Systems.Loot;

using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

#endregion

namespace Darkages.Types
{
    public class Monster : Sprite
    {
        public Monster()
        {
            BashEnabled = false;
            CastEnabled = false;
            WalkEnabled = false;
            WaypointIndex = 0;
            TaggedAislings = new HashSet<int>();

            EntityType = TileContent.Monster;
        }

        public bool Aggressive { get; set; }
        public bool BashEnabled { get; set; }
        public bool CastEnabled { get; set; }
        public ushort Image { get; set; }
        public bool WalkEnabled { get; set; }
        public GameServerTimer BashTimer { get; set; }
        public GameServerTimer CastTimer { get; set; }
        public MonsterTemplate Template { get; set; }
        public GameServerTimer WalkTimer { get; set; }
        [JsonIgnore] public LootTable UpgradeTable { get; set; }
        [JsonIgnore] public bool IsAlive => CurrentHp > 0;
        [JsonIgnore] public LootDropper LootManager { get; set; }
        [JsonIgnore] public LootTable LootTable { get; set; }
        [JsonIgnore] public bool Rewarded { get; set; }
        [JsonIgnore] public Dictionary<string, MonsterScript> Scripts { get; set; }
        [JsonIgnore] public bool Skulled { get; set; }
        [JsonIgnore] public HashSet<int> TaggedAislings { get; set; }
        [JsonIgnore] public int WaypointIndex;

        [JsonIgnore] public Aisling Summoner => GetObject<Aisling>(Map, b => b.Serial == SummonerId);

        [JsonIgnore] public Position CurrentWaypoint => Template?.Waypoints?[WaypointIndex];
        [JsonIgnore] public Item GlobalLastItemRoll { get; set; }
        [JsonIgnore]  public int SummonerId { get; set; }

        public static Monster Create(MonsterTemplate template, Area map)
        {
            var monsterCreateScript = ScriptManager.Load<MonsterCreateScript>(ServerContext.Config.MonsterCreationScript,
                    template,
                    map)
                .FirstOrDefault();

            return monsterCreateScript.Value?.Create(template, map);
        }

        public static void InitScripting(MonsterTemplate template, Area map, Monster obj)
        {
            if (obj.Scripts == null || !obj.Scripts.Any())
                obj.Scripts = ScriptManager.Load<MonsterScript>(template.ScriptName, obj, map);
        }

        public static void DistributeExperience(Aisling player, double exp)
        {
            var chunks = exp / 1000;

            if (chunks <= 1)
                HandleExp(player, exp);
            else
                for (var i = 0; i < chunks; i++)
                    HandleExp(player, 1000);
        }

        public void AppendTags(Sprite target)
        {
            if (TaggedAislings == null)
                TaggedAislings = new HashSet<int>();

            if (!(target is Aisling aisling))
                return;

            if (!TaggedAislings.Contains(aisling.Serial))
                TaggedAislings.Add(aisling.Serial);

            if (aisling.GroupParty != null && aisling.GroupParty.PartyMembers.Count - 1 <= 0)
                return;

            if (aisling.GroupParty == null) return;

            foreach (var member in aisling.GroupParty.PartyMembers.Where(member =>
                !TaggedAislings.Contains(member.Serial)))
                TaggedAislings.Add(member.Serial);
        }

        public void GenerateRewards(Aisling player)
        {
            if (Rewarded)
                return;

            if (player.Equals(null))
                return;

            if (player.Client.Aisling == null)
                return;

            var script = ScriptManager.Load<RewardScript>(ServerContext.Config.MonsterRewardScript, this, player).FirstOrDefault();
            script.Value?.GenerateRewards(this, player);

            Rewarded = true;
            player.UpdateStats();
        }

        public List<Aisling> GetTaggedAislings()
        {
            if (TaggedAislings.Any())
                return TaggedAislings.Select(b => GetObject<Aisling>(Map, n => n.Serial == b)).Where(i => i != null)
                    .ToList();

            return new List<Aisling>();
        }

        public void Patrol(bool ignoreWalls = false)
        {
            if (CurrentWaypoint != null) WalkTo(CurrentWaypoint.X, CurrentWaypoint.Y, ignoreWalls);

            if (Position.DistanceFrom(CurrentWaypoint) <= 1 || CurrentWaypoint == null)
            {
                if (WaypointIndex + 1 < Template.Waypoints.Count)
                    WaypointIndex++;
                else
                    WaypointIndex = 0;
            }
        }

        private static void HandleExp(Aisling player, double exp)
        {
            if (exp <= 0)
                exp = 1;

            if (player.GroupParty != null)
            {
                var bonus = exp * (1 + player.GroupParty.PartyMembers.Count - 1) *
                            ServerContext.Config.GroupExpBonus /
                            100;

                if (bonus > 0)
                    exp += bonus;
            }

            player.ExpTotal += (uint) exp;
            player.ExpNext -= (uint) exp;

            if (player.ExpNext >= int.MaxValue) player.ExpNext = 0;

            {
                if (player.ExpLevel >= ServerContext.Config.PlayerLevelCap)
                    return;
            }

            while (player.ExpNext <= 0 && player.ExpLevel < 99)
            {
                // 지금 ExpLevel 은 아직 오르기 전의 값이다. 아래에서 Levelup 이 하나 올리므로, 그 뒤에
                // 사람이 바라볼 다음 목표는 (지금+2) 레벨에 닿는 값이다.
                player.ExpNext = ExperienceCurve.ToReach(player.ExpLevel + 2);

                if (player.ExpLevel == 99)
                    break;

                if (player.ExpTotal <= 0)
                    player.ExpTotal = uint.MaxValue;

                if (player.ExpTotal >= uint.MaxValue)
                    player.ExpTotal = uint.MaxValue;

                if (player.ExpNext <= 0)
                    player.ExpNext = 1;

                if (player.ExpNext >= uint.MaxValue)
                    player.ExpNext = uint.MaxValue;

                Levelup(player);
            }
        }

        /// <summary>레벨마다 콘에 더해 오르는 최대 체력(원작 상수 30).</summary>
        public const int HpPerLevel = 30;

        /// <summary>레벨마다 위즈에 더해 오르는 최대 마력(원작 상수 25).</summary>
        public const int MpPerLevel = 25;

        public static void Levelup(Aisling player)
        {
            if (player.ExpLevel >= ServerContext.Config.PlayerLevelCap)
                return;

            // 원작(5.99 Novaonline.exe 0x469a46~0x469a94 · 혼든 Yuki.exe 0x45fa29~0x45fa46): 기본 최대 체력에
            // 콘+30, 기본 최대 마력에 위즈+25. 장비로 붙는 몫이 아닌 제 능력치(_Con·_Wis)를 읽는다. 직업·무작위 없음.
            player._MaximumHp += player._Con + HpPerLevel;
            player._MaximumMp += player._Wis + MpPerLevel;
            player.StatPoints += ServerContext.Config.StatsPerLevel;

            player.ExpLevel++;

            player.Client.SendMessage(0x02,
                string.Format(ServerContext.Config.LevelUpMessage, player.ExpLevel));
            player.Show(Scope.NearbyAislings,
                new ServerFormat29((uint) player.Serial, (uint) player.Serial, 0x004F, 0x004F, 64));
        }
    }
}