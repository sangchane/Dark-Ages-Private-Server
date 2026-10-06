using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Darkages.Network.Game;

namespace Darkages.Types
{
    /// <summary>
    /// 대신 사냥(2026-10-05, 설계 <c>autopilot/proxy-hunt/</c>) — 자동 사냥 중인 앱의 접속이 끊기면 같은 기계의 대리 프로그램
    /// (<c>mobile/bots/Lod.HuntProxy</c>)이 그 캐릭터로 들어와 앱의 설정대로 사냥한다.
    /// </summary>
    /// <remarks>
    /// 앱이 0xF1 7 로 맡김 설정을 보내 두면(<see cref="Arm" />), 접속이 끊길 때(<see cref="LeftWorld" />) 일회용 열쇠를 만들고
    /// 작업 파일(<c>ProxyJobFolder/이름.json</c>)에 적는다. 대리는 비밀번호 대신 그 열쇠로 로그인한다(<see cref="Take" />) —
    /// 같은 기계(루프백)에서, 그 이름으로, 한 번, <see cref="TokenTtl" /> 안에, 캐릭터가 접속해 있지 않을 때만.
    /// 앱이 다시 로그인하면 원래 규칙(새 접속이 이김)대로 대리가 밀려난다. 비밀번호 사본은 어디에도 생기지 않는다.
    /// </remarks>
    public static class ProxyHunt
    {
        public const int TokenBytes = 32;
        public const int HoursDefault = 2;
        public const int HoursMax = 8;
        public const int ArmBytesMax = 2048;
        public static readonly TimeSpan TokenTtl = TimeSpan.FromSeconds(120);
        public static readonly TimeSpan Grace = TimeSpan.FromSeconds(60);

        private sealed record Ticket(byte[] Token, DateTime Expires, DateTime Until);

        private static readonly object Gate = new object();
        private static readonly Dictionary<string, Ticket> Waiting = new Dictionary<string, Ticket>();
        // 열쇠로 로그인을 마치고 게임 서버로 오는 중인 대리 — 그 로그인 접속 번호(Redirect.Serial = 0x10 의 Id)에 묶는다.
        // 이름에만 묶으면 같은 순간 비밀번호로 들어온 앱의 게임 접속과 가리지 못한다.
        private sealed record Arrival(DateTime Until, int LoginSerial, DateTime Expires);

        private static readonly Dictionary<string, Arrival> Entering = new Dictionary<string, Arrival>();

        /// <summary>작업 파일 폴더를 어디서 읽나 — 서버 설정. 시험은 서버 없이 바꿔 끼운다(이 람다는 부를 때만 서버를 깨운다).</summary>
        public static Func<string> FolderSource = () => ServerContext.Config?.ProxyJobFolder ?? "";

        private static string Folder => FolderSource() ?? "";

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        private static void Log(string line) => ServerContext.Logger(line);

        /// <summary>앱이 보낸 맡김 설정(0xF1 7). 빈 것은 지움, 크거나 JSON 이 아니면 버림.</summary>
        public static void Arm(GameClient client, byte[] payload)
        {
            if (client == null)
                return;

            if (payload == null || payload.Length == 0)
            {
                client.ProxyArm = null;
                return;
            }

            if (payload.Length > ArmBytesMax)
            {
                Log($"대신 사냥: {client.Aisling?.Username} 맡김 설정이 너무 큼({payload.Length}) — 버림");
                return;
            }

            string json = Encoding.UTF8.GetString(payload);

            try
            {
                using JsonDocument parsed = JsonDocument.Parse(json);
                if (parsed.RootElement.ValueKind != JsonValueKind.Object)
                    return;
            }
            catch (JsonException)
            {
                Log($"대신 사냥: {client.Aisling?.Username} 맡김 설정을 읽지 못함 — 버림");
                return;
            }

            client.ProxyArm = json;
        }

