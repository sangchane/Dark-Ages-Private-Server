#region

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Newtonsoft.Json;


#endregion

namespace Darkages.Types
{
    public class MonsterTemplate : Template
    {
        [Description("What Drops?")]
        public List<string> Drops = new List<string>();

        /// <summary>
        /// Its armour, when the definition states one. Lower is better and the floor is -70.
        /// Null leaves it to the level, which is what the monsters Hades itself ships do.
        /// </summary>
        [Description("Armour. Lower is better. Leave unset to work it out from Level.")]
        public int? Ac { get; set; }

        public int AreaID { get; set; }

        public int AttackSpeed { get; set; }

        public string BaseName { get; set; }

        public int CastSpeed { get; set; }

        public ElementManager.Element DefenseElement { get; set; }

        [Description("Leave empty unless SpawnQualifer = Defined.")]
        public ushort DefinedX { get; set; }

        [Description("Leave empty unless SpawnQualifer = Defined.")]
        public ushort DefinedY { get; set; }

        /// <summary>
        /// The most and least one of its blows is worth, before armour and elements, when the definition
        /// states them. Null for either leaves the blow to the level.
        /// </summary>
        [Description("Hardest blow, before armour. Leave unset to work it out from Level.")]
        public int? DmgMax { get; set; }

        [Description("Softest blow, before armour. Leave unset to work it out from Level.")]
        public int? DmgMin { get; set; }

        public ElementQualifer ElementType { get; set; }

        /// <summary>What killing it is worth. Null leaves it to the level.</summary>
        [Description("Experience for killing it. Leave unset to work it out from Level.")]
        public int? Exp { get; set; }

        public int EngagedWalkingSpeed { get; set; }

        /// <summary>
        /// 죽을 때 떨구는 금화. 5.99 의 <c>골드 &lt;액수&gt; &lt;확률%&gt;</c> 를 그대로 옮긴 것이다.
        /// Null 은 레벨에 맡긴다는 뜻인데, 하데스의 정의는 모두 <c>Level 1</c> 이라 그 길은 500~999 전이다.
        /// </summary>
        [Description("Gold it drops. Leave unset to work it out from Level.")]
        public int? Gold { get; set; }

        /// <summary>백에 몇 번 떨어지나. Null 이면 늘 떨어진다. <see cref="Gold" /> 가 없으면 보지 않는다.</summary>
        [Description("Chance in a hundred the gold drops at all. Leave unset for always.")]
        public int? GoldChance { get; set; }

        public string FamilyKey { get; set; }

        [Description("Does this monster grow stonger over time? default = false")]
        public bool Grow { get; set; }

        public bool IgnoreCollision { get; set; }

        [Description("What sprite ID? range from 0x4000 - 0x8000 ")]
        public ushort Image { get; set; }

        [Description("Does this monster have various other sprites? use 0 if not.")]
        public int ImageVarience { get; set; }

        public int Level { get; set; }

        public LootQualifer LootType { get; set; }

        public int MaximumHP { get; set; }

        public int MaximumMP { get; set; }

        public MoodQualifer MoodType { get; set; }

        public int MovementSpeed { get; set; }

        [JsonIgnore] public DateTime NextAvailableSpawn { get; set; }

        public ElementManager.Element OffenseElement { get; set; }

        public PathQualifer PathQualifer { get; set; }

        [JsonIgnore] public bool Ready => DateTime.UtcNow > NextAvailableSpawn;

        [Description("What script will this monster run?")]
        public string ScriptName { get; set; }

        [Description("What Skills will this monster use?")]
        public List<string> SkillScripts { get; set; } = new List<string>();

        [Description("Monsters spawned will not exceed this.")]
        public int SpawnMax { get; set; }

        [Description("Does this aisling spawn if no aislings are on this map? default = false")]
        public bool SpawnOnlyOnActiveMaps { get; set; }

        [Description("In seconds, what is the respawn rate?")]
        public int SpawnRate { get; set; }

        [Description("How many monsters will i spawn at any single time?")]
        public int SpawnSize { get; set; }

        public SpawnQualifer SpawnType { get; set; }

        [Description("What Spells will this monster cast?")]
        public List<string> SpellScripts { get; set; }

        public bool UpdateMapWide { get; set; }
        public double UpdateRate { get; set; } = 1000;
        public List<Position> Waypoints { get; set; }

        public override string[] GetMetaData()
        {
            return new[]
            {
                ""
            };
        }

        /// <summary>
        /// 이번 순회에 이 정의를 세워도 되는지 묻고, 세운다면 <paramref name="seconds" /> 만큼 재운다.
        /// </summary>
        /// <remarks>
        /// 간격이 <see cref="SpawnRate" /> 가 아니라 인자인 이유는 같은 정의라도 맵이 넓으면 더 자주
        /// 세워야 같은 밀도가 되기 때문이다. 부르는 쪽(<c>MonolithComponent</c>)이 맵 넓이를 안다.
        /// </remarks>
        public bool ReadyToSpawn(double seconds)
        {
            if (!Ready)
                return false;

            NextAvailableSpawn = DateTime.UtcNow.AddSeconds(seconds);
            return true;
        }
    }
}