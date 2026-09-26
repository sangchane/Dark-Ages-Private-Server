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

        /// <summary>
        /// 경험치를 더하고, 실제로 더한 값을 돌려준다 — 알림은 이 값을 적는다(아래 <see cref="GenerateExperience"/>).
        /// </summary>
        /// <remarks>
        /// 레벨이 오를 때 남는 몫은 다음 레벨로 넘긴다. 예전에는 <c>ExpNext</c> 를 uint 로 빼서 음수가 되면 0 으로
        /// 자르고 새 목표를 통째로 줘, 넘친 몫이 사라졌다(2026-09-25).
        /// </remarks>
        private uint HandleExp(Aisling player, double exp)
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

            var given = (uint)exp;
            var left = given;

            player.ExpTotal = player.ExpTotal + given < player.ExpTotal ? uint.MaxValue : player.ExpTotal + given;

            while (left >= player.ExpNext
                   && player.ExpLevel < 99
                   && player.ExpLevel < ServerContext.Config.PlayerLevelCap)
            {
                left -= player.ExpNext;

                // 지금 ExpLevel 은 아직 오르기 전의 값이다. 아래에서 Levelup 이 하나 올리므로, 그 뒤에
                // 사람이 바라볼 다음 목표는 (지금+2) 레벨에 닿는 값이다.
                player.ExpNext = Math.Max(1, ExperienceCurve.ToReach(player.ExpLevel + 2));

                // 레벨업 식은 서버 한 곳(Monster.Levelup — 원작 콘+30·위즈+25)을 쓴다. 여기 따로 적힌 하데스 식을 치웠다(2026-09-25).
                Darkages.Types.Monster.Levelup(player);
            }

            player.ExpNext = left >= player.ExpNext ? 0 : player.ExpNext - left;

            return given;
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

        /// <summary>
        /// 한 마리가 떨굴 물건 하나. 목록의 <c>DropRate</c> 를 한 줄로 이어 붙이고(전체 길이 = 목록 칸수) 그 위의
        /// 한 점을 뽑는다 — 한 물건이 나올 확률은 그대로 <c>DropRate ÷ 칸수</c> 다. 옛 셈(한 칸을 고르고 그 칸을
        /// 굴린다)과 1 이하에서는 같은 확률이지만, 옛 셈에서는 1 을 넘는 값이 1 처럼 굴었다. 마력 포션을 두 배로
        /// 올리며(2026-09-26, 0.6 → 1.2) 바꿨다.
        /// </summary>
        private void DetermineRandomDrop()
        {
            var drops = _monster.Template.Drops;
            if (drops == null || drops.Count == 0)
                return;

            var point = Generator.Random.NextDouble() * drops.Count;

            foreach (var name in drops)
            {
                if (name == null || !ServerContext.GlobalItemTemplateCache.TryGetValue(name, out var template))
                    continue;

                if (point < template.DropRate)
                {
                    var item = Item.Create(_monster, template, true);
                    item.Stacks = BundleSize(item);
                    item.Release(_monster, _monster.Position);
                    return;
                }

                point -= Math.Max(0, template.DropRate);
            }
        }

        /// <summary>
        /// 겹쳐지는 소모품(포션·시약)은 1~3개가 한 묶음으로 떨어진다(2026-09-26, 사용자 "번들이니까 여러 개도").
        /// 나머지는 하나 — 0 은 줍기(<c>Item.GiveTo</c>)가 1 로 센다.
        /// </summary>
        private static ushort BundleSize(Item item) =>
            item.Template.Flags.HasFlag(ItemFlags.Consumable) && item.Template.Flags.HasFlag(ItemFlags.Stackable)
                ? (ushort)Generator.Random.Next(1, 4)
                : (ushort)1;

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

                    rolledItem.Stacks = BundleSize(rolledItem);
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
            var gap = player.ExpLevel - CutLevel(MonsterExp(), _monster.CurrentMapId);

            if (gap <= Forgiven)
                return exp;

            var share = Math.Pow(0.5, (gap - Forgiven) / (double) Halving);

            return exp * Math.Max(Least, share);
        }

        /// <summary>
        /// 깎기에 쓰는 괴물 레벨 — 괴물의 경험치에서 추정한다(사용자 결정 2026-09-25: "입장 레벨 생각해서 경험치량으로
        /// 비교해 봐"). 괴물 정의 568개가 모두 <c>Level</c> 1 이라 정의의 레벨로 깎으면 7레벨부터 어디서든 깎였다.
        /// 이 값은 <b>깎기에만</b> 쓴다 — 체력·능력치를 만드는 <c>Template.Level</c> 은 그대로다.
        /// </summary>
        /// <remarks>
        /// 입장 레벨이 워프 레벨문으로 적힌 사냥터 셋(노비스 1~22 · 포테의숲 21~51 · 아벨해안 51~80)에서 존(맵)마다 대표
        /// 경험치(SpawnMax 무게 기하평균)를 구해, 사냥터 안에서 경험치 순으로 범위에 펼친 것이 기준점이다(사용자: "존마다
        /// 몬스터 레벨 차이가 좀 날 거야, 경험치량이랑"). 기준점 사이는 ln(경험치) 위의 꺾은선, 양 끝 밖은 첫 점과 끝 점을
        /// 잇는 기울기로 뻗고 1~99 로 자른다. 우드랜드는 입장 레벨 구간(<c>CutWoodland</c>)마다 따로. 방법·대조표는 <c>scripts/build-monster-cut-level.py</c> — 값은 그 생성기가 아래 칸에 쓴다.
        /// </remarks>
        private static int CutLevel(double exp, int mapId)
        {
            var x = Math.Log(Math.Max(exp, 1));

            // 우드랜드는 제 구간 안에서 따로 — 가장 낮은 경험치 = 아래 끝, 가장 높은 = 위 끝(사용자 2026-09-26: "우드랜드도
            // 존별로 차이가 많이 나"). 같은 입장 레벨에서 경험치가 다른 사냥터의 3~4배 적어 위의 한 줄 대응에 못 넣는다.
            foreach (var (maps, lowExp, highExp, low, high) in CutWoodland)
            {
                if (Array.IndexOf(maps, mapId) < 0)
                    continue;

                var within = highExp <= lowExp
                    ? low
                    : low + (x - Math.Log(lowExp)) * (high - low) / (Math.Log(highExp) - Math.Log(lowExp));

                return Math.Clamp((int)Math.Round(within, MidpointRounding.ToEven), low, high);
            }
            var last = CutExp.Length - 1;
            int from = 0, to = last;

            if (x > Math.Log(CutExp[0]) && x < Math.Log(CutExp[last]))
            {
                to = 1;
                while (x > Math.Log(CutExp[to]))
                    to++;
                from = to - 1;
            }

            var span = Math.Log(CutExp[to]) - Math.Log(CutExp[from]);
            var value = span <= 0
                ? CutLevels[to]
                : CutLevels[from] + (x - Math.Log(CutExp[from])) * (CutLevels[to] - CutLevels[from]) / span;

            return Math.Clamp((int)Math.Round(value, MidpointRounding.ToEven), 1, 99);
        }

        // <cut-level> scripts/build-monster-cut-level.py 가 쓴다 — 손으로 고치지 말고 생성기를 다시 돌린다.
        // 노비스(1~22)·포테의숲(21~51)·아벨해안(51~80) 존마다의 대표 경험치 → 레벨, 경험치 순.
        private static readonly double[] CutExp = { 1133, 1133, 1191, 1191, 1638, 1638, 1638, 1638, 1706, 7552, 7821, 8437, 10710, 12323, 12798, 38664, 38664, 43645, 43645, 43645, 50132, 50132, 50132, 51011, 54594, 54594 };
        private static readonly double[] CutLevels = { 1.0, 1.0, 3.55, 3.55, 19.93, 19.93, 19.93, 19.93, 22.0, 22.0, 22.99, 27.3, 40.87, 48.85, 51.0, 51.0, 51.0, 61.18, 61.18, 61.18, 72.83, 72.83, 72.83, 74.29, 80.0, 80.0 };
        // 우드랜드 구간(맵 번호들, 가장 낮은·높은 경험치, 아래·위 레벨) — 5.99 WoodLand_Warp 입장 레벨.
        private static readonly (int[] Maps, double LowExp, double HighExp, int Low, int High)[] CutWoodland =
        {
            (new[] { 20015, 20016, 20017 }, 466, 575, 1, 10), // 우드랜드1-1 · 우드랜드1-2 · 우드랜드1-3
            (new[] { 20022 }, 452, 589, 11, 20), // 우드랜드2-1
            (new[] { 20023, 20024 }, 2466, 2877, 21, 50), // 우드랜드3-1 · 우드랜드4-1
            (new[] { 20025, 20026, 20027, 20018, 20019 }, 11164, 12329, 51, 80), // 우드랜드5-1 · 우드랜드6-1 · 우드랜드6-1(진) · 우드랜드10-1 · 우드랜드11-1
            (new[] { 20020, 20021 }, 31507, 32192, 81, 99), // 우드랜드14-1 · 우드랜드14-1(진)
        };
        // </cut-level>

        /// <summary>몇 레벨 차이까지는 깎지 않나.</summary>
        private const int Forgiven = 5;

        /// <summary>그 뒤로 몇 레벨마다 반으로 줄이나.</summary>
        private const int Halving = 5;

        /// <summary>아무리 낮아도 이만큼은 준다 — 0 이면 잡을 까닭이 아예 없어진다.</summary>
        private const double Least = 0.02;

        /// <returns>실제로 더한 경험치 — 레벨 차이로 깎고 그룹 몫을 더한 뒤의 값.</returns>
        /// <remarks>
        /// 예전에는 1,000 이 넘으면 1,000 씩 나눠 주면서 끝수를 올려(1,500 → 2,000) 알림과 달랐다. 레벨업 때 넘친 몫을
        /// 이제 <see cref="HandleExp"/> 가 넘기므로 나눌 까닭이 없다(2026-09-25).
        /// </remarks>
        public uint DistributeExperience(Aisling player, double exp) => HandleExp(player, ForLevel(player, exp));

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

            // 알림에는 괴물 정의의 값이 아니라 실제로 더한 값을 적는다 — 레벨 차이로 깎인 뒤다(사용자 2026-09-25:
            // "경험치가 표시되는 것만큼 줄지 않는 것 같은데?" — 20레벨이 우드랜드에서 알림 466, 실제 66).
            var mine = DistributeExperience(player, exp);

            if (player.PartyMembers != null)
                foreach (var party in player.PartyMembers
                    .Where(party => party.Serial != player.Serial)
                    .Where(party => party.WithinRangeOf(player)))
                {
                    var shared = DistributeExperience(party, exp);

                    party.Client.SendStats(StatusFlags.StructC);
                    party.Client.SendMessage(0x02, $"경험치가 {shared} 올랐습니다");
                }

            player.Client.SendStats(StatusFlags.StructC);
            player.Client.SendMessage(0x02, $"경험치가 {mine} 올랐습니다");
        }

        /// <summary>
        /// 경험치 한 점당 금화. 원작에도 하데스에도 "몬스터 레벨"이라는 값이 없어(2026-09-24 조사 —
        /// 서버팩 3개·원작 아카이브·참고저장소 16개 어디에도 몬스터 레벨 필드가 없다) 레벨 대신 **경험치에
        /// 비례**시킨다. 노비스 괴물 11마리의 경험치(1,068~1,849)와 지금 금화(20~30)를 나눠 보면 비율이
        /// 0.0162~0.0247 사이(평균 0.0187)였다 — 그 폭 가운데 값으로 0.02 를 골라, 노비스 대부분(경험치
        /// 1,068~1,301)이 새 식에서도 20~31전으로 지금 폭과 거의 겹치게 했다(경험치가 큰 지네·독거미
        /// 1,781~1,849 만 32~44전으로 조금 올라간다). 그 0.02 는 이제 노비스에만 쓰고(<see cref="NoviceGoldPerExp"/>),
        /// 노비스 밖은 그 다섯 배인 0.1 이다(사용자 2026-09-25).
        /// </summary>
        private const double GoldPerExp = 0.1;

        /// <summary>
        /// 노비스는 그대로 경험치 한 점당 0.02 — 위의 0.1 은 그 다섯 배다(사용자 2026-09-25: "포테 3존인데 47원씩
        /// 들어오는데 금전이 너무 적다" → 경험치×0.1, 단 노비스는 지금 금액 그대로).
        /// </summary>
        private const double NoviceGoldPerExp = 0.02;

        /// <summary>
        /// 노비스 맵 번호 — <c>database/server/areas/</c> 에서 이름이 "노비스"로 시작하는 맵 전부:
        /// 20083 노비스1 · 20084 노비스던전1 · 20085 노비스사냥터1 · 20086 노비스상점1 ·
        /// 20373 노비스마을 · 20374~20379 노비스 마을 안 건물(식당·무기방어구상점·민가1·민가2·잡화상점·주점) ·
        /// 20380~20388 노비스지하던전 a1~c3 · 20389~20392 노비스지하동굴 a~d · 20393·20394 노비스평원 a·b.
        /// 괴물이 서는 곳은 지하던전(a1~b3·c3)과 평원 a·b 다(2026-09-25 templates/monsters 의 AreaID 로 셈).
        /// </summary>
        private static bool IsNovice(int mapId) =>
            mapId is >= 20083 and <= 20086 or >= 20373 and <= 20394;

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
            var perExp = IsNovice(_monster.CurrentMapId) ? NoviceGoldPerExp : GoldPerExp;
            var baseline = MonsterExp() * perExp;
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
