#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Object;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 동료 봇 — 사람을 따라다니며 회복·버프를 걸어 주는 성직자(사용자 결정 2026-09-26: 봇 프로그램이 성직자 캐릭터로
    /// 접속해 있고, 앱의 [동료 부르기] 단추(0xF1)로 부르며, 봇은 부른 사람 레벨에 맞춰 준비된다). 봇의 판단은 봇 프로그램
    /// (<c>mobile/bots/Lod.CompanionBot</c>)이 하고, 서버는 짝을 맺고 풀고(<see cref="CompanionPairing" />), 봇을 준비시키고
    /// (<see cref="CompanionKit" />), 상태를 알릴(<see cref="CompanionStatus" />) 뿐이다. 이 클래스는 셋이 함께 쓰는 조회만 둔다.
    /// </summary>
    public class Companions : ObjectManager
    {
        /// <summary>
        /// 봇이 받는 성직자 회복·버프 마법과 그 레벨 — 5.99 사범 NPC 메뉴의 레벨이다(<c>Npc_Skill.txt</c>: 소라카
        /// "신성력강화[11] 쿠러스[11] 호르라마[15]", 소라카2 "에나르마[21] … 쿠라노[21]", "쿠라노소[55] 쿠라누스[63]",
        /// "수페라쿠라노[83] 쿠라네라[87] … 엑스쿠라노[99] 엑스쿠라네라[99]"). 쿠로는 직업을 고를 때 받는다(<c>Npc_Quest.txt:219</c>).
        /// 해제 둘(디나르콜리 = 수면 sleep, 디소루마 = 빙결 frozen — 스크립트의 mobnar_end·mobsor_end)도 준다.
        /// 공격(홀리볼트)·무적(이모탈)은 봇의 일이 아니라 주지 않는다.
        /// 마법사 저주·나르콜리도 준다(사용자, 2026-10-03) — 렌토 11 · 바르도 41 · 나르콜리 41 · 데프레코 71 · 프라보 99(바르도·데프레코·프라보는
        /// 사용자 기준 — 5.99 사범 메뉴는 21·55). 저주는 한 칸이라 봇은 배운 것 중 가장 센 것 하나만 건다. 서버는 직업을 따지지 않고
        /// 쓰게 한다. 앱 봇 탭 「마법사」로 켜고 끈다.
        /// </summary>
        public static readonly (int Level, string Name)[] PriestSpells =
        {
            (1, "쿠로"),
            (11, "신성력강화"),
            (11, "쿠러스"),
            (15, "호르라마"),
            (21, "에나르마"),
            (21, "쿠라노"),
            (21, "디나르콜리"),
            (21, "디소루마"),
            (11, "렌토"),
            (41, "바르도"),
            (41, "나르콜리"),
            (71, "데프레코"),
            (99, "프라보"),
            (55, "쿠라노소"),
            (63, "쿠라누스"),
            (83, "수페라쿠라노"),
            (87, "쿠라네라"),
            (99, "엑스쿠라노"),
            (99, "엑스쿠라네라")
        };

        internal static readonly Companions Finder = new Companions();

        /// <summary>봇 레벨 = 부른 사람 − 2, 적어도 1 (사용자 결정).</summary>
        public static int LevelFor(int ownerLevel) => Math.Max(1, ownerLevel - 2);

        public static bool IsBot(string name) =>
            ServerContext.Config.CompanionBots?.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)) ?? false;

        /// <summary>이 사람이 부른 봇의 이름, 없으면 null.</summary>
        public static string CompanionOf(string owner)
        {
            lock (CompanionState.Gate)
            {
                return CompanionState.OwnerOf.FirstOrDefault(p => string.Equals(p.Value, owner, StringComparison.OrdinalIgnoreCase)).Key;
            }
        }

        /// <summary>
        /// 접속해 있는 그 이름의 캐릭터 — 세상(맵) 목록이 아니라 **접속 목록**에서 찾는다. 맵 목록은 사람이 맵을 옮기는 동안(월드맵 이동은
        /// 옛 맵에서 빼고 0.5초 뒤에 새 맵에 넣는다) 비고, 월드맵을 가장자리로 열면 심연(Abyss)에 들어 조회에서 빠져서, 봇 짝이 주인이
        /// 나간 줄 알고 풀렸다(2026-09-27 클라우드 11:02 "짝을 풂 (주인 없음)" — "월드맵 이동하면 봇을 부르지도 보내지도 못한다").
        /// </summary>
        internal static Aisling FindOnline(string name) =>
            ServerContext.Game?.Clients
                .Select(client => client?.Aisling)
                .FirstOrDefault(a => a != null && a.LoggedIn && a.Client != null &&
                                     string.Equals(a.Username, name, StringComparison.OrdinalIgnoreCase));
    }
}
