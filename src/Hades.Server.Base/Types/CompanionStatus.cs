#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using static Darkages.Types.CompanionState;

#endregion

namespace Darkages.Types
{
    /// <summary>상태 알림 — 모두에게 제 상태 아이콘, 짝끼리 서로의 상태·체력, 봇이 못 푸는 수면 안내.</summary>
    public static class CompanionStatus
    {
        /// <summary>짝에게 서로의 상태를 — 봇에게 주인·제 상태, 주인에게 봇의 상태와 체력·마력(앱의 봇 칸, 2026-09-26).</summary>
        internal static void TellPair(Aisling bot, Aisling owner)
        {
            bot.Client.Send(ServerFormat5E.Status(owner.Serial, StatusesOf(owner)));
            bot.Client.Send(ServerFormat5E.Status(bot.Serial, StatusesOf(bot)));

            // 주인에게도 봇의 상태를 — 앱의 봇 칸 상태 아이콘 줄(2026-09-26).
            owner.Client.Send(ServerFormat5E.Status(bot.Serial, StatusesOf(bot)));
            owner.Client.Send(ServerFormat5E.Life(bot.Serial, Percent(bot.CurrentHp, bot.MaximumHp),
                Percent(bot.CurrentMp, bot.MaximumMp), (bot.CurrentHp, bot.MaximumHp, bot.CurrentMp, bot.MaximumMp)));
        }

        /// <summary>
        /// 주인이 잠들었는데 봇이 디나르콜리를 아직 못 배운 레벨(사범 21레벨)이면 주인에게 한 줄 — 잠들 때마다 한 번(사용자, 2026-09-26
        /// "알람 띄워"). 서버가 보낸다: 주인의 수면도 봇의 마법책도 서버가 알고 있어 이것이 가장 작다(봇 프로그램·귓속말이 필요 없다).
        /// </summary>
        internal static void TellCannotWake(Aisling bot, Aisling owner)
        {
            if (!owner.HasDebuff("sleep"))
            {
                lock (Gate)
                    ToldAsleep.Remove(owner.Username);
                return;
            }

            if (bot.SpellBook.Spells.Values.Any(s => s?.Template?.Name == "디나르콜리"))
                return;

            bool first;
            lock (Gate)
                first = ToldAsleep.Add(owner.Username);

            if (first)
                owner.Client.SendMessage(0x02, "봇이 아직 수면을 풀지 못합니다 (21레벨부터)");
        }

        /// <summary>하데스 버프·디버프와 5.99 시간 상태 — 이름·남은 초·해로움·그림 번호(스펠 시트).</summary>
        private static List<(string Name, int Seconds, bool Harmful, ushort Icon)> StatusesOf(Sprite who)
        {
            var listed = new List<(string, int, bool, ushort)>();
            listed.AddRange(who.Buffs.Values.Where(b => b != null).Select(b => (b.Name, b.Length - b.Timer.Tick, false, (ushort) b.Icon)));
            listed.AddRange(who.Debuffs.Values.Where(d => d != null).Select(d => (d.Name, d.Length - d.Timer.Tick, true, (ushort) d.Icon)));
            listed.AddRange(TimedStates.Of(who).Select(s => (s.Name, s.Seconds, false, TimedStates.IconOf(s.Name))));

            // 유령 — 봇 프로그램이 멈춰 있다가 되살아나면 다시 돈다(원작 상태 칸이 아니라 우리 표시, 그림 없음).
            if (who is Aisling { Dead: true })
                listed.Add(("ghost", 0, true, (ushort) 0));

            return listed.Take(byte.MaxValue).ToList();
        }

        /// <summary>
        /// 모든 사람에게 제 상태를(0x5E 종류 3, 제 serial) — 앱이 내 판에 상태 아이콘 줄을 그린다(2026-09-26). 5.99 상태(호르라마·
        /// 에나르마)는 원작 상태 아이콘(0x3A)으로 오지 않아 이것 말고는 앱이 알 길이 없다. 무엇이 걸렸는지가 바뀔 때만 보낸다 —
        /// 남은 초는 앱이 등급으로만 쓰니 매초 보낼 까닭이 없다.
        /// </summary>
        internal static void TellEachTheirOwn()
        {
            var online = Companions.Finder.GetObjects<Aisling>(null, a => a != null && a.LoggedIn && a.Client != null).ToList();

            lock (ToldOwn)
            {
                foreach (var who in online)
                {
                    var listed = StatusesOf(who);
                    var said = string.Join(";", listed.Select(one => $"{one.Name}/{one.Icon}/{ServerStatusGrade(one.Seconds)}"));

                    if (ToldOwn.TryGetValue(who.Serial, out var before) ? before == said : said.Length == 0)
                        continue;

                    ToldOwn[who.Serial] = said;
                    who.Client.Send(ServerFormat5E.Status(who.Serial, listed));
                }

                foreach (var gone in ToldOwn.Keys.Where(serial => online.All(a => a.Serial != serial)).ToList())
                    ToldOwn.Remove(gone);
            }
        }

        /// <summary>앱이 쓰는 원작 시간 등급(90·60·30·20·10초) — 등급이 바뀔 때도 다시 알린다.</summary>
        private static int ServerStatusGrade(int seconds) =>
            seconds >= 90 ? 6 : seconds >= 60 ? 5 : seconds >= 30 ? 4 : seconds >= 20 ? 3 : seconds >= 10 ? 2 : 1;

        private static byte Percent(int value, int maximum) =>
            (byte) (maximum > 0 ? Math.Clamp(100L * value / maximum, 0, 100) : 0);
    }
}