        /// <summary>맡김 설정의 시간 — 1~<see cref="HoursMax" />, 없거나 틀리면 <see cref="HoursDefault" />.</summary>
        public static int Hours(string json)
        {
            try
            {
                using JsonDocument parsed = JsonDocument.Parse(json);
                if (parsed.RootElement.TryGetProperty("hours", out JsonElement hours) && hours.TryGetInt32(out int value))
                    return Math.Clamp(value, 1, HoursMax);
            }
            catch (JsonException)
            {
            }

            return HoursDefault;
        }

        /// <summary>
        /// 접속이 세계를 떠난다(<see cref="GameServer.ClientDisconnected" />, 저장 전). 대리였으면 결과 한 줄을 캐릭터에 남기고,
        /// 맡김이 있던 앱이면 대리에게 넘긴다.
        /// </summary>
        public static void LeftWorld(GameClient client, DateTime now)
        {
            var aisling = client?.Aisling;
            if (aisling == null)
                return;

            if (client.ProxyUntil != null)
            {
                // 표시는 남겨 둔다 — 뒤이은 활동 기록(로그아웃)도 대리로 적히게.
                TimeSpan spent = now - client.ProxySince;
                aisling.ProxyReport =
                    $"대신 사냥 {(int)spent.TotalHours}시간 {spent.Minutes}분 · 경험치 +{Math.Max(0, aisling.ExpBank - client.ProxyExp):N0} · 금화 +{Math.Max(0, aisling.GoldPoints - client.ProxyGold):N0}";
                return;
            }

            // 끊김은 무응답 검사와 받기 스레드에서 겹쳐 올 수 있다 — 맡김을 읽고 비우는 것은 한 번만.
            string arm;
            lock (Gate)
            {
                arm = client.ProxyArm;
                client.ProxyArm = null;
            }

            if (arm == null || Folder.Length == 0 || aisling.Dead)
                return;

            try
            {
                Issue(aisling.Username, arm, now);
            }
            catch (Exception failed)
            {
                Log($"대신 사냥: {aisling.Username} 작업 파일을 쓰지 못함 — {failed.Message}");
            }
        }

        /// <summary>열쇠를 만들고 작업 파일을 쓴다. 돌려주는 것은 열쇠(hex) — 시험용.</summary>
        public static string Issue(string name, string settings, DateTime now)
        {
            byte[] token = RandomNumberGenerator.GetBytes(TokenBytes);
            DateTime until = now.AddHours(Hours(settings));
            string key = name.ToLowerInvariant();

            lock (Gate)
                Waiting[key] = new Ticket(token, now + TokenTtl, until);

            string hex = Convert.ToHexString(token);

            if (Folder.Length == 0)
                return hex;

            using JsonDocument parsed = JsonDocument.Parse(settings);
            string job = JsonSerializer.Serialize(new Dictionary<string, object>
            {
                ["name"] = name,
                ["token"] = hex,
                ["until"] = until.ToString("O"),
                ["settings"] = parsed.RootElement,
            });

            Directory.CreateDirectory(Folder);
            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(Folder, UnixFileMode.UserRead | UnixFileMode.UserWrite | UnixFileMode.UserExecute);

            string path = JobPath(key);
            string temporary = path + ".tmp";
            File.WriteAllText(temporary, job);
            if (!OperatingSystem.IsWindows())
                File.SetUnixFileMode(temporary, UnixFileMode.UserRead | UnixFileMode.UserWrite);
            File.Move(temporary, path, true);

            // 열쇠는 기록에 남기지 않는다.
            Log($"대신 사냥: {name} 넘김 — {until:HH:mm} UTC 까지");
            return hex;
        }

        /// <summary>
        /// 대리의 로그인 — 비밀번호 자리의 글이 그 이름의 열쇠면 끝 시각을 돌려주고 열쇠를 지운다. 맞지 않으면 null(표는 그대로).
        /// </summary>
        public static DateTime? Take(string name, string password, IPAddress remote, bool online, DateTime now, int loginSerial)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(password) || remote == null || !IsLoopback(remote))
                return null;

            if (password.Length != TokenBytes * 2)
                return null;

            byte[] offered;
            try
            {
                offered = Convert.FromHexString(password);
            }
            catch (FormatException)
            {
                return null;
            }

