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

                // 레벨업 식은 서버 한 곳(Monster.Levelup — 원작 콘+30·위즈+25)을 쓴다. 여기 따로 적힌 하데스 식을 치웠다(2026-09-25).
                Darkages.Types.Monster.Levelup(player);
            }
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
        /// 경험치 한 점당 금화. 원작에도 하데스에도 "몬스터 레벨"이라는 값이 없어(2026-09-24 조사 —
        /// 서버팩 3개·원작 아카이브·참고저장소 16개 어디에도 몬스터 레벨 필드가 없다) 레벨 대신 **경험치에
        /// 비례**시킨다. 노비스 괴물 11마리의 경험치(1,068~1,849)와 지금 금화(20~30)를 나눠 보면 비율이
        /// 0.0162~0.0247 사이(평균 0.0187)였다 — 그 폭 가운데 값으로 0.02 를 골라, 노비스 대부분(경험치
        /// 1,068~1,301)이 새 식에서도 20~31전으로 지금 폭과 거의 겹치게 했다(경험치가 큰 지네·독거미
        /// 1,781~1,849 만 32~44전으로 조금 올라간다).
        /// </summary>
        private const double GoldPerExp = 0.02;

        /// <summary>무작위 폭 — 사용자가 정한 ±20%.</summary>
        private const double GoldVariance = 0.2;

        /// <summary>
        /// 경험치와 같은 값 — <see cref="GenerateExperience"/> 가 쓰는 "정의에 적힌 값, 없으면 레벨식"
        /// 을 그대로 되풀이한다(그쪽 메서드는 손대지 않는다 — 실제 지급 경험치가 바뀌면 안 되므로).
        /// </summary>
        private int MonsterExp()
        {
            if (_monster.Template.Exp is { } stated)
                return stated;

            var seed = _monster.Template.Level * 0.1 + 1.5;
            return (int)(_monster.Template.Level * seed * 300);
        }

        /// <summary>
        /// 몬스터를 잡으면 금화는 <b>무조건</b> 떨어진다(사용자 결정, 2026-09-24). <c>LootType</c> 의
        /// Gold 플래그와 <c>GoldChance</c> 는 더는 보지 않는다 — 있어도 없어도 항상 준다.
        /// </summary>
        /// <remarks>
        /// <b>레벨 기반 최저금액 분기를 걷어내고 경험치 비례식으로 바꿨다(사용자 결정, 2026-09-24).</b>
        /// 금화 = 경험치 × <see cref="GoldPerExp"/> × (0.8~1.2 무작위). 템플릿의 <c>Gold</c>·
        /// <c>GoldChance</c> 칸은 더 이상 읽지 않지만 자료에서 지우지는 않았다(생성기는 더하고 고치기만).
        /// </remarks>
        private void GenerateGold()
        {
            var baseline = MonsterExp() * GoldPerExp;
            var factor = 1 + (Generator.Random.NextDouble() * 2 - 1) * GoldVariance;
            var sum = (int)Math.Round(baseline * factor);

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
