using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.IO;
using System.Net;
using System.Linq;
using System.Text.Json;
using Darkages.Types;

namespace Darkages.Network.Game
{
    /// <summary>인증된 게임 접속의 허용된 활동만 기록한다. 원시 패킷과 대화는 저장하지 않는다.</summary>
    public sealed class ActivitySession
    {
        private static readonly object FileGate = new object();
        private static DateTime _lastError;
        private readonly object _gate = new object();
        private readonly GameClient _client;
        private readonly string _folder;
        private readonly string _id = Guid.NewGuid().ToString("N");
        private readonly DateTime _started = DateTime.UtcNow;
        private readonly Dictionary<(string Kind, string Detail), int> _requests = new Dictionary<(string Kind, string Detail), int>();
        private DateTime _flushed = DateTime.UtcNow;
        private bool _closed;
        private int _map = -1;
        private long _xp;
        private int _gold;
        private Timer _heartbeat;
        private int _mutationsInProgress;
        private int _mutationVersion;
        private DateTime _telemetryMinute;
        private int _telemetryCount;
        private Dictionary<(string Item, string Place), long> _items;
        private static readonly ConcurrentDictionary<uint, ActivitySession> Sessions = new();
        private static readonly AsyncLocal<Mutation> CurrentMutation = new();
        private static readonly Dictionary<string, (DateTime At, int Count)> Failures = new();

        // Each synchronous server packet shares one transaction and observes affected players after all moves finish.
        public sealed class Mutation : IDisposable
        {
            internal readonly string Id = Guid.NewGuid().ToString("N");
            internal readonly string Reason;
            internal readonly HashSet<ActivitySession> Affected = new();
            internal readonly Dictionary<ActivitySession, string> Peers = new();
            internal readonly Dictionary<ActivitySession, string> ItemReasons = new();
            private readonly Mutation _previous;
            internal Mutation(ActivitySession session, string reason)
            {
                _previous = CurrentMutation.Value;
                Reason = reason;
                CurrentMutation.Value = this;
                Add(session);
            }
            internal void Add(ActivitySession session)
            {
                if (session != null && Affected.Add(session))
                {
                    Interlocked.Increment(ref session._mutationsInProgress);
                    Interlocked.Increment(ref session._mutationVersion);
                }
            }
            public void Dispose()
            {
                CurrentMutation.Value = _previous;
                foreach (var session in Affected)
                {
                    try { session.ObserveItems(ItemReasons.GetValueOrDefault(session) ?? Reason, Id, Peers.GetValueOrDefault(session)); }
                    finally { Interlocked.Increment(ref session._mutationVersion); Interlocked.Decrement(ref session._mutationsInProgress); }
                }
            }
        }

        public static Mutation BeginMutation(ActivitySession session, string reason)
        {
            if (CurrentMutation.Value is Mutation existing)
            {
                existing.Add(session);
                return null;
            }
            return new Mutation(session, reason);
        }

        public static string CallerReason()
        {
            // Server code paths only: never inspect locals, arguments, or user strings.
            var frames = new StackTrace(false).GetFrames();
            foreach (var frame in frames ?? Array.Empty<StackFrame>())
            {
                var method = frame.GetMethod();
                if (method?.DeclaringType == null || method.DeclaringType == typeof(ActivitySession)
                    || method.Name.StartsWith("set_") || method.DeclaringType == typeof(Inventory)
                    || method.DeclaringType == typeof(Item) || method.DeclaringType == typeof(EquipmentManager)
                    || method.DeclaringType == typeof(Bank)) continue;
                return method.DeclaringType.Name + "." + method.Name;
            }
            return "server_update";
        }

        public void Counterparty(string player)
        {
            if (CurrentMutation.Value is Mutation operation)
            {
                operation.Add(this);
                operation.Peers[this] = player;
            }
        }

        public void Currency(string asset, long delta, long balance)
        {
            if (delta == 0) return;
            lock (_gate)
            {
                if (_closed) return;
                var operation = CurrentMutation.Value;
                Write("ledger", meta: new { transaction = operation?.Id ?? Guid.NewGuid().ToString("N"), asset,
                    delta, balance, reason = CallerReason(), counterparty = operation?.Peers.GetValueOrDefault(this) ?? _client.Aisling.Exchange?.Trader?.Username ?? "" });
            }
        }

