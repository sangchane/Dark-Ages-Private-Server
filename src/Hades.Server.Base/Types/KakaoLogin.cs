using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace Darkages.Types
{
    /// <summary>
    /// 게임 로그인을 카카오로(사용자 2026-10-11 「비밀번호 대신 카카오」, 명세 <c>autopilot/game-kakao-login/SPEC.md</c>).
    /// 앱은 비밀번호 칸에 게임 표(<c>g번호.만료.서명</c> — 홈페이지 서비스가 준다)를, 옛 캐릭터를 처음 이을 때는 <c>표|옛비밀번호</c> 를 싣는다.
    /// 표는 홈페이지 서비스가 확인한다(설정 <c>KakaoCheckUrl</c>) — 거부하면 다음 로그인부터 막힌다. 설정이 비면 끔(옛 방식 그대로).
    /// 같은 기계(봇 셋·시험)는 평문 비밀번호 그대로 — 대신 사냥 열쇠는 이보다 먼저 본다(<see cref="ProxyHunt" />).
    /// </summary>
    public static class KakaoLogin
    {
        public enum Verdict { Enter, Link, Refuse, PasswordAsBefore }

        /// <summary>확인 결과 — 들어올 수 있으면 카카오 회원번호, 아니면 그 사람에게 보일 문구.</summary>
        public readonly record struct Check(string KakaoId, string Refusal);

        public const string Expired = "카카오 로그인이 끝났습니다. 앱에서 다시 로그인해 주십시오.";
        public const string OldApp = "새 앱으로 카카오 로그인을 해 주십시오.";
        // 문구는 CP949 로 간다(원작 꼴) — 「—」처럼 거기 없는 글자는 「?」가 된다.
        public const string NotLinked = "아직 카카오에 이어지지 않은 캐릭터입니다. 「옛 캐릭터 잇기」로 옛 비밀번호를 한 번 넣어 주십시오.";
        public const string OtherOwner = "다른 카카오 계정의 캐릭터입니다.";
        public const string Pending = "관리자 승인을 기다리는 중입니다.";
        public const string Denied = "들어올 수 없는 카카오 계정입니다.";
        public const string Unreachable = "로그인 확인 서버에 닿지 못했습니다. 잠시 뒤 다시 해 주십시오.";
        public const string NoPassword = "카카오 계정은 비밀번호가 없습니다.";
        public const string LinkLocked = "옛 비밀번호를 여러 번 틀렸습니다. 10분 뒤에 다시 해 주십시오.";

        private static readonly Regex Token = new(@"^g\d{1,19}\.\d{1,11}\.[0-9a-f]{64}$", RegexOptions.CultureInvariant);
        private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(3) };
        // 꼴만 맞춘 가짜 표를 쏟아부어도 로그인 처리 줄이 다 묶이지 않게 한꺼번에 넷까지만 묻는다(리뷰 2026-10-11).
        private static readonly SemaphoreSlim Asking = new(4);
        // 잇기(옛 비밀번호)는 캐릭터마다 10분에 5번까지 틀릴 수 있다 — 허가된 사람이 남의 옛 캐릭터 비밀번호를 끝없이 맞춰 보지 못하게.
        private static readonly Dictionary<string, (DateTime Since, int Count)> LinkFailures = new(StringComparer.OrdinalIgnoreCase);
        private static readonly TimeSpan LinkWindow = TimeSpan.FromMinutes(10);

        public static bool LinkBlocked(string name, DateTime now)
        {
            lock (LinkFailures)
                return LinkFailures.TryGetValue(name, out var failed) && now - failed.Since < LinkWindow && failed.Count >= 5;
        }

        public static void LinkFailed(string name, DateTime now)
        {
            lock (LinkFailures)
            {
                if (LinkFailures.Count >= 4096)
                    foreach (var stale in LinkFailures.Where(kv => now - kv.Value.Since >= LinkWindow).Select(kv => kv.Key).ToArray())
                        LinkFailures.Remove(stale);
                LinkFailures[name] = LinkFailures.TryGetValue(name, out var failed) && now - failed.Since < LinkWindow
                    ? (failed.Since, failed.Count + 1) : (now, 1);
            }
        }

        public static bool Enabled => !string.IsNullOrEmpty(ServerContext.Config?.KakaoCheckUrl);

        /// <summary>표 확인 — 시험이 가짜로 바꾼다.</summary>
        public static Func<string, Check> Checker = token => Ask(ServerContext.Config.KakaoCheckUrl, token);

        /// <summary>비밀번호 칸을 (표, 옛 비밀번호)로 — 표에는 「|」가 없다. 옛 비밀번호가 없으면 null.</summary>
        public static (string Token, string OldPassword) Split(string field)
        {
            int bar = (field ?? "").IndexOf('|');
            return bar < 0 ? (field ?? "", null) : (field.Substring(0, bar), field.Substring(bar + 1));
        }

        /// <summary>로그인(0x03) — 캐릭터 주인(없으면 빈칸)과 비밀번호 칸으로. Link 면 부르는 쪽이 옛 비밀번호를 확인하고 주인을 묶는다.</summary>
        public static (Verdict Verdict, string Message, string KakaoId) Decide(string owner, string field, bool loopback, Func<string, Check> check)
        {
            var (token, old) = Split(field);
            if (!Token.IsMatch(token))
                return loopback ? (Verdict.PasswordAsBefore, null, null) : (Verdict.Refuse, OldApp, null);
            var result = check(token);
            if (result.KakaoId == null)
                return (Verdict.Refuse, result.Refusal, null);
            if (!string.IsNullOrEmpty(owner))
                return owner == result.KakaoId ? (Verdict.Enter, null, result.KakaoId) : (Verdict.Refuse, OtherOwner, null);
            return old == null ? (Verdict.Refuse, NotLinked, null) : (Verdict.Link, null, result.KakaoId);
        }

        /// <summary>만들기(0x02) — 밖에서는 표로만. (들여도 되나, 묶을 회원번호 — 같은 기계 평문이면 null, 거절 문구).</summary>
        public static (bool Allowed, string KakaoId, string Message) ForCreate(string field, bool loopback, Func<string, Check> check)
        {
            var (token, _) = Split(field);
            if (!Token.IsMatch(token))
                return loopback ? (true, null, null) : (false, null, OldApp);
            var result = check(token);
            return result.KakaoId == null ? (false, null, result.Refusal) : (true, result.KakaoId, null);
        }

        // ponytail: 로그인 처리 줄에서 기다린다(최대 3초, 한꺼번에 넷) — 로그인이 몰려 막히면 비동기로 바꾼다.
        public static Check Ask(string url, string token)
        {
            if (!Asking.Wait(TimeSpan.FromSeconds(3)))
                return new Check(null, Unreachable);
            try
            {
                using var content = new StringContent(JsonSerializer.Serialize(new { token }), Encoding.UTF8, "application/json");
                using var response = Http.PostAsync(url, content).GetAwaiter().GetResult();
                string body = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using var ok = JsonDocument.Parse(body);
                    string id = ok.RootElement.GetProperty("id").GetString();
                    return string.IsNullOrEmpty(id) ? new Check(null, Unreachable) : new Check(id, null);
                }
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    using var refused = JsonDocument.Parse(body);
                    return new Check(null, refused.RootElement.TryGetProperty("status", out var status) && status.GetString() == "pending" ? Pending : Denied);
                }
                return new Check(null, response.StatusCode == HttpStatusCode.Unauthorized ? Expired : Unreachable);
            }
            catch (Exception)
            {
                // 주소가 틀렸든 응답이 깨졌든 들이지 않는다 — 엉뚱한 「읽을 수 없는 캐릭터」 문구로 새지 않게 여기서 다 받는다.
                return new Check(null, Unreachable);
            }
            finally
            {
                Asking.Release();
            }
        }
    }
}
