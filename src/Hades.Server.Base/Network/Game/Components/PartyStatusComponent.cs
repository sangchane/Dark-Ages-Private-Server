using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using Darkages.Types;

namespace Darkages.Network.Game.Components
{
    /// <summary>
    /// 1초마다 그룹원끼리 서로의 체력 %·마력 %·상태 그림과 체력·마력 숫자(현재·최대, 2026-09-27 — 앱이 게이지 안에 적는다)를 보낸다(우리 확장 0x5E 종류 6, 앱의 파티원 칸 — 2026-09-26). 원작은 그룹원의
    /// 체력을 보내지 않는다: 맞는 것이 보일 때의 0x13 백분율뿐이라 멀리 있거나 맞지 않는 그룹원의 막대는 비어 있었다. 그룹을 떠난
    /// 사람에게는 serial 0 을 한 번 보내 칸을 비우게 한다.
    /// </summary>
    public class PartyStatusComponent : GameServerComponent
    {
        // 지난번에 그룹원 소식을 받은 사람 — 그룹에서 빠지면 끝(serial 0)을 한 번 알린다.
        private readonly HashSet<int> _told = new HashSet<int>();

        public PartyStatusComponent(GameServer server) : base(server)
        {
            Timer = new GameServerTimer(TimeSpan.FromSeconds(1));
        }

        public GameServerTimer Timer { get; set; }

        protected internal override void Update(TimeSpan elapsedTime)
        {
            if (Timer.Update(elapsedTime))
                Tell();
        }

        private void Tell()
        {
            var online = GetObjects<Aisling>(null, a => a != null && a.LoggedIn && a.Client != null).ToList();
            var grouped = new HashSet<int>();

            foreach (var who in online)
            {
                var members = who.PartyMembers;

                if (members == null || members.Count < 2)
                    continue;

                grouped.Add(who.Serial);

                foreach (var other in members.Where(m => m != null && m.Serial != who.Serial))
                    who.Client.Send(ServerFormat5E.MemberStatus(other.Serial, other.Username, Percent(other.CurrentHp, other.MaximumHp),
                        Percent(other.CurrentMp, other.MaximumMp), IconsOf(other),
                        (other.CurrentHp, other.MaximumHp, other.CurrentMp, other.MaximumMp)));
            }

            foreach (var who in online.Where(a => _told.Contains(a.Serial) && !grouped.Contains(a.Serial)))
                who.Client.Send(ServerFormat5E.MemberStatus(0, string.Empty, 0, 0, Array.Empty<ushort>()));

            _told.Clear();
            _told.UnionWith(grouped);
        }

        /// <summary>그 사람에게 걸린 것의 그림 번호 — 하데스 버프·디버프와 5.99 시간 상태(그림을 아는 것만).</summary>
        private static IReadOnlyList<ushort> IconsOf(Sprite who) =>
            who.Buffs.Values.Where(b => b != null).Select(b => (ushort) b.Icon)
                .Concat(who.Debuffs.Values.Where(d => d != null).Select(d => (ushort) d.Icon))
                .Concat(TimedStates.Of(who).Select(s => TimedStates.IconOf(s.Name)))
                .Where(icon => icon != 0)
                .Distinct()
                .Take(byte.MaxValue)
                .ToList();

        private static byte Percent(int value, int maximum) =>
            (byte) (maximum > 0 ? Math.Clamp(100L * value / maximum, 0, 100) : 0);
    }
}