        public static Mutation BeginItemMutation(uint owner) => Sessions.TryGetValue(owner, out var session)
            ? BeginMutation(session, "Item.Stacks") : null;

        public static void ItemChanged(uint owner)
        {
            if (Sessions.TryGetValue(owner, out var session)) session.ItemsChanged();
        }

        public void ItemsChanged()
        {
            var operation = CurrentMutation.Value;
            if (operation != null)
            {
                operation.Add(this);
                operation.ItemReasons[this] = CallerReason();
            }
            else ObserveItems(CallerReason());
        }

        // ponytail: scan each character's containers per packet; cache container totals if large banks become a hot path.
        private Dictionary<(string Item, string Place), long> Holdings()
        {
            var result = new Dictionary<(string, string), long>();
            void Add(Item item, string place)
            {
                if (item?.Template?.Name == null) return;
                var key = (item.Template.Name, place);
                result[key] = result.GetValueOrDefault(key) + Math.Max(1, (int)item.Stacks);
            }
            var who = _client.Aisling;
            foreach (var item in who.Inventory?.Items?.Values.ToArray() ?? Array.Empty<Item>()) Add(item, "inventory");
            foreach (var slot in who.EquipmentManager?.Equipment?.Values.ToArray() ?? Array.Empty<EquipmentSlot>()) Add(slot?.Item, "equipment");
            foreach (var stack in who.BankManager?.Items?.Values.ToArray() ?? Array.Empty<Stack<Item>>())
                foreach (var item in stack.ToArray()) Add(item, "bank");
            foreach (var item in who.Exchange?.Items?.ToArray() ?? Array.Empty<Item>()) Add(item, "exchange");
            foreach (var item in who.Remains?.Items?.ToArray() ?? Array.Empty<Item>()) Add(item, "death_bag");
            return result;
        }

        private void ObserveItems(string reason, string transaction = null, string counterparty = null)
        {
            try { ObserveItemsCore(reason, transaction, counterparty); }
            catch (Exception error) when (error is InvalidOperationException || error is ArgumentException || error is IndexOutOfRangeException)
            { } // Concurrent container change: next packet/heartbeat reconciles it.
        }

        private void ObserveItemsCore(string reason, string transaction, string counterparty)
        {
            lock (_gate)
            {
                if (_closed) return;
                int version = Volatile.Read(ref _mutationVersion);
                if (Volatile.Read(ref _mutationsInProgress) > (transaction == null ? 0 : 1)) return;
                var next = Holdings();
                if (version != Volatile.Read(ref _mutationVersion) || Volatile.Read(ref _mutationsInProgress) > (transaction == null ? 0 : 1)) return;
                if (_items == null) { _items = next; return; }
                string id = transaction ?? Guid.NewGuid().ToString("N");
                foreach (var item in _items.Keys.Concat(next.Keys).Select(key => key.Item).Distinct())
                {
                    var changes = _items.Keys.Concat(next.Keys).Where(key => key.Item == item).Distinct()
                        .ToDictionary(key => key.Place, key => next.GetValueOrDefault(key) - _items.GetValueOrDefault(key));
                    foreach (var source in changes.Keys.ToArray())
                        foreach (var target in changes.Keys.ToArray())
                        {
                            if (changes[source] >= 0 || changes[target] <= 0) continue;
                            long count = Math.Min(-changes[source], changes[target]);
                            Write("ledger", meta: new { transaction = id, asset = "item", item, quantity = count, delta = 0,
                                balance = next.Where(kv => kv.Key.Item == item).Sum(kv => kv.Value), reason,
                                from = source, to = target, counterparty = counterparty ?? _client.Aisling.Exchange?.Trader?.Username ?? "" });
                            changes[source] += count; changes[target] -= count;
                        }
                    foreach (var change in changes.Where(kv => kv.Value != 0))
                        Write("ledger", meta: new { transaction = id, asset = "item", item, quantity = Math.Abs(change.Value), delta = change.Value,
                            balance = next.Where(kv => kv.Key.Item == item).Sum(kv => kv.Value), reason,
                            from = change.Value < 0 ? change.Key : (reason.EndsWith(".Format07Handler") ? "ground" : "external"),
                            to = change.Value > 0 ? change.Key : (reason.EndsWith(".Format08Handler") ? "ground" : "external"),
                            counterparty = counterparty ?? _client.Aisling.Exchange?.Trader?.Username ?? "" });
                }
                _items = next;
            }
        }

