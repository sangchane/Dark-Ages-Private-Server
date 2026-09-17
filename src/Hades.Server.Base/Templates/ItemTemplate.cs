#region

using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using Darkages.Systems.Loot.Interfaces;

using System.Text.Json;
using static Darkages.Types.ElementManager;
using Newtonsoft.Json;

#endregion

namespace Darkages.Types
{
    public class Common : ItemUpgrade
    {
        public override string Name => "Upgraded";
        public override int Upgrade => 1;
        public override double Weight => 2.5;
    }

    public class Epic : ItemUpgrade
    {
        public override string Name => "Epic";
        public override int Upgrade => 4;
        public override double Weight => 0.1;
    }

    public class Forsaken : ItemUpgrade
    {
        public override string Name => "Forsaken";
        public override int Upgrade => 8;
        public override double Weight => 0.02;
    }

    public class Godly : ItemUpgrade
    {
        public override string Name => "Godly";
        public override int Upgrade => 7;
        public override double Weight => 0.05;
    }

    public class ItemTemplate : Template, ILootDefinition
    {
        [Category("Mods")] public StatusOperator AcModifer { get; set; }

        /// <summary>
        /// 5.99 아이템의 공격모션·공격속도. 5.99 서버(Novaonline.exe 0x4160f7)는 평타의 몸 동작을 무기에서, 무기가
        /// 없으면 갑옷에서 고른다(도복 132 주먹 · 단검 134 찌르기 · 활 142). 0 은 칸이 없는 것이다.
        /// </summary>
        [Category("Item Properties")] public byte AttackMotion { get; set; }

        [Category("Item Properties")] public byte AttackSpeed { get; set; }

        /// <summary>
        /// 5.99 소모품의 체력변화·마력변화 — 쓰면 그만큼 돌아온다(물약·음식). 장비의 같은 칸은 입었을 때 최대치를 올리는
        /// 것이라 <see cref="HealthModifer" /> 로 가고, 쓰고 없어지는 것만 이 칸을 쓴다(scripts/Items/Consumable.cs).
        /// </summary>
        [Category("Item Properties")] public int HealthRestore { get; set; }

        [Category("Item Properties")] public int ManaRestore { get; set; }

        /// <summary>5.99 귀환 주문서의 이동맵·이동좌표 — 쓰면 그 맵 그 칸으로 간다. 맵 번호 0 은 칸이 없는 것이다.</summary>
        [Category("Item Properties")] public int RecallArea { get; set; }

        [Category("Item Properties")] public int RecallX { get; set; }

        [Category("Item Properties")] public int RecallY { get; set; }

        /*
                [Category("Script")] public ActivationTrigger ActivationTrigger { get; set; } = ActivationTrigger.OnUse;
        */
        public bool CanStack { get; set; }
        [Category("Item Properties")] public byte CarryWeight { get; set; }

        [Category("Requirements")]
        public Class Class { get; set; }

        [Category("Item Properties")]
        public ItemColor Color { get; set; }

        [Category("Mods")] public StatusOperator ConModifer { get; set; }

        [Category("Elements")]
        public Element DefenseElement { get; set; }

        [Category("Mods")] public StatusOperator DexModifer { get; set; }
        [Browsable(false)] public ushort DisplayImage { get; set; }
        [Category("Item Properties")] public int DmgMax { get; set; }
        [Category("Item Properties")] public int DmgMin { get; set; }
        [Category("Mods")] public StatusOperator DmgModifer { get; set; }
        [Category("Item Properties")] public double DropRate { get; set; }
        [Category("Item Properties")] public bool Enchantable { get; set; }
        [Browsable(false)] public int EquipmentSlot { get; set; }

        [Category("Item Properties")]
        [JsonIgnore]
        public EquipSlot EquipSlot { get; set; }

        [Category("Item Properties")]
        public ItemFlags Flags { get; set; }

        public Gender Gender { get; set; }

        [Category("Item Properties")] public bool HasPants { get; set; }
        [Category("Mods")] public StatusOperator HealthModifer { get; set; }
        [Category("Mods")] public StatusOperator HitModifer { get; set; }
        [Browsable(false)] public ushort Image { get; set; }
        [Category("Mods")] public StatusOperator IntModifer { get; set; }
        [Category("Requirements")] public byte LevelRequired { get; set; }
        [Category("Mods")] public StatusOperator ManaModifer { get; set; }
        [Category("Item Properties")] public uint MaxDurability { get; set; }
        /// <summary>
        /// 한 칸에 쌓을 수 있는 수. 예전에는 1바이트라 255 를 넘는 값을 적으면 **아이템 정의를 읽다 실패하고 서버가
        /// 접속을 받지 못했다**(2026-09-18, 시약을 1000 으로 두었을 때). 보내는 쪽은 이미 더 큰 수를 담는다 —
        /// 묶음 수는 <see cref="Types.Item.Stacks" /> 가 2바이트이고 0x0F 는 4바이트로 쓴다.
        /// </summary>
        public int MaxStack { get; set; }
        [Category("Mods")] public StatusOperator MrModifer { get; set; }
        [Browsable(false)] public string NpcKey { get; set; }

        [Category("Elements")]
        public Element OffenseElement { get; set; }

        [Category("Mods")] public StatusOperator RegenModifer { get; set; }
        [Browsable(false)] public string ScriptName { get; set; }
        [Category("Mods")] public SpellOperator SpellOperator { get; set; }

        [Category("Requirements")]
        [Description("What state is required of the player to use the item?")]
        public ClassStage StageRequired { get; set; }

        [Category("Mods")] public StatusOperator StrModifer { get; set; }
        [Category("Item Properties")] public uint Value { get; set; }
        [Browsable(false)] public string WeaponScript { get; set; }

        [JsonIgnore]
        [Browsable(false)]
        public double Weight
        {
            get => DropRate;
            set { }
        }


        [Category("Mods")] public StatusOperator WisModifer { get; set; }

        public override string[] GetMetaData()
        {
            var category = string.IsNullOrEmpty(Group) ? string.Empty : Group;

            if (string.IsNullOrEmpty(category)) category = Class == Class.Peasant ? "All" : Class.ToString();

            return new[]
            {
                LevelRequired.ToString(),
                ((int) Class).ToString(),
                CarryWeight.ToString(),
                Gender switch
                {
                    Gender.Both => category,
                    Gender.Female => "Female " + category,
                    Gender.Male => "Male " + category,
                    _ => throw new ArgumentOutOfRangeException()
                },
                Gender switch
                {
                    Gender.Both => "All",
                    Gender.Female => "Female " + category,
                    Gender.Male => "Male " + category,
                    _ => throw new ArgumentOutOfRangeException()
                } + $" Lev{LevelRequired}, Wt {CarryWeight}"
            };
        }
    }

    public class ItemUpgrade : ILootDefinition
    {
        public virtual string Name { get; set; }
        public virtual int Upgrade { get; set; }
        public virtual double Weight { get; set; }
    }

    public class Legendary : ItemUpgrade
    {
        public override string Name => "Legendary";
        public override int Upgrade => 5;
        public override double Weight => 0.08;
    }

    public class Mythical : ItemUpgrade
    {
        public override string Name => "Mythical";
        public override int Upgrade => 6;
        public override double Weight => 0.06;
    }

    public class Rare : ItemUpgrade
    {
        public override string Name => "Rare";
        public override int Upgrade => 3;
        public override double Weight => 0.5;
    }

    public class Uncommon : ItemUpgrade
    {
        public override string Name => "Enhanced";
        public override int Upgrade => 2;
        public override double Weight => 1.5;
    }
}