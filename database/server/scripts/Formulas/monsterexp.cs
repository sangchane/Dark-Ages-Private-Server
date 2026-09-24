using Darkages.Common;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Darkages.Storage.locales.Scripts.Formulas
{
    [Script("Monster Exp 1x", "Wren", "Base Script for handling monster rewards and exp.")]
    public class Monsterexp : RewardScript
    {
        private readonly Monster _monster;
        private readonly Aisling _player;

        public Monsterexp(Monster monster, Aisling player)
        {
            _monster = monster;
            _player = player;
        }

        public override void GenerateRewards(Monster monster, Aisling player)
        {
            UpdateCounters(player);
            GenerateExperience(player);
            GenerateGold();
            GenerateDrops();
        }

        private void HandleExp(Aisling player, double exp)
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

            player.ExpTotal += (uint)exp;
            player.ExpNext -= (uint)exp;

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

        public static void Levelup(Aisling player)
        {
            if (player.ExpLevel >= ServerContext.Config.PlayerLevelCap)
                return;

            player._MaximumHp += (int)(ServerContext.Config.HpGainFactor * player.Con * 0.65);
            player._MaximumMp += (int)(ServerContext.Config.MpGainFactor * player.Wis * 0.45);
            player.StatPoints += ServerContext.Config.StatsPerLevel;

            player.ExpLevel++;

            player.Client.SendMessage(0x02,
                string.Format(ServerContext.Config.LevelUpMessage, player.ExpLevel));
            player.Show(Scope.NearbyAislings,
                new ServerFormat29((uint)player.Serial, (uint)player.Serial, 0x004F, 0x004F, 64));
        }

        private List<string> DetermineDrop()
        {
            return _monster.LootManager.Drop(_monster.LootTable, Generator.Random.Next(ServerContext.Config.LootTableStackSize))
                .Select(i => i?.Name).ToList();
        }

        private ItemUpgrade DetermineQuality()
        {
            return (ItemUpgrade)_monster.LootManager.Drop(_monster.UpgradeTable, 1).FirstOrDefault();
        }

        private void DetermineRandomDrop()
        {
            var idx = 0;
            if (_monster.Template.Drops.Count > 0)
                idx = Generator.Random.Next(_monster.Template.Drops.Count);

            var rndSelector = _monster.Template.Drops[idx];
            if (!ServerContext.GlobalItemTemplateCache.ContainsKey(rndSelector))
                return;

            var item = Item.Create(_monster, ServerContext.GlobalItemTemplateCache[rndSelector], true);
            var chance = Math.Round(Generator.Random.NextDouble(), 2);

            if (chance <= item.Template.DropRate)
                item.Release(_monster, _monster.Position);
        }

        private Item.Variance DetermineVariance()
        {
            return Generator.RandomEnumValue<Item.Variance>();
        }

        private void GenerateDrops()
        {
            if (_monster.Template.LootType.HasFlag(LootQualifer.Table))
            {
                if (_monster.LootTable == null || _monster.LootManager == null)
                    return;

                DetermineDrop().ForEach(i =>
                {
                    if (i == null)
                        return;
                    if (!ServerContext.GlobalItemTemplateCache.ContainsKey(i))
                        return;

                    var rolledItem = Item.Create(_monster, ServerContext.GlobalItemTemplateCache[i]);

                    if (rolledItem == _monster.GlobalLastItemRoll)
                        return;

                    _monster.GlobalLastItemRoll = rolledItem;

                    var upgrade = DetermineQuality();

                    if (rolledItem.Template.Enchantable)
                    {
                        var variance = DetermineVariance();

                        if (!ServerContext.Config.UseLoruleVariants)
                            variance = Item.Variance.None;

                        if (variance != Item.Variance.None)
                            rolledItem.ItemVariance = variance;
                    }

                    if (rolledItem.Template.Flags.HasFlag(ItemFlags.QuestRelated))
                        upgrade = null;

                    if (!ServerContext.Config.UseLoruleItemRarity)
                        upgrade = null;

                    rolledItem.Upgrades = upgrade?.Upgrade ?? 0;

                    if (rolledItem.Upgrades > 0)
                    {
                        Item.ApplyQuality(rolledItem);

                        if (rolledItem.Upgrades > 2)
                            if (_monster.Target != null)
                            {
                                var user = _monster.Target;

                                if (user is Aisling aisling)
                                {
                                    // 그룹이 없으면 GroupParty 가 null 이다 — 그때는 잡은 사람 혼자 듣는다.
                                    var party = aisling.GroupParty?.PartyMembers ?? new List<Aisling> { aisling };

                                    foreach (var player in party)
                                        player.Client.SendMessage(0x03,
                                            $"귀한 물건이 떨어졌습니다: {rolledItem.DisplayName}");

                                    Task.Delay(1000).ContinueWith(ct => { rolledItem.Animate(160, 200); });
                                }
                            }
                    }

                    rolledItem.Cursed = true;
                    rolledItem.AuthenticatedAislings = _monster.GetTaggedAislings().Cast<Sprite>().ToArray();
                    rolledItem.Release(_monster, _monster.Position);
                });
            }
            else if (_monster.Template.LootType.HasFlag(LootQualifer.Random))
            {
                DetermineRandomDrop();
            }
        }

        /// <summary>
        /// 저보다 한참 낮은 괴물은 경험치를 거의 주지 않는다. 다섯 레벨까지는 그대로 주고, 그 뒤로는 다섯
        /// 레벨마다 반으로 줄여 2% 에서 멈춘다 — 24레벨이 1레벨 괴물을 잡으면 1,100 이 아니라 90 남짓이다.
        /// </summary>
        /// <remarks>
        /// **이 값은 우리가 정한 것이다.** 원작 표에도 5.99 팩에도 레벨 차이로 경험치를 깎는 규칙은 없었다
        /// (2026-09-18 찾아봄). 없이 두었더니 24레벨이 노비스평원에서 한 대에 1~7 맞으며 한 마리에 1,100 씩
        /// 벌고 있었다(사용자). 근거가 나오면 그 값으로 바꾼다.
        /// </remarks>
        private double ForLevel(Aisling player, double exp)
        {
            var gap = player.ExpLevel - _monster.Template.Level;

            if (gap <= Forgiven)
                return exp;

            var share = Math.Pow(0.5, (gap - Forgiven) / (double) Halving);

            return exp * Math.Max(Least, share);
        }

        /// <summary>몇 레벨 차이까지는 깎지 않나.</summary>
        private const int Forgiven = 5;

        /// <summary>그 뒤로 몇 레벨마다 반으로 줄이나.</summary>
        private const int Halving = 5;

        /// <summary>아무리 낮아도 이만큼은 준다 — 0 이면 잡을 까닭이 아예 없어진다.</summary>
        private const double Least = 0.02;

        public void DistributeExperience(Aisling player, double exp)
        {
            exp = ForLevel(player, exp);

            var chunks = exp / 1000;

            if (chunks <= 1)
                HandleExp(player, exp);
            else
                for (var i = 0; i < chunks; i++)
                    HandleExp(player, 1000);
        }

        private void GenerateExperience(Aisling player, bool canCrit = false)
        {
            int exp;

            // 정의가 경험치를 적어 두면 그것이 답이다. 안 적어 둔 괴물은 그대로 레벨에서 나온다.
            if (_monster.Template.Exp is { } stated)
            {
                exp = stated;
            }
            else
            {
                var seed = _monster.Template.Level * 0.1 + 1.5;
                exp = (int)(_monster.Template.Level * seed * 300);
            }

            if (canCrit)
                lock (Generator.Random)
                {
                    var critical = Math.Abs(Generator.GenerateNumber() % 100);
                    if (critical >= 85) exp *= 2;
                }

            DistributeExperience(player, exp);

            if (player.PartyMembers != null)
                foreach (var party in player.PartyMembers
                    .Where(party => party.Serial != player.Serial)
                    .Where(party => party.WithinRangeOf(player)))
                {
                    DistributeExperience(party, exp);

                    party.Client.SendStats(StatusFlags.StructC);
                    party.Client.SendMessage(0x02, $"경험치가 {exp} 올랐습니다");
                }

            player.Client.SendStats(StatusFlags.StructC);
            player.Client.SendMessage(0x02, $"경험치가 {exp} 올랐습니다");
        }

        /// <summary>
        /// 정의가 금화를 적어 두면 그것이 답이다 — 5.99 의 <c>골드 &lt;액수&gt; &lt;확률%&gt;</c> 를 그대로 옮겨
        /// 적었다(<c>scripts/build-pack-gold.py</c>). 안 적어 둔 괴물만 레벨에서 나온다.
        /// </summary>
        /// <remarks>
        /// 레벨 쪽 식은 손대지 않았지만 지금 세상에는 거의 쓰이지 않는다. 하데스의 정의 568개가 모두
        /// <c>Level 1</c> 이라, 그 길로 가면 세상의 모든 괴물이 한 마리에 500~999 전을 냈다 — 레더튜닉이
        /// 300전이고 5.99 는 같은 노비스 괴물에게 스무 전을 셋에 하나꼴로 준다.
        /// </remarks>
        private void GenerateGold()
        {
            if (!_monster.Template.LootType.HasFlag(LootQualifer.Gold))
                return;

            int sum;

            if (_monster.Template.Gold is { } stated)
            {
                var chance = _monster.Template.GoldChance ?? 100;

                if (chance <= 0 || Generator.Random.Next(100) >= chance)
                    return;

                sum = stated;
            }
            else
            {
                sum = Generator.Random.Next(
                    _monster.Template.Level * 500,
                    _monster.Template.Level * 1000);
            }

            if (sum > 0)
                Money.Create(_monster, sum, new Position(_monster.XPos, _monster.YPos));
        }

        private void UpdateCounters(Aisling player)
        {
            if (!player.MonsterKillCounters.ContainsKey(_monster.Template.Name))
                player.MonsterKillCounters[_monster.Template.Name] = new KillRecord()
                {
                    MonsterLevel = _monster.Template.Level,
                    TimeKilled = DateTime.UtcNow,
                    TotalKills = 1
                };
            else
                player.MonsterKillCounters[_monster.Template.Name].TotalKills++;
        }
    }
}