        public bool Telemetry(string json)
        {
            if (string.IsNullOrEmpty(json) || Encoding.UTF8.GetByteCount(json) > 2048) return false;
            try
            {
                using var doc = JsonDocument.Parse(json, new JsonDocumentOptions { MaxDepth = 4 });
                var root = doc.RootElement;
                if (root.ValueKind != JsonValueKind.Object || !root.TryGetProperty("kind", out var kindNode)
                    || kindNode.ValueKind != JsonValueKind.String || !root.TryGetProperty("meta", out var input)
                    || input.ValueKind != JsonValueKind.Object) return false;
                string kind = kindNode.GetString();
                if (kind != "app_device" && kind != "app_screen" && kind != "app_button" && kind != "app_error" && kind != "app_lifecycle") return false;
                int count = 1;
                if (root.TryGetProperty("count", out var countNode) && (!countNode.TryGetInt32(out count) || count < 1 || count > 1000)) return false;
                foreach (var property in root.EnumerateObject()) if (property.Name != "kind" && property.Name != "meta" && property.Name != "count") return false;
                var meta = new Dictionary<string, object> { ["reported"] = true };
                foreach (var property in input.EnumerateObject())
                {
                    string key = property.Name;
                    if (key == "occurredAt")
                    {
                        if (property.Value.ValueKind != JsonValueKind.String) return false;
                        string occurredValue = property.Value.GetString();
                        var now = DateTimeOffset.UtcNow;
                        if (occurredValue.Length > 64 || occurredValue.Any(char.IsControl) || !occurredValue.Contains('T')
                            || !(occurredValue.EndsWith("Z", StringComparison.Ordinal) || occurredValue.EndsWith("+00:00", StringComparison.Ordinal))
                            || !DateTimeOffset.TryParse(occurredValue, System.Globalization.CultureInfo.InvariantCulture,
                                System.Globalization.DateTimeStyles.None, out var occurred)
                            || occurred.Offset != TimeSpan.Zero || occurred < now.AddDays(-90) || occurred > now.AddDays(1)) return false;
                        meta[key] = occurredValue; continue;
                    }
                    if (key == "beforeLogin")
                    {
                        if (property.Value.ValueKind != JsonValueKind.True && property.Value.ValueKind != JsonValueKind.False) return false;
                        meta[key] = property.Value.GetBoolean(); continue;
                    }
                    if (key == "seconds")
                    {
                        if (!property.Value.TryGetInt32(out int seconds) || seconds < 0 || seconds > 604800) return false;
                        meta[key] = seconds; continue;
                    }
                    if (key != "install" && key != "run" && key != "platform" && key != "model" && key != "os" && key != "version"
                        && key != "screen" && key != "action" && key != "error" && key != "previousScreen") return false;
                    if (property.Value.ValueKind != JsonValueKind.String) return false;
                    string value = property.Value.GetString();
                    if (value.Length > 128 || value.Any(char.IsControl)) return false;
                    if ((key == "install" || key == "run") && !Guid.TryParse(value, out _)) return false;
                    if ((key == "screen" || key == "action" || key == "error" || key == "previousScreen")
                        && value.Any(c => c > 127 || (!char.IsLetterOrDigit(c) && c != '_' && c != '.' && c != ':' && c != '-'))) return false;
                    meta[key] = value;
                }
                lock (_gate)
                {
                    if (_closed) return false;
                    if ((DateTime.UtcNow - _telemetryMinute).TotalMinutes >= 1) { _telemetryMinute = DateTime.UtcNow; _telemetryCount = 0; }
                    if (++_telemetryCount > 60) return false;
                    Write(kind, count: count, meta: meta);
                    return true;
                }
            }
            catch (Exception error) when (error is JsonException || error is InvalidOperationException || error is FormatException) { return false; }
        }