            string key = name.ToLowerInvariant();

            lock (Gate)
            {
                if (!Waiting.TryGetValue(key, out Ticket ticket) || now > ticket.Expires
                    || !CryptographicOperations.FixedTimeEquals(ticket.Token, offered))
                    return null;

                // 쓴 열쇠는 버린다. 앱이 이미 돌아와 있으면 대리는 앱을 밀어내지 않는다.
                Waiting.Remove(key);
                DeleteJob(key);

                if (online)
                    return null;

                Entering[key] = new Arrival(ticket.Until, loginSerial, now + TokenTtl);
                return ticket.Until;
            }
        }

        /// <summary>앱이 비밀번호로 로그인했다 — 기다리는 넘김을 지운다.</summary>
        public static void Cancel(string name)
        {
            string key = name?.ToLowerInvariant() ?? "";
            lock (Gate)
                Waiting.Remove(key);

            DeleteJob(key);
        }

        /// <summary>
        /// 게임 서버에 들어오는 접속(0x10) — 열쇠로 로그인한 그 접속이면 끝 시각을 돌려준다. 그 사이 앱이 먼저 들어와 있으면
        /// <paramref name="refuse" /> — 대리는 앱을 밀어내지 않는다.
        /// </summary>
        public static DateTime? Arrive(string name, int loginSerial, bool online, out bool refuse)
        {
            refuse = false;
            string key = name?.ToLowerInvariant() ?? "";
            Arrival arrival;

            lock (Gate)
            {
                if (!Entering.TryGetValue(key, out arrival) || arrival.LoginSerial != loginSerial)
                    return null;

                Entering.Remove(key);
            }

            if (online)
            {
                refuse = true;
                return null;
            }

            return arrival.Until;
        }

        /// <summary>게임 서버가 캐릭터를 올릴 때 — 열쇠로 들어온 접속이면 대리 표시를 달고, 아니면 지난 대리 결과를 알린다.</summary>
        public static void Enter(GameClient client, DateTime now)
        {
            var aisling = client?.Aisling;
            if (aisling == null)
                return;

            if (client.ProxyUntil is DateTime until)
            {
                client.ProxySince = now;
                client.ProxyExp = aisling.ExpBank;
                client.ProxyGold = aisling.GoldPoints;
                Log($"대신 사냥: {aisling.Username} 대리 접속 — {until:HH:mm} UTC 까지");
                return;
            }

            if (!string.IsNullOrEmpty(aisling.ProxyReport))
            {
                client.SendMessage(0x02, aisling.ProxyReport);
                aisling.ProxyReport = null;
            }
        }

        /// <summary>대리 프로그램이 멈춰도 끝 시각은 지킨다 — 끝 + <see cref="Grace" /> 가 지났나.</summary>
        public static bool Overdue(GameClient client, DateTime now) =>
            client?.ProxyUntil is DateTime until && now > until + Grace;

        /// <summary>쓰이지 않고 시간이 지난 열쇠·작업 파일을 치운다.</summary>
        public static void Expire(DateTime now)
        {
            List<string> gone = null;

            lock (Gate)
            {
                foreach (var (key, arrival) in new List<KeyValuePair<string, Arrival>>(Entering))
                    if (now > arrival.Expires)
                        Entering.Remove(key);

                foreach (var (key, ticket) in Waiting)
                    if (now > ticket.Expires)
                        (gone ??= new List<string>()).Add(key);

                if (gone == null)
                    return;

                foreach (string key in gone)
                    Waiting.Remove(key);
            }

            foreach (string key in gone)
                DeleteJob(key);
        }

        internal static bool IsLoopback(IPAddress remote) =>
            IPAddress.IsLoopback(remote.IsIPv4MappedToIPv6 ? remote.MapToIPv4() : remote);

        private static string JobPath(string key) => Path.Combine(Folder, key + ".json");

        private static void DeleteJob(string key)
        {
            if (Folder.Length == 0)
                return;

            try
            {
                File.Delete(JobPath(key));
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
