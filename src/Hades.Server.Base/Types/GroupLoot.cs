using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Common;
using Darkages.Network.ServerFormats;
using Darkages.Storage;

namespace Darkages.Types
{
    /// <summary>
    /// 그룹 전리품(2026-10-07, 설계 <c>autopilot/loot-auction/</c>) — 그룹 처치의 장비·강화·변형 물건은 룰렛, 나머지는 차례로 돌리고,
    /// 금화는 똑같이 나눠 바로 준다. 나눌 사람이 둘 못 되면 false — 드롭 스크립트가 지금처럼 바닥에 떨군다.
    /// </summary>
    /// <remarks>
    /// 나눌 사람 = 처치한 이의 그룹원 중 같은 맵 · 살아 있음 · 로그인 · 동료 봇 아님(SPEC S-1). 동료 봇은 주인 그룹에 들어가므로
    /// 빼지 않으면 사람이 혼자 사냥해도 전리품 절반이 봇에게 간다.
    /// </remarks>
    public static class GroupLoot
    {
        /// <summary>와우 /roll 1-100 (03 상수 표).</summary>
        public const int ROLL_MAX = 100;

        private static List<Aisling> Sharers(Aisling killer)
        {
            var party = killer?.GroupParty;
            if (!ServerContext.Config.GroupLootRoll || party == null)
                return new List<Aisling>();

            var sharers = party.PartyMembers
                .Where(member => member.LoggedIn && member.Client != null && member.CurrentMapId == killer.CurrentMapId && !member.IsDead()
                                 && !Companions.IsBot(member.Username))
                .OrderBy(member => member.Username.ToLowerInvariant(), StringComparer.Ordinal)
                .ToList();
            return sharers.Count >= 2 ? sharers : new List<Aisling>();
        }

        /// <summary>룰렛을 돌리는 물건(03 용어) — 장비 · 강화 · 변형.</summary>
        public static bool IsRolled(Item item) =>
            item.Template.Flags.HasFlag(ItemFlags.Equipable) || item.Upgrades > 0 || item.ItemVariance != Item.Variance.None;

        /// <summary>물건 하나를 나눈다. 받을 이의 가방이 차면 그 사람 발밑에 그 사람만 주울 수 있게 떨군다.</summary>
        public static bool Share(Aisling killer, Item item, Sprite source)
        {
            var sharers = Sharers(killer);
            if (sharers.Count == 0)
                return false;

            var winner = IsRolled(item) ? Roll(sharers, item) : Turn(killer.GroupParty, sharers);
            bool given;
            // 받는 이의 캐릭터 자물쇠 안에서 — 그 사람이 제 접속에서 경매에 올리는 동안 같은 묶음을 고치지 않게(AuctionHouse.AsLive).
            lock (AislingStorage.LockFor(winner.Username))
                given = winner.LoggedIn && item.GiveTo(winner);

            if (!given)
            {
                item.Cursed = true;
                item.AuthenticatedAislings = new Sprite[] { winner };
                item.Release(source, winner.Position);
            }

            return true;
        }

        private static Aisling Roll(List<Aisling> sharers, Item item)
        {
            List<(Aisling Who, int Roll)> first = null;
            var all = new List<object>();
            var standing = sharers;
            while (true)
            {
                List<(Aisling Who, int Roll)> rolls;
                lock (Generator.Random)
                    rolls = standing.Select(member => (member, Generator.Random.Next(1, ROLL_MAX + 1))).ToList();

                first ??= rolls;
                all.AddRange(rolls.Select(one => new { name = one.Who.Username, roll = one.Roll }));
                int top = rolls.Max(one => one.Roll);
                standing = rolls.Where(one => one.Roll == top).Select(one => one.Who).ToList();
                if (standing.Count == 1)
                    break;
            }

            var winner = standing[0];
            int shown = first.First(one => one.Who == winner).Roll;
            var told = ServerFormat5E.Of(ServerFormat5E.Roll, writer =>
            {
                writer.Write(item.DisplayImage);
                writer.Write(item.Color);
                writer.WriteStringA(item.DisplayName);
                writer.Write((byte) first.Count);
                foreach (var (who, roll) in first)
                {
                    writer.Write((uint) who.Serial);
                    writer.WriteStringA(who.Username);
                    writer.Write((byte) roll);
                }

                writer.Write((uint) winner.Serial);
            });

            foreach (var member in sharers)
            {
                member.Client?.Send(told);
                member.Client?.SendMessage(0x03, $"{item.DisplayName}: {winner.Username} ({shown})");
            }

            AuctionHouse.Record("roll", winner.Username, item.DisplayName, 0, new { rolls = all });
            return winner;
        }

        private static Aisling Turn(Party party, List<Aisling> sharers)
        {
            string key = string.Join("|", sharers.Select(member => member.Username.ToLowerInvariant()));
            lock (party)
            {
                if (party.LootTurnKey != key)
                {
                    party.LootTurnKey = key;
                    party.LootTurn = 0;
                }

                return sharers[party.LootTurn++ % sharers.Count];
            }
        }

        /// <summary>금화를 똑같이 — 나머지는 처치한 이. 들 수 있는 금화를 넘는 몫은 그 사람의 경매장 받을 것으로(까닭 5).</summary>
        public static bool ShareGold(Aisling killer, int amount)
        {
            var sharers = Sharers(killer);
            if (sharers.Count == 0 || amount <= 0)
                return false;

            int share = amount / sharers.Count;
            var extra = sharers.Contains(killer) ? killer : sharers[0];
            foreach (var member in sharers)
            {
                long mine = share + (member == extra ? amount - share * sharers.Count : 0);
                long given;
                // 캐릭터 자물쇠 안에서 더한다 — 경매 지불·받기와 섞여 한쪽이 지워지지 않게. 경매장(받을 것)은 자물쇠를 놓은 뒤에.
                lock (AislingStorage.LockFor(member.Username))
                {
                    given = Math.Min(mine, Math.Max(0, (long) ServerContext.Config.MaxCarryGold - member.GoldPoints));
                    if (given > 0)
                        member.GoldPoints += (int) given;
                }

                if (given > 0)
                {
                    member.Client?.SendStats(StatusFlags.StructC);
                    member.Client?.SendMessage(0x03, $"금전 {given}전을 나눠 받았습니다.");
                }

                if (mine > given)
                    AuctionHouse.AddGoldClaim(member.Username, mine - given, new { amount, sharers = sharers.Select(one => one.Username) });
            }

            return true;
        }
    }
}
