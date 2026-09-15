using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Common;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Systems.Loot;
using Darkages.Types;
using static Darkages.Common.Generator;

namespace Darkages.Storage.locales.Scripts.Creations
{
    [Script("Create Monster", "Wren", "Default Monster Creation Script")]
    public class Monsters : MonsterCreateScript
    {
        private readonly MonsterTemplate _template;
        private readonly Area _map;

        public Monsters(MonsterTemplate template, Area map)
        {
            _template = template;
            _map = map;
        }

        public override Monster Create(MonsterTemplate template, Area map)
        {
            void TemplateCreationSanityChecks()
            {
                if (template.CastSpeed == 0)
                    template.CastSpeed = 2000;

                if (template.AttackSpeed == 0)
                    template.AttackSpeed = 1000;

                if (template.MovementSpeed == 0)
                    template.MovementSpeed = 2000;

                if (template.Level <= 0)
                    template.Level = 1;
            }

            // 한 번 젠할 때 자리를 찾아 보는 횟수. 우드랜드1-1 은 3,600칸 중 벽이 절반이 안 되므로
            // 열 번이면 사실상 늘 찾는다. 벽뿐인 실내에서 영영 돌지 않게 상한을 둔다.
            const int TilePicks = 10;

            bool FindBestMonsterMapSlot(Monster monster)
            {
                switch (template.SpawnType)
                {
                    case SpawnQualifer.Random:
                        {
                            // 칸을 한 번만 찍어 보고 벽이면 포기하던 자리다. 포기는 공짜가 아니다 —
                            // 부르는 쪽이 이미 ReadyToSpawn() 을 써 버렸으므로 그 정의는 SpawnRate 만큼
                            // (우드랜드는 50초) 다시 잠든다. 벽은 실패가 아니라 다시 찍으면 되는 것이므로
                            // 설 만한 칸이 나올 때까지 몇 번 더 찍는다. 그래도 못 찾으면 그때는 정말
                            // 자리가 없는 것이다(사람이 꽉 찬 방, 벽뿐인 실내).
                            for (var pick = 0; pick < TilePicks; pick++)
                            {
                                int x, y;

                                lock (Generator.Random)
                                {
                                    x = Generator.Random.Next(1, map.Cols);
                                    y = Generator.Random.Next(1, map.Rows);
                                }

                                if (map.IsWall(x, y))
                                    continue;

                                // 이미 누가 서 있는 칸에 세우면 둘이 겹친다. ObjectGrid 가 그 칸에 선
                                // 괴물·NPC·사람을 알고 있으므로 물어보고 피한다.
                                if (map.ObjectGrid[x, y].Sprites.Count > 0)
                                    continue;

                                monster.XPos = x;
                                monster.YPos = y;

                                return false;
                            }

                            return true;
                        }
                    case SpawnQualifer.Defined:
                        monster.XPos = template.DefinedX;
                        monster.YPos = template.DefinedY;
                        break;
                }

                return false;
            }

            //validate values in template.
            TemplateCreationSanityChecks();

            if (template.Name == "test")
            {

            }

            var obj = new Monster
            {
                Template = template,
                CastTimer = new GameServerTimer(TimeSpan.FromMilliseconds(1 + template.CastSpeed)),
                BashTimer = new GameServerTimer(TimeSpan.FromMilliseconds(1 + template.AttackSpeed)),
                WalkTimer = new GameServerTimer(TimeSpan.FromMilliseconds(1 + template.MovementSpeed)),
                TaggedAislings = new HashSet<int>()
            };

            if (obj.Template.Grow)
                obj.Template.Level++;

            // 정의가 체력을 적어 두면 그것이 답이다. 예전에는 여기서 레벨로 만든 값을 그 위에 덮어써서,
            // 파일에 적힌 숫자가 쓰이지 않았다 — 하데스가 싣는 spider.json 의 680 도 275 로 덮였다.
            // 안 적어 둔 정의(bees·minion 은 0)는 그대로 레벨로 만든다. Grow 는 젠할 때마다 레벨을
            // 올리므로 늘 다시 만든다.
            if (obj.Template.Grow || obj.Template.MaximumHP <= 0)
            {
                var mod = (obj.Template.Level + 1) * 0.01;
                var hp = mod + 50 + obj.Template.Level * (obj.Template.Level + 40);

                obj.Template.MaximumHP = (int)hp;
                obj.Template.MaximumMP = (int)(hp / 3);
            }

            // 마력을 정한 **뒤에** 본다. 예전에는 위 초기화 구문에서 읽어, 아직 0 이던 첫 젠은 마법이
            // 꺼지고 두 번째부터 켜졌다 — 같은 정의인데 결과가 달랐다.
            obj.CastEnabled = obj.Template.MaximumMP > 0;

            var stat = RandomEnumValue<PrimaryStat>();

            obj._Str = 1;
            obj._Int = 1;
            obj._Wis = 1;
            obj._Con = 1;
            obj._Dex = 1;

            switch (stat)
            {
                case PrimaryStat.STR:
                    obj._Str += (byte)(obj.Template.Level * 0.5 * 2);
                    break;

                case PrimaryStat.INT:
                    obj._Int += (byte)(obj.Template.Level * 0.5 * 2);
                    break;

                case PrimaryStat.WIS:
                    obj._Wis += (byte)(obj.Template.Level * 0.5 * 2);
                    break;

                case PrimaryStat.CON:
                    obj._Con += (byte)(obj.Template.Level * 0.5 * 2);
                    break;

                case PrimaryStat.DEX:
                    obj._Dex += (byte)(obj.Template.Level * 0.5 * 2);
                    break;
            }

            obj.MajorAttribute = stat;

            // 정의가 방어를 적어 두면 그것이 답이다. 0 도 적어 둔 값일 수 있으므로(초보 사냥터 괴물이
            // 그렇다) "없음" 은 null 로 가린다.
            obj.BonusAc = obj.Template.Ac ?? (int)(70 - obj.Template.Level * 0.5 / 1.0);

            if (obj.BonusAc < -70) obj.BonusAc = -70;

            obj.DefenseElement = ElementManager.Element.None;
            obj.OffenseElement = ElementManager.Element.None;

            if (obj.Template.ElementType == ElementQualifer.Random)
            {
                obj.DefenseElement = RandomEnumValue<ElementManager.Element>();
                obj.OffenseElement = RandomEnumValue<ElementManager.Element>();
            }
            else if (obj.Template.ElementType == ElementQualifer.Defined)
            {
                obj.DefenseElement = template.DefenseElement == ElementManager.Element.None
                    ? RandomEnumValue<ElementManager.Element>()
                    : template.DefenseElement;
                obj.OffenseElement = template.OffenseElement == ElementManager.Element.None
                    ? RandomEnumValue<ElementManager.Element>()
                    : template.OffenseElement;
            }

            obj.BonusMr = (byte)(10 * (template.Level / 20));

            if (obj.BonusMr > ServerContext.Config.BaseMR)
                obj.BonusMr = ServerContext.Config.BaseMR;

            if ((template.PathQualifer & PathQualifer.Wander) == PathQualifer.Wander)
                obj.WalkEnabled = true;
            else if ((template.PathQualifer & PathQualifer.Fixed) == PathQualifer.Fixed)
                obj.WalkEnabled = false;
            else if ((template.PathQualifer & PathQualifer.Patrol) == PathQualifer.Patrol)
                obj.WalkEnabled = true;

            if (template.MoodType.HasFlag(MoodQualifer.Aggressive))
                obj.Aggressive = true;
            else if (template.MoodType.HasFlag(MoodQualifer.Unpredicable))
                lock (Generator.Random)
                {
                    obj.Aggressive = Generator.Random.Next(1, 101) > 50;
                }
            else
                obj.Aggressive = false;

            //Based on Spawn type, Work out where i need to spawn.
            if (FindBestMonsterMapSlot(obj))
                return null;

            lock (Generator.Random)
            {
                obj.Serial = GenerateNumber();
            }

            obj.CurrentMapId = map.ID;
            obj.CurrentHp = template.MaximumHP;
            obj.CurrentMp = template.MaximumMP;
            obj._MaximumHp = template.MaximumHP;
            obj._MaximumMp = template.MaximumMP;
            obj.AbandonedDate = DateTime.UtcNow;

            lock (Generator.Random)
            {
                obj.Image = template.ImageVarience
                            > 0
                    ? (ushort)Generator.Random.Next(template.Image, template.Image + template.ImageVarience)
                    : template.Image;
            }

            //Don't load scripts on creation. Instead on Approach.
            //InitScripting(template, map, obj);

            if (!obj.Template.LootType.HasFlag(LootQualifer.Table)) return obj;
            obj.LootManager = new LootDropper();
            obj.LootTable = new LootTable(template.Name);
            obj.UpgradeTable = new LootTable("Probabilities");

            foreach (var drop in obj.Template.Drops)
                if (drop.Equals("random", StringComparison.OrdinalIgnoreCase))
                {
                    lock (Generator.Random)
                    {
                        var available = ServerContext.GlobalItemTemplateCache.Select(i => i.Value)
                            .Where(i => Math.Abs(i.LevelRequired - obj.Template.Level) <= 10).ToList();
                        if (available.Count > 0) obj.LootTable.Add(available[GenerateNumber() % available.Count]);
                    }
                }
                else
                {
                    if (ServerContext.GlobalItemTemplateCache.ContainsKey(drop))
                        obj.LootTable.Add(ServerContext.GlobalItemTemplateCache[drop]);
                }

            obj.UpgradeTable.Add(new Types.Common());
            obj.UpgradeTable.Add(new Uncommon());
            obj.UpgradeTable.Add(new Rare());
            obj.UpgradeTable.Add(new Epic());
            obj.UpgradeTable.Add(new Legendary());
            obj.UpgradeTable.Add(new Mythical());
            obj.UpgradeTable.Add(new Godly());
            obj.UpgradeTable.Add(new Forsaken());

            return obj;
        }
    }
}
