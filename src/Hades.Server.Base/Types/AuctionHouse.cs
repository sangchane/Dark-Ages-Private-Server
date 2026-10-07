using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Darkages.Common;
using Darkages.Network;
using Darkages.Network.ClientFormats;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Darkages.Types
{
    /// <summary>
    /// 경매장(와우 방식, 2026-10-07) — 설계 <c>autopilot/loot-auction/</c>(03 요구사항·05 계약·SPEC 결정 S-1~S-12).
    /// 우편 대신 「받을 것」: 낙찰품·판매 대금·유찰품·밀린 입찰금·취소품·넘친 나눔 금화를 캐릭터 이름으로 맡아 둔다.
    /// </summary>
    /// <remarks>
    /// 캐릭터 파일과 경매장 파일은 따로 쓰여 한꺼번에 쓸 수 없다. 그래서 순서를 「사건 한 줄(flush) → 내주는 쪽 저장 → 받는 쪽 저장
    /// → commit 한 줄」로 고정해, 어디서 꺼져도 물건·금화가 두 곳에 생기지는 않는다(INV-3). 그 사이에 꺼져 생긴 유실은 commit 도
    /// abort 도 없는 seq 줄로 운영자가 되살린다(07 R1). 내주는 쪽 저장이 실패하면 메모리를 되돌리고 거절한다.
    /// 자물쇠는 <see cref="Gate" /> 하나 — 그 안에서는 다른 자물쇠를 잡지 않고, 알림은 풀고 나서 보낸다.
    /// </remarks>
    public static class AuctionHouse
    {
        // 03 상수 표 — 근거는 03(와우 출처 URL 또는 설계 결정 DL-5).
        public static readonly int[] AUCTION_DURATIONS = { 12, 24, 48 };
        // 보증금은 시작가의 % (사용자 2026-10-07 — 와우처럼 상인 값 15/30/60% 면 5억 물건이 하루 9천만이라 아무도 안 올린다, DL-14).
        public static readonly int[] AUCTION_DEPOSIT_RATE = { 1, 2, 4 };
        // 값(시작가·즉시 구매가·입찰가) 상한 — 들 수 있는 금화(MaxCarryGold)와 따로. 모자라면 은행 금화로 낸다(DL-14). int 안.
        public const int AUCTION_MAX_PRICE = 2_000_000_000;
        public const int AUCTION_CUT = 5;
        public const int AUCTION_MIN_STEP = 5;
        public const int AUCTION_MAX_LISTINGS = 20;
        public const int AUCTION_PAGE = 20;
        public static readonly TimeSpan AUCTION_TICK = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan RequestGap = TimeSpan.FromMilliseconds(300);

        public const int ClaimItem = 0, ClaimGold = 1;
        public const int ForWon = 0, ForSale = 1, ForExpired = 2, ForOutbid = 3, ForCancel = 4, ForSplit = 5;

        public sealed class Listing
        {
            public long Id { get; set; }
            public string Seller { get; set; }
            public Item Item { get; set; }
            public int StartBid { get; set; }
            public int Buyout { get; set; }
            public int Bid { get; set; }
            public string Bidder { get; set; }
            public int Deposit { get; set; }
            public int Hours { get; set; }
            public DateTime PostedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
        }

        public sealed class Claim
        {
            public long Id { get; set; }
            public string Owner { get; set; }
            public int Kind { get; set; }
            public Item Item { get; set; }
            public long Gold { get; set; }
            public int Reason { get; set; }
            public long ListingId { get; set; }
            public DateTime At { get; set; }
        }

        private sealed class Book
        {
            public long NextId { get; set; } = 1;
            public long LastSeq { get; set; }
            public List<Listing> Listings { get; set; } = new List<Listing>();
            public List<Claim> Claims { get; set; } = new List<Claim>();
        }

        private static readonly object Gate = new object();
        private static Book _book = new Book();
        private static bool _loaded;
        private static DateTime _lastExpire = DateTime.MinValue;

        public static Func<string> FolderSource = () => Path.Combine(ServerContext.StoragePath, "auction");
        private static string Folder => FolderSource();
        private static string BookPath => Path.Combine(Folder, "auction.json");

        private static bool Same(string a, string b) => string.Equals(a, b, StringComparison.OrdinalIgnoreCase);

        // ---- 불러오기 · 저장 · 사건 기록 ----

        /// <summary>서버 시작 때 한 번. 읽지 못하면 경매장을 닫는다(게임은 뜬다, 07 R2).</summary>
        public static void Load()
        {
            lock (Gate)
            {
                try
                {
                    Directory.CreateDirectory(Folder);
                    string text = SafeFile.Read(BookPath, content => Parse(content) != null);
                    bool none = text == null && !File.Exists(BookPath) && !File.Exists(SafeFile.BackupPath(BookPath));
                    _book = none ? new Book() : Parse(text) ?? throw new InvalidDataException("auction.json 을 읽을 수 없습니다.");
                    _book.LastSeq = Math.Max(_book.LastSeq, LastLoggedSeq());
                    _loaded = true;
                }
                catch (Exception e)
                {
                    _loaded = false;
                    ServerContext.Error(e);
                    ServerContext.Logger("auction load failed — 경매장을 닫습니다", Microsoft.Extensions.Logging.LogLevel.Error);
                }
            }
        }

        private static Book Parse(string text)
        {
            try
            {
                return string.IsNullOrWhiteSpace(text) ? null : JsonConvert.DeserializeObject<Book>(text, StorageManager.Settings);
            }
            catch (Exception)
            {
                return null;
            }
        }

        // 경매장 파일보다 사건 기록이 먼저 쓰인다 — 꺼지기 직전 조작의 seq 를 다시 쓰지 않게 가장 새 기록 파일에서 큰 수를 찾는다.
        private static long LastLoggedSeq()
        {
            string newest = Directory.GetFiles(Folder, "events-*.jsonl").OrderBy(path => path, StringComparer.Ordinal).LastOrDefault();
            if (newest == null)
                return 0;

            long seq = 0;
            foreach (string line in File.ReadLines(newest))
            {
                try
                {
                    seq = Math.Max(seq, JObject.Parse(line).Value<long?>("seq") ?? 0);
                }
                catch (JsonException)
                {
                    // 꺼지며 반만 쓰인 줄
                }
            }

            return seq;
        }

        // 경매장 파일을 쓰지 못해 commit 을 못 적은 조작 — 다음에 파일을 쓰면 그 상태가 함께 저장되므로 그때 commit 을 적는다.
        // 안 그러면 보고서가 「끊긴 조작」으로 보이고, 런북대로 되살리면 하나가 더 생긴다(리뷰 2026-10-07).
        private static readonly List<long> Pending = new List<long>();

        private static bool SaveBook()
        {
            try
            {
                SafeFile.Write(BookPath, JsonConvert.SerializeObject(_book, Formatting.None, StorageManager.Settings));
                foreach (long seq in Pending)
                    Write(new JObject { ["seq"] = seq, ["ev"] = "commit" });
                Pending.Clear();
                return true;
            }
            catch (Exception e)
            {
                ServerContext.Error(e);
                ServerContext.Logger("auction save failed", Microsoft.Extensions.Logging.LogLevel.Error);
                return false;
            }
        }

        private static bool Write(JObject line)
        {
            try
            {
                DateTime now = DateTime.UtcNow;
                line["at"] = now.ToString("yyyy-MM-ddTHH:mm:ssZ");
                Directory.CreateDirectory(Folder);
                byte[] bytes = Encoding.UTF8.GetBytes(line.ToString(Formatting.None) + "\n");
                using var stream = new FileStream(Path.Combine(Folder, $"events-{now:yyyy-MM-dd}.jsonl"), FileMode.Append, FileAccess.Write, FileShare.Read);
                stream.Write(bytes, 0, bytes.Length);
                stream.Flush(true);
                return true;
            }
            catch (Exception e)
            {
                ServerContext.Error(e);
                ServerContext.Logger("auction event write failed", Microsoft.Extensions.Logging.LogLevel.Error);
                return false;
            }
        }

        private static JObject Line(long seq, string ev, string who, long listing, string item, long gold, object data = null)
        {
            var line = new JObject { ["seq"] = seq, ["ev"] = ev, ["who"] = who, ["listing"] = listing, ["item"] = item, ["gold"] = gold };
            if (data != null)
                line["data"] = JToken.FromObject(data);
            return line;
        }

        /// <summary>조작의 첫 줄. 적지 못하면 0 — 그 조작은 하지 않는다.</summary>
        private static long Begin(string ev, string who, long listing, string item, long gold, long? before = null, long? after = null, object data = null)
        {
            long seq = _book.LastSeq + 1;
            JObject line = Line(seq, ev, who, listing, item, gold, data);
            if (before.HasValue)
            {
                line["goldBefore"] = before.Value;
                line["goldAfter"] = after ?? before.Value;
            }

            if (!Write(line))
                return 0;

            _book.LastSeq = seq;
            return seq;
        }

        private static void Note(long seq, string ev, string who, long listing, string item, long gold) =>
            Write(Line(seq, ev, who, listing, item, gold));

        private static void Commit(long seq)
        {
            if (SaveBook())
                Write(new JObject { ["seq"] = seq, ["ev"] = "commit" });
            else
                Pending.Add(seq);
        }

        private static void Abort(long seq) => Write(new JObject { ["seq"] = seq, ["ev"] = "abort" });

        /// <summary>룰렛처럼 한쪽만 바뀌는 일의 기록(seq 0 — commit 을 기다리지 않는다, SPEC S-5).</summary>
        public static void Record(string ev, string who, string item, long gold, object data)
        {
            lock (Gate)
                Write(Line(0, ev, who, 0, item, gold, data));
        }

        // ---- 받을 것 ----

        private static Claim AddClaim(string owner, int reason, long listingId, Item item = null, long gold = 0)
        {
            var claim = new Claim
            {
                Id = _book.NextId++, Owner = owner, Kind = item != null ? ClaimItem : ClaimGold, Item = item, Gold = gold,
                Reason = reason, ListingId = listingId, At = DateTime.UtcNow
            };
            _book.Claims.Add(claim);
            return claim;
        }

        public static int ClaimCount(string name)
        {
            lock (Gate)
                return _book.Claims.Count(claim => Same(claim.Owner, name));
        }

        /// <summary>그룹 금화 몫이 들 수 있는 금화를 넘을 때 넘는 만큼(까닭 5). 경매장이 닫혀 있어도 메모리에는 맡는다(S-12).</summary>
        public static void AddGoldClaim(string owner, long gold, object shares)
        {
            if (gold <= 0)
                return;

            lock (Gate)
            {
                long seq = Begin("split", owner, 0, null, gold, data: shares);
                AddClaim(owner, ForSplit, 0, gold: gold);
                if (seq == 0 || !_loaded || ServerContext.Config.DontSavePlayers)
                {
                    ServerContext.Logger($"경매장: {owner} 넘친 금화 {gold} 를 메모리 받을 것에만 맡김", Microsoft.Extensions.Logging.LogLevel.Warning);
                    return;
                }

                Commit(seq);
            }
        }

        // ---- 요청 처리(0xF4) ----

        public sealed class Notices : List<(string Name, string Message)> { }

        public static void Handle(GameClient client, ClientFormatF4 format)
        {
            DateTime now = DateTime.UtcNow;
            if (now - client.LastAuctionRequest < RequestGap)
            {
                Reply(client, false, "잠시 뒤에 다시 하십시오");
                return;
            }

            client.LastAuctionRequest = now;
            var notices = new Notices();
            (bool ok, string message) result;

            switch (format.Kind)
            {
                case ClientFormatF4.Browse:
                    client.Send(Page(client.Aisling.Username, 0, format.Page,
                        () => Browse(format.Category, format.Sort, format.Query)));
                    return;
                case ClientFormatF4.Mine:
                    client.Send(Page(client.Aisling.Username, 1, format.Page, () => Mine(client.Aisling.Username)));
                    return;
                case ClientFormatF4.Claims:
                    client.Send(ClaimPage(client.Aisling.Username, format.Page));
                    return;
                case ClientFormatF4.Post:
                    result = Post(client, format.Slot, format.Start, format.BuyoutPrice, format.Hours);
                    break;
                case ClientFormatF4.Bid:
                    result = Bid(client, format.Id, format.Amount, notices);
                    break;
                case ClientFormatF4.Buyout:
                    result = Buyout(client, format.Id, notices);
                    break;
                case ClientFormatF4.Cancel:
                    result = Cancel(client, format.Id, notices);
                    break;
                case ClientFormatF4.Take:
                    result = Take(client, format.Id);
                    break;
                default:
                    return;
            }

            client.SendStats(StatusFlags.StructC);
            Reply(client, result.ok, result.message);
            Tell(notices);
        }

        private static void Reply(GameClient client, bool ok, string message) =>
            client.Send(Done(ok, message, ClaimCount(client.Aisling.Username), notice: false));

        /// <summary>E-03 — 끝의 알림 바이트(2026-10-07): 1 이면 내 요청의 답이 아니라 남의 조작이 알린 것(팔림·밀림). 옛 앱은 읽지 않는다.</summary>
        private static ServerFormat5E Done(bool ok, string message, int claims, bool notice) =>
            ServerFormat5E.Of(ServerFormat5E.AuctionDone, writer =>
            {
                writer.Write((byte) (ok ? 1 : 0));
                // 길이는 한 바이트다 — 긴 물건 이름이 255 바이트를 넘기지 않게 자른다.
                string text = message ?? string.Empty;
                writer.WriteStringA(text.Length > 80 ? text.Substring(0, 80) : text);
                writer.Write((ushort) Math.Min(claims, ushort.MaxValue));
                writer.Write((byte) (notice ? 1 : 0));
            });

        private static void Tell(Notices notices)
        {
            foreach (var (name, message) in notices)
            {
                var online = ServerContext.Game?.Clients.FirstOrDefault(c => c?.Aisling != null && Same(c.Aisling.Username, name));
                online?.Send(Done(true, message, ClaimCount(name), notice: true));
            }
        }

        private static string Closed(bool taking) =>
            !_loaded || ServerContext.Config.DontSavePlayers || (!taking && !ServerContext.Config.AuctionEnabled)
                ? "경매장이 닫혀 있습니다"
                : null;

        private static string CannotAct(Aisling aisling) =>
            aisling.IsDead() ? "살아 있을 때만 할 수 있습니다"
            : aisling.Exchange != null ? "교환 중에는 안 됩니다"
            : null;

        private static long MaxGold => ServerContext.Config.MaxCarryGold;

        /// <summary>경매에 낼 수 있는 금화 — 들고 있는 것 + 은행에 맡긴 것(DL-14). 사건 줄의 goldBefore·goldAfter 도 이 합이다.</summary>
        private static long Funds(Aisling me) => me.GoldPoints + (me.BankManager?.Gold ?? 0);

        private static void AddBankGold(Aisling me, long delta)
        {
            if (delta == 0)
                return;

            Bank bank = me.BankManager ??= new Bank();
            bank.Gold += delta;
            me.Client?.Activity?.Currency("bank_gold", delta, bank.Gold);
        }

        private static bool SaveCharacter(Aisling aisling) => StorageManager.AislingBucket.TrySave(aisling);

        /// <summary>
        /// 경매 문(<see cref="Gate" />)과 캐릭터 자물쇠(저장과 같은 것, <see cref="AislingStorage.LockFor" />)를 잡고, 이 접속이 아직 세상에 있을 때만
        /// 일을 한다. 접속 끊기·밀어내기 로그인(<c>GameServer.ClientDisconnected</c>)이 같은 자물쇠 안에서 저장하고 접속을 빼므로, 조작이 다 끝난
        /// 뒤에 끊기거나 끊긴 뒤의 조작은 거절된다 — 새 접속이 지불 전 파일을 읽어 금화가 되살아나지 않는다. 룰렛·금화 나눔(<see cref="GroupLoot" />)도
        /// 같은 캐릭터 자물쇠 안에서 가방·금화를 고친다(리뷰 2026-10-07).
        /// </summary>
        private static (bool, string) AsLive(GameClient client, Func<(bool, string)> work)
        {
            lock (Gate)
            lock (AislingStorage.LockFor(client.Aisling.Username))
            {
                if (client.Aisling?.LoggedIn != true || ServerContext.Game?.Clients.Contains(client) != true)
                    return (false, "접속이 끊겼습니다");

                return work();
            }
        }

        public static (bool, string) Post(GameClient client, byte slot, uint start, uint buyout, byte hours)
        {
            string refused = Closed(false);
            if (refused != null)
                return (false, refused);

            int duration = Array.IndexOf(AUCTION_DURATIONS, (int) hours);
            if (duration < 0 || start < 1 || start > AUCTION_MAX_PRICE || (buyout != 0 && (buyout < start || buyout > AUCTION_MAX_PRICE)))
                return (false, "값이 맞지 않습니다");

            return AsLive(client, () =>
            {
                Aisling me = client.Aisling;
                if (CannotAct(me) is { } cannot)
                    return (false, cannot);

                Item item = me.Inventory.FindInSlot(slot);
                if (item?.Template == null || !item.Template.Flags.HasFlag(ItemFlags.Tradeable))
                    return (false, "올릴 수 없는 물건입니다");
                if (_book.Listings.Count(listing => Same(listing.Seller, me.Username)) >= AUCTION_MAX_LISTINGS)
                    return (false, "올린 물건이 너무 많습니다");

                long deposit = Math.Max(1, (long) start * AUCTION_DEPOSIT_RATE[duration] / 100);
                if (deposit > Funds(me))
                    return (false, "보증금이 모자랍니다");

                long seq = Begin("post", me.Username, _book.NextId, item.DisplayName, deposit, Funds(me), Funds(me) - deposit,
                    new { start, buyout, hours, stacks = (int) item.Stacks });
                if (seq == 0)
                    return (false, "저장에 실패했습니다");

                me.Inventory.RemoveRange(client, item, Math.Max(1, (int) item.Stacks));
                if (!Pay(me, deposit, seq))
                {
                    me.Inventory.Set(item, false);
                    me.CurrentWeight += item.Template.CarryWeight;
                    client.Send(new ServerFormat0F(item));
                    return (false, "저장에 실패했습니다");
                }

                DateTime now = DateTime.UtcNow;
                _book.Listings.Add(new Listing
                {
                    Id = _book.NextId++, Seller = me.Username, Item = item, StartBid = (int) start, Buyout = (int) buyout,
                    Deposit = (int) deposit, Hours = hours, PostedAt = now, ExpiresAt = now.AddHours(hours)
                });
                Commit(seq);
                return (true, $"올렸습니다: {item.DisplayName} (보증금 {deposit:N0}전)");
            });
        }

        // 끝 시각이 지난 경매는 기간 끝 처리(Expire, 1분마다)를 기다리는 동안에도 끝난 것이다 — 입찰·구매·취소를 받지 않는다.
        private static Listing Find(uint id) => _book.Listings.FirstOrDefault(listing => listing.Id == id && listing.ExpiresAt > DateTime.UtcNow);

        /// <summary>
        /// 내주는 쪽(캐릭터) 금화를 빼고 저장한다 — 들고 있는 것 먼저, 모자라면 은행에서(DL-14). 저장하지 못하면 되돌리고 abort.
        /// 더하고 빼기만 한다 — 그사이 들어온 금화를 지우지 않게.
        /// </summary>
        private static bool Pay(Aisling me, long amount, long seq)
        {
            long hand = Math.Clamp(me.GoldPoints, 0, amount);
            me.GoldPoints -= (int) hand;
            AddBankGold(me, hand - amount);
            if (SaveCharacter(me))
                return true;

            me.GoldPoints += (int) hand;
            AddBankGold(me, amount - hand);
            Abort(seq);
            return false;
        }

        private static void Refund(Listing listing, long seq, Notices notices)
        {
            if (listing.Bidder == null)
                return;

            AddClaim(listing.Bidder, ForOutbid, listing.Id, gold: listing.Bid);
            Note(seq, "outbid", listing.Bidder, listing.Id, listing.Item.DisplayName, listing.Bid);
            notices.Add((listing.Bidder, $"입찰에서 밀렸습니다: {listing.Item.DisplayName}"));
        }

        /// <summary>낙찰 — 사는 이에게 물건, 파는 이에게 값 − 수수료 + 보증금.</summary>
        private static void Settle(Listing listing, string buyer, long price, Notices notices)
        {
            _book.Listings.Remove(listing);
            AddClaim(buyer, ForWon, listing.Id, item: listing.Item);
            AddClaim(listing.Seller, ForSale, listing.Id, gold: price - price * AUCTION_CUT / 100 + listing.Deposit);
            notices.Add((listing.Seller, $"경매 물건이 팔렸습니다: {listing.Item.DisplayName}"));
        }

        public static (bool, string) Bid(GameClient client, uint id, uint amount, Notices notices)
        {
            string refused = Closed(false);
            if (refused != null)
                return (false, refused);

            return AsLive(client, () =>
            {
                Aisling me = client.Aisling;
                if (CannotAct(me) is { } cannot)
                    return (false, cannot);

                Listing listing = Find(id);
                if (listing == null)
                    return (false, "이미 끝난 경매입니다");
                if (Same(listing.Seller, me.Username))
                    return (false, "제 물건에는 입찰할 수 없습니다");
                if (amount > AUCTION_MAX_PRICE)
                    return (false, "값이 맞지 않습니다");
                // 즉시 구매가 이상이면 최고 입찰자라도 즉시 구매(S-9) — 맡긴 입찰금은 돌려받는다.
                if (listing.Buyout > 0 && amount >= listing.Buyout)
                    return BuyoutLocked(me, listing, notices);
                if (Same(listing.Bidder, me.Username))
                    return (false, "이미 최고 입찰자입니다");

                long least = NextBid(listing);
                if (amount < least)
                    return (false, $"입찰가가 낮습니다 (최소 {least:N0}전)");
                if (amount > Funds(me))
                    return (false, "금화가 모자랍니다");

                long seq = Begin("bid", me.Username, listing.Id, listing.Item.DisplayName, amount, Funds(me), Funds(me) - amount);
                if (seq == 0 || !Pay(me, amount, seq))
                    return (false, "저장에 실패했습니다");

                Refund(listing, seq, notices);
                listing.Bid = (int) amount;
                listing.Bidder = me.Username;
                Commit(seq);
                return (true, $"입찰했습니다: {listing.Item.DisplayName} {amount:N0}전");
            });
        }

        public static long NextBid(Listing listing) =>
            listing.Bidder == null ? listing.StartBid : listing.Bid + Math.Max(1L, (long) listing.Bid * AUCTION_MIN_STEP / 100);

        public static (bool, string) Buyout(GameClient client, uint id, Notices notices)
        {
            string refused = Closed(false);
            if (refused != null)
                return (false, refused);

            return AsLive(client, () =>
            {
                Aisling me = client.Aisling;
                if (CannotAct(me) is { } cannot)
                    return (false, cannot);

                Listing listing = Find(id);
                if (listing == null)
                    return (false, "이미 끝난 경매입니다");
                if (listing.Buyout == 0)
                    return (false, "즉시 구매가 없는 경매입니다");
                if (Same(listing.Seller, me.Username))
                    return (false, "제 물건입니다");

                return BuyoutLocked(me, listing, notices);
            });
        }

        private static (bool, string) BuyoutLocked(Aisling me, Listing listing, Notices notices)
        {
            long price = listing.Buyout;
            if (price > Funds(me))
                return (false, "금화가 모자랍니다");

            long seq = Begin("buyout", me.Username, listing.Id, listing.Item.DisplayName, price, Funds(me), Funds(me) - price);
            if (seq == 0 || !Pay(me, price, seq))
                return (false, "저장에 실패했습니다");

            Refund(listing, seq, notices);
            Settle(listing, me.Username, price, notices);
            Commit(seq);
            return (true, $"샀습니다: {listing.Item.DisplayName} — 받을 것에서 받으십시오");
        }

        public static (bool, string) Cancel(GameClient client, uint id, Notices notices)
        {
            string refused = Closed(false);
            if (refused != null)
                return (false, refused);

            return AsLive(client, () =>
            {
                Aisling me = client.Aisling;
                if (CannotAct(me) is { } cannot)
                    return (false, cannot);

                Listing listing = Find(id);
                if (listing == null)
                    return (false, "이미 끝난 경매입니다");
                if (!Same(listing.Seller, me.Username))
                    return (false, "내 경매가 아닙니다");

                long fee = listing.Bidder != null ? (long) listing.Bid * AUCTION_CUT / 100 : 0;
                if (fee > Funds(me))
                    return (false, "수수료가 모자랍니다");

                long seq = Begin("cancel", me.Username, listing.Id, listing.Item.DisplayName, fee, Funds(me), Funds(me) - fee);
                if (seq == 0 || (fee > 0 && !Pay(me, fee, seq)))
                    return (false, "저장에 실패했습니다");

                Refund(listing, seq, notices);
                _book.Listings.Remove(listing);
                AddClaim(me.Username, ForCancel, listing.Id, item: listing.Item);
                Commit(seq);
                return (true, $"취소했습니다: {listing.Item.DisplayName} — 받을 것에서 받으십시오");
            });
        }

        /// <summary>
        /// 받기 — 경매장(내주는 쪽)을 먼저 저장하고 가방·금화에 넣는다. 물건은 들어갈 것만 꺼내고, 금화는 모두 — 들 수 있는 만큼 손에,
        /// 넘는 것은 은행에(DL-14, 값 상한이 들 수 있는 금화보다 커서).
        /// </summary>
        public static (bool, string) Take(GameClient client, uint id)
        {
            string refused = Closed(true);
            if (refused != null)
                return (false, refused);

            return AsLive(client, () =>
            {
                Aisling me = client.Aisling;
                // 교환에 내놓은 금화는 들고 있는 금화에서 빠져 있다 — 그 사이 받으면 교환을 물릴 때 상한에 잘려 사라진다.
                if (CannotAct(me) is { } cannot)
                    return (false, cannot);

                var mine = _book.Claims.Where(claim => Same(claim.Owner, me.Username) && (id == 0 || claim.Id == id))
                    .OrderBy(claim => claim.Id).ToList();
                if (mine.Count == 0)
                    return (false, id == 0 ? "받을 것이 없습니다" : "이미 받은 것입니다");

                int free = me.Inventory.Items.Values.Count(item => item == null);
                var items = new List<Claim>();
                var golds = new List<Claim>();
                bool full = false;

                foreach (Claim claim in mine)
                {
                    if (claim.Kind == ClaimItem)
                    {
                        if (!Merges(me, claim.Item))
                        {
                            if (free == 0)
                            {
                                full = true;
                                continue;
                            }

                            free--;
                        }

                        items.Add(claim);
                    }
                    else
                    {
                        golds.Add(claim);
                    }
                }

                if (items.Count == 0 && golds.Count == 0)
                    return (false, "가방에 자리가 없습니다");

                long goldTotal = golds.Sum(claim => claim.Gold);
                long seq = Begin("take", me.Username, 0, string.Join(",", items.Select(claim => claim.Item.DisplayName)), goldTotal,
                    Funds(me), Funds(me) + goldTotal, new { claims = items.Concat(golds).Select(claim => claim.Id) });
                if (seq == 0)
                    return (false, "저장에 실패했습니다");

                // 내주는 쪽: 경매장
                foreach (Claim claim in items.Concat(golds))
                    _book.Claims.Remove(claim);

                if (!SaveBook())
                {
                    _book.Claims.AddRange(items.Concat(golds));
                    Abort(seq);
                    return (false, "저장에 실패했습니다");
                }

                // 받는 쪽: 캐릭터(캐릭터 자물쇠 안이라 그사이 나눔 금화가 끼어 상한을 넘지 않는다)
                bool returned = false;
                foreach (Claim claim in items)
                {
                    lock (Generator.Random)
                        claim.Item.Serial = Generator.GenerateNumber();

                    if (!claim.Item.GiveTo(me))
                    {
                        _book.Claims.Add(claim);
                        returned = true;
                    }
                }

                long hand = Math.Clamp(MaxGold - me.GoldPoints, 0, goldTotal);
                me.GoldPoints += (int) hand;
                AddBankGold(me, goldTotal - hand);
                if (returned)
                    SaveBook();

                if (SaveCharacter(me))
                {
                    Write(new JObject { ["seq"] = seq, ["ev"] = "commit" });
                }
                else
                {
                    // 경매장은 이미 내줬다 — 다음 주기 저장(45초)이 캐릭터에 넣을 수 있으니 「끊김」이 아니라 「저장 못 함」으로 적는다.
                    // 런북은 캐릭터 파일을 보고 정말 빠졌을 때만 되살린다(auction-report.py).
                    Write(new JObject { ["seq"] = seq, ["ev"] = "unsaved", ["who"] = me.Username });
                    ServerContext.Logger($"auction save failed — {me.Username} 받기 seq {seq} 캐릭터 저장 실패", Microsoft.Extensions.Logging.LogLevel.Error);
                }

                string left = full || returned ? " — 가방에 자리가 없어 남은 것이 있습니다"
                    : goldTotal > hand ? $" — 들 수 없는 {goldTotal - hand:N0}전은 은행에 넣었습니다"
                    : string.Empty;
                return (true, $"받았습니다{left}");
            });
        }

        // 겹치는 물건이 가방의 같은 묶음에 더해지는지 — Item.GiveTo 와 같은 조건.
        private static bool Merges(Aisling me, Item item) =>
            item.Template.Flags.HasFlag(ItemFlags.Stackable)
            && me.Inventory.Has(one => one.Template?.Name == item.Template.Name
                                       && one.Stacks + Math.Max(1, (int) item.Stacks) <= one.Template.MaxStack) != null;

        /// <summary>한 번의 기간 끝 처리에서 다루는 경매 수 — 오래 꺼졌다 켜져 한꺼번에 끝나도 서버 루프를 오래 붙잡지 않게.</summary>
        private const int ExpireBatch = 50;

        /// <summary>
        /// 기간 끝 — 서버 루프(<c>GameServer.UpdateClients</c>)가 부르고, AUCTION_TICK 마다 한 번만 본다. 사건 줄은 경매마다, 경매장 파일은 한 번만
        /// 쓰고 commit 을 모아 적는다(남은 것은 다음 틱).
        /// </summary>
        public static void Expire(DateTime now)
        {
            if (!_loaded || ServerContext.Config.DontSavePlayers || now - _lastExpire < AUCTION_TICK)
                return;

            var notices = new Notices();
            lock (Gate)
            {
                _lastExpire = now;
                var done = new List<long>();
                foreach (Listing listing in _book.Listings.Where(one => one.ExpiresAt <= now).Take(ExpireBatch).ToList())
                {
                    bool sold = listing.Bidder != null;
                    long seq = Begin(sold ? "sold" : "expired", listing.Seller, listing.Id, listing.Item.DisplayName,
                        sold ? listing.Bid : listing.Deposit, data: new { bidder = listing.Bidder });
                    if (seq == 0)
                        continue;

                    if (sold)
                    {
                        Settle(listing, listing.Bidder, listing.Bid, notices);
                        notices.Add((listing.Bidder, $"경매에서 낙찰받았습니다: {listing.Item.DisplayName}"));
                    }
                    else
                    {
                        _book.Listings.Remove(listing);
                        AddClaim(listing.Seller, ForExpired, listing.Id, item: listing.Item);
                        notices.Add((listing.Seller, $"경매가 유찰되었습니다: {listing.Item.DisplayName}"));
                    }

                    done.Add(seq);
                }

                if (done.Count > 0)
                {
                    Pending.AddRange(done);
                    SaveBook();
                }
            }

            Tell(notices);
        }

        // ---- 읽기(0x5E 8) ----

        private readonly record struct Row(long Id, ushort Image, byte Color, string Name, ushort Stacks, byte Band, long Price, long Buyout, byte Flags, Item Item);

        /// <summary>종류(SPEC S-11): 1 무기 · 2 방어구 · 3 장신구 · 4 기타.</summary>
        public static byte KindOf(Item item)
        {
            if (!item.Template.Flags.HasFlag(ItemFlags.Equipable))
                return 4;

            switch (item.Template.EquipmentSlot)
            {
                case 1: return 1;
                case 2: case 3: case 4: case 9: case 10: case 12: case 13: case 15: case 16: return 2;
                case 5: case 6: case 7: case 8: case 11: case 14: case 17: return 3;
                default: return 4;
            }
        }

        private static byte Band(TimeSpan left) =>
            left < TimeSpan.FromMinutes(30) ? (byte) 0 : left < TimeSpan.FromHours(2) ? (byte) 1 : left < TimeSpan.FromHours(12) ? (byte) 2 : (byte) 3;

        private static Row ToRow(Listing listing, string viewer, DateTime now) => new Row(
            listing.Id, listing.Item.DisplayImage, listing.Item.Color, listing.Item.DisplayName, (ushort) Math.Max(1, (int) listing.Item.Stacks),
            Band(listing.ExpiresAt - now), listing.Bidder != null ? listing.Bid : listing.StartBid, listing.Buyout,
            (byte) ((Same(listing.Seller, viewer) ? 1 : 0) | (Same(listing.Bidder, viewer) ? 2 : 0) | (listing.Bidder != null ? 4 : 0)),
            listing.Item);

        private static IEnumerable<Listing> Browse(byte category, byte sort, string query)
        {
            IEnumerable<Listing> found = _book.Listings.Where(listing =>
                (category == 0 || KindOf(listing.Item) == category)
                && (string.IsNullOrEmpty(query) || listing.Item.DisplayName.Contains(query, StringComparison.OrdinalIgnoreCase)));

            return sort switch
            {
                1 => found.OrderBy(listing => listing.Bidder != null ? listing.Bid : listing.StartBid),
                2 => found.OrderBy(listing => listing.Buyout == 0 ? long.MaxValue : listing.Buyout),
                _ => found.OrderBy(listing => listing.ExpiresAt)
            };
        }

        private static IEnumerable<Listing> Mine(string viewer) =>
            _book.Listings.Where(listing => Same(listing.Seller, viewer) || Same(listing.Bidder, viewer)).OrderBy(listing => listing.ExpiresAt);

        private static ServerFormat5E Page(string viewer, byte view, ushort page, Func<IEnumerable<Listing>> query)
        {
            List<Row> rows;
            int pages, claims;
            DateTime now = DateTime.UtcNow;
            lock (Gate)
            {
                var all = query().ToList();
                pages = Math.Max(1, (all.Count + AUCTION_PAGE - 1) / AUCTION_PAGE);
                page = (ushort) Math.Min(page, pages - 1);
                rows = all.Skip(page * AUCTION_PAGE).Take(AUCTION_PAGE).Select(listing => ToRow(listing, viewer, now)).ToList();
                claims = _book.Claims.Count(claim => Same(claim.Owner, viewer));
            }

            return ServerFormat5E.Of(ServerFormat5E.AuctionPage, writer =>
            {
                WritePageHead(writer, view, page, pages, claims, rows.Count);
                foreach (Row row in rows)
                {
                    writer.Write((uint) row.Id);
                    writer.Write(row.Image);
                    writer.Write(row.Color);
                    writer.WriteStringA(row.Name);
                    writer.Write(row.Stacks);
                    writer.Write(row.Band);
                    writer.Write((uint) row.Price);
                    writer.Write((uint) row.Buyout);
                    writer.Write(row.Flags);
                }

                // 꼬리(2026-10-07): 줄마다 장비 수치 — 소지품(0x0F)과 같은 모양. 봇·앱이 「맞고 더 좋은지」를 본다. 옛 앱은 줄까지만 읽는다.
                foreach (Row row in rows)
                    ServerFormat0F.WriteNumbers(writer, row.Item);
            });
        }

        private static ServerFormat5E ClaimPage(string viewer, ushort page)
        {
            List<Claim> rows;
            int pages, claims;
            lock (Gate)
            {
                var all = _book.Claims.Where(claim => Same(claim.Owner, viewer)).OrderBy(claim => claim.Id).ToList();
                claims = all.Count;
                pages = Math.Max(1, (all.Count + AUCTION_PAGE - 1) / AUCTION_PAGE);
                page = (ushort) Math.Min(page, pages - 1);
                rows = all.Skip(page * AUCTION_PAGE).Take(AUCTION_PAGE).ToList();
            }

            return ServerFormat5E.Of(ServerFormat5E.AuctionPage, writer =>
            {
                WritePageHead(writer, 2, page, pages, claims, rows.Count);
                foreach (Claim claim in rows)
                {
                    writer.Write((uint) claim.Id);
                    writer.Write((byte) claim.Kind);
                    writer.Write(claim.Item?.DisplayImage ?? (ushort) 0);
                    writer.Write(claim.Item?.Color ?? (byte) 0);
                    writer.WriteStringA(claim.Item?.DisplayName ?? "금화");
                    writer.Write((ushort) Math.Max(1, (int) (claim.Item?.Stacks ?? 1)));
                    writer.Write((uint) Math.Min(claim.Gold, uint.MaxValue));
                    writer.Write((byte) claim.Reason);
                }
            });
        }

        private static void WritePageHead(NetworkPacketWriter writer, byte view, ushort page, int pages, int claims, int count)
        {
            writer.Write(view);
            writer.Write(page);
            writer.Write((ushort) pages);
            writer.Write((ushort) Math.Min(claims, ushort.MaxValue));
            writer.Write((byte) count);
        }
    }
}