        public static void LoginFailure(string player, string ip, string reason)
        {
            if (reason != "password" && reason != "account" && reason != "read_failure" && reason != "map_unavailable") return;
            string folder = Environment.GetEnvironmentVariable("LOD_ACTIVITY_DIR") ?? Path.Combine(AppContext.BaseDirectory, "activity");
            lock (FileGate)
            {
                var now = DateTime.UtcNow;
                foreach (var stale in Failures.Where(kv => (now - kv.Value.At).TotalMinutes >= 1).Select(kv => kv.Key).ToArray()) Failures.Remove(stale);
                if (!Failures.TryGetValue(ip, out var value))
                {
                    if (Failures.Count >= 4096) return;
                    value = (now, 0);
                }
                if (value.Count >= 10) return;
                Failures[ip] = (value.At, value.Count + 1);
                string safePlayer = new string((player ?? "").Take(64).Where(c => !char.IsControl(c)).ToArray());
                Append(folder, new { id = Guid.NewGuid().ToString("N"), at = now.ToString("O"), kind = "login_failure", player = safePlayer,
                    bot = Companions.IsBot(safePlayer), ip, session = "", count = 1, xp = 0, gold = 0, seconds = 0, detail = reason, meta = new { reason } });
            }
        }

        public ActivitySession(GameClient client, string folder = null)
        {
            _client = client;
            _folder = folder ?? Environment.GetEnvironmentVariable("LOD_ACTIVITY_DIR") ?? Path.Combine(AppContext.BaseDirectory, "activity");
            _xp = client.Aisling.ExpTotal;
            _gold = client.Aisling.GoldPoints;
            _items = Holdings();
            if (client.Aisling.Inventory != null) client.Aisling.Inventory.Activity = this;
            if (client.Aisling.BankManager != null) client.Aisling.BankManager.Activity = this;
            Sessions[(uint)client.Aisling.Serial] = this;
        }

        public void Login()
        {
            lock (_gate)
            {
                Write("login");
                Map();
                using (ExecutionContext.SuppressFlow())
                    _heartbeat = new Timer(_ =>
                    {
                        lock (_gate)
                        {
                            if (_closed) return;
                            if (Volatile.Read(ref _mutationsInProgress) == 0) ObserveItems("reconcile");
                            Flush();
                            Write("heartbeat", seconds: (int)(DateTime.UtcNow - _started).TotalSeconds);
                        }
                    }, null, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(60));
            }
        }

        public void Map()
        {
            lock (_gate)
            {
                if (_closed || _map == _client.Aisling.CurrentMapId) return;
                _map = _client.Aisling.CurrentMapId;
                Write("map");
            }
        }

        public void Request(byte command, int slot = 0)
        {
            lock (_gate)
            {
                if (_closed) return;
                string kind = command switch
                {
                    0x07 => "pickup", 0x08 => "drop", 0x0F => "spell", 0x3E => "skill",
                    0x1C => "item", 0x24 => "drop_gold", 0x39 => "npc", 0x3A => "npc_choice",
                    0x4A => "exchange", 0x2E => "profile", 0xF0 => "worldmap", 0x3F => "warp",
                    0xF1 => "companion", 0xF2 => "shop",
                    _ => null
                };
                if (kind != null)
                {
                    string detail = command switch
                    {
                        0x3E => _client.Aisling.SkillBook?.Get(i => i != null && i.Slot == slot).FirstOrDefault()?.Template?.Name,
                        0x0F => _client.Aisling.SpellBook?.Get(i => i != null && i.Slot == slot).FirstOrDefault()?.Template?.Name,
                        0x1C => _client.Aisling.Inventory?.Get(i => i != null && i.Slot == slot).FirstOrDefault()?.Template?.Name,
                        _ => ""
                    };
                    var key = (kind, detail ?? "");
                    _requests[key] = _requests.TryGetValue(key, out int n) ? n + 1 : 1;
                }
                // ponytail: 최대 60초의 요청을 묶는다. 강제 종료 직전 구간까지 필요하면 주기를 줄인다.
                if ((DateTime.UtcNow - _flushed).TotalSeconds >= 60) Flush();
            }
        }

