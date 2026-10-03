#region

using System;
using System.Collections.Generic;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 동료 봇의 서버 기억 — 짝과 "이미 알렸다" 표시들. 모두 서버 메모리에만 있어 서버가 다시 뜨면 비어 있다.
    /// <see cref="ToldOwn" /> 만 제 자물쇠(사전 자신)를 쓰고 나머지는 <see cref="Gate" /> 로 잠근다.
    /// </summary>
    internal static class CompanionState
    {
        public static readonly object Gate = new object();

        // 봇 이름 → 부른 사람 이름. 둘 다 접속해 있는 동안만 산다.
        public static readonly Dictionary<string, string> OwnerOf =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        // 봇 이름 → 봇에게 마지막으로 알린 주인 serial. 주인이 로그아웃 없이 끊겼다 다시 들어오면(소켓이 아직 열려 있어 짝이
        // 풀리지 않은 채) serial 만 바뀐다 — 그때 다시 알린다(2026-09-27 클라우드: 봇이 옛 serial 을 쥔 채 서 있었다).
        public static readonly Dictionary<string, int> ToldMaster = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        // 쓰러졌다고 주인에게 이미 알린 봇.
        public static readonly HashSet<string> Fallen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 봇 이름 → (지금까지 가장 가까웠던 거리, 그때). 가까워지면 새로 적는다.
        public static readonly Dictionary<string, (int Best, DateTime Since)> Progress =
            new Dictionary<string, (int, DateTime)>(StringComparer.OrdinalIgnoreCase);

        // 짝 없는 봇 이름 → 대기 장소 밖에서 처음 본 때.
        public static readonly Dictionary<string, DateTime> IdleAway = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

        // 이번 수면에서 "못 푼다" 를 이미 알린 주인.
        public static readonly HashSet<string> ToldAsleep = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // 주인 이름 → 봇 탭 「마법사」 체크 비트(1 렌토 · 2 나르콜리 · 4 바르도 · 8 데프레코, 0xF1 6). 없으면 모두 켬. 주인 알림(0x5E 1) 꼬리로 봇에게 간다.
        public static readonly Dictionary<string, byte> Magic = new Dictionary<string, byte>(StringComparer.OrdinalIgnoreCase);

        // 사람마다 지난번에 알린 제 상태(이름·그림 목록) — 바뀔 때만 다시 보낸다.
        public static readonly Dictionary<int, string> ToldOwn = new Dictionary<int, string>();
    }
}
