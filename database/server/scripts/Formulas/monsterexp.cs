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
            var gap = player.ExpLevel - HuntingGroundLevel(_monster.CurrentMapId, _monster.Template.Level);

            if (gap <= Forgiven)
                return exp;

            var share = Math.Pow(0.5, (gap - Forgiven) / (double) Halving);

            return exp * Math.Max(Least, share);
        }

        /// <summary>
        /// 깎기에 쓰는 괴물 레벨 — 그 괴물이 사는 사냥터의 입장 레벨 범위의 <b>위쪽 끝</b>(사용자 결정 2026-09-25).
        /// 괴물 정의 568개 가운데 567개가 <c>Level</c> 1 이라(나머지 하나도 1), 정의의 레벨로 깎으면 7레벨부터 어디서든
        /// 깎였다. 이 값은 <b>깎기에만</b> 쓴다 — 체력·능력치를 만드는 <c>Template.Level</c> 은 그대로다.
        /// </summary>
        /// <remarks>
        /// 근거는 워프의 레벨문(5.99 <c>warp/*.txt</c> 줄 끝 두 칸 = 최소·최대, 추출본
        /// <c>data/server-packs/extracted/5.99-server/warps.json</c>의 <c>raw</c>):
        /// <list type="bullet">
        /// <item>노비스 22 — 노비스마을→평원A·B 1~22, 평원→지하던전 5~22, 지하던전끼리 10~22(<c>Novice_Warp</c>, 하데스 워프도 같다).</item>
        /// <item>포테의숲 1~6존 51 — 하데스 워프 템플릿 수오미마을→1존·존끼리 21~51(5.99 map_create 사본). 5.99 에는 5존→오솔길 21~52 한 줄뿐.</item>
        /// <item>아벨해안 1-A~4-C 80 — 5.99 <c>Abel_Warp</c> 가 해안 안의 모든 문에 51~80. 하데스는 입구에만 남겼으므로 5.99 원본을 쓴다.</item>
        /// <item>우드랜드 2~6·14 99 — 5.99 <c>WoodLand_Warp</c> 가 입구에서 11·21·21·51·51·81 이상만 묻고 위쪽은 99 다.</item>
        /// <item>신죽음의마을·신죽마집안·죽음의마을 99 — 5.99 문이 모두 99~99.</item>
        /// </list>
        /// 레벨문이 없는 곳(우드랜드1-1~1-3·드라큐라백작의성·마운틴메리·지하수로D·카스마늄 갱도·승급던젼)은 정의의 레벨을 그대로 쓴다.
        /// </remarks>
        private static int HuntingGroundLevel(int mapId, int stated) => mapId switch
        {
            _ when IsNovice(mapId) => 22,
            >= 20263 and <= 20268 => 51,  // 포테의숲1존~6존
            >= 20584 and <= 20594 => 80,  // 아벨해안1-A~4-C
            20020 => 99,                  // 우드랜드14-1
            >= 20022 and <= 20026 => 99,  // 우드랜드2-1~6-1
            >= 20657 and <= 20682 => 99,  // 신죽마집안·신죽음의마을·죽음의마을
            _ => stated
        };

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