        public void Result(string kind, string detail)
        {
            lock (_gate) { if (!_closed) Write(kind, detail); }
        }

        public void Logout()
        {
            lock (_gate)
            {
                if (_closed) return;
                var operation = CurrentMutation.Value;
                if (operation != null) ObserveItems(operation.ItemReasons.GetValueOrDefault(this) ?? operation.Reason, operation.Id, operation.Peers.GetValueOrDefault(this));
                else ObserveItems("disconnect_reconcile");
                Flush();
                Write("logout", seconds: (int)(DateTime.UtcNow - _started).TotalSeconds);
                _closed = true;
                _heartbeat?.Dispose();
                Sessions.TryRemove((uint)_client.Aisling.Serial, out _);
                if (_client.Aisling.Inventory?.Activity == this) _client.Aisling.Inventory.Activity = null;
                if (_client.Aisling.BankManager?.Activity == this) _client.Aisling.BankManager.Activity = null;
            }
        }

        private void Flush()
        {
            foreach (var entry in _requests) Write("request_" + entry.Key.Kind, entry.Key.Detail, count: entry.Value);
            _requests.Clear();
            long xp = (long)_client.Aisling.ExpTotal - _xp;
            int gold = _client.Aisling.GoldPoints - _gold;
            Write("state", xp: xp, gold: gold);
            _xp = _client.Aisling.ExpTotal;
            _gold = _client.Aisling.GoldPoints;
            _flushed = DateTime.UtcNow;
        }

        private void Write(string kind, string detail = "", int count = 1, long xp = 0, int gold = 0, int seconds = 0, object meta = null)
        {
            var who = _client.Aisling;
            string ip = "";
            try { if (_client.Socket?.RemoteEndPoint is IPEndPoint peer) ip = peer.Address.IsIPv4MappedToIPv6 ? peer.Address.MapToIPv4().ToString() : peer.Address.ToString(); }
            catch (ObjectDisposedException) { }
            Append(_folder, new
            {
                id = Guid.NewGuid().ToString("N"), at = DateTime.UtcNow.ToString("O"), kind, player = who.Username,
                bot = Companions.IsBot(who.Username), ip, session = _id, count, xp, gold, seconds, meta = meta ?? new { },
                detail = $"맵 {who.CurrentMapId} ({who.X},{who.Y}) · 레벨 {who.ExpLevel} · 경험치 {who.ExpTotal} · 금화 {who.GoldPoints}" + (detail == "" ? "" : " · " + detail)
            });
        }

        private static void Append(string folder, object payload)
        {
            try
            {
                var now = DateTime.UtcNow;
                lock (FileGate)
                {
                    Directory.CreateDirectory(folder);
                    string file = Path.Combine(folder, now.AddHours(9).ToString("yyyy-MM-dd") + ".jsonl");
                    bool fresh = !File.Exists(file);
                    using (var stream = new FileStream(file, FileMode.Append, FileAccess.Write, FileShare.Read))
                    {
                        if (fresh && !OperatingSystem.IsWindows()) File.SetUnixFileMode(file, UnixFileMode.UserRead | UnixFileMode.UserWrite);
                        using var writer = new StreamWriter(stream);
                        writer.WriteLine(JsonSerializer.Serialize(payload));
                    }
                    if (fresh)
                        foreach (var old in Directory.GetFiles(folder, "????-??-??.jsonl"))
                            if (DateTime.TryParseExact(Path.GetFileNameWithoutExtension(old), "yyyy-MM-dd", null,
                                System.Globalization.DateTimeStyles.None, out var day) && day.Date < now.AddHours(9).Date.AddDays(-89)) File.Delete(old);
                }
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
            {
                lock (FileGate)
                    if ((DateTime.UtcNow - _lastError).TotalMinutes >= 1)
                    {
                        _lastError = DateTime.UtcNow;
                        ServerContext.Logger("게임 활동 기록 저장 실패: " + error.GetType().Name, Microsoft.Extensions.Logging.LogLevel.Error);
                    }
            }
        }
    }
}
