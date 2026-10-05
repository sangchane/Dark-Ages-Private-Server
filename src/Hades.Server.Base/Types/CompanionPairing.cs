#region

using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using static Darkages.Types.CompanionState;
using static Darkages.Types.Companions;

#endregion

namespace Darkages.Types
{
    /// <summary>동료 봇 짝 — 부르기·보내기, 주기적으로 짝 살피기(따라붙이기·돌려보내기), 혼수 깨우기.</summary>
    public static class CompanionPairing
    {
        /// <summary>같은 맵에서 이만큼 넘게 떨어지면(막혀서 못 따라오면) 옆으로 옮긴다 — 화면(시야) 밖이다.</summary>
        public const int CatchUpDistance = 12;

        /// <summary>
        /// 이만큼 동안 주인에게 한 칸도 더 가까워지지 못하면(벽·괴물에 걸렸거나, 벽 파일이 없는 맵이라 벽을 모르거나) 옆으로 옮긴다
        /// ("봇이 지형에 걸리면 잘 못 쫓아온다", 사용자 2026-09-26). 봇이 서는 거리(<see cref="CloseEnough" />) 안이면 재지 않는다.
        /// </summary>
        public static readonly TimeSpan StuckFor = TimeSpan.FromSeconds(3);

        /// <summary>봇 판단이 따라 걷기를 멈추는 거리(CompanionSettings.FollowFrom 기본 3) — 이 안이면 막힌 것이 아니다.</summary>
        public const int CloseEnough = 3;

        /// <summary>[봇 부르기]. 알림은 모두 부른 사람에게 한국어 한 줄로.</summary>
        public static void Call(Aisling caller)
        {
            if (caller?.Client == null || IsBot(caller.Username))
                return;

            Aisling bot;

            lock (Gate)
            {
                var mine = OwnerOf.FirstOrDefault(p => string.Equals(p.Value, caller.Username, StringComparison.OrdinalIgnoreCase)).Key;

                if (mine == null && caller.GroupParty != null && !caller.LeaderPrivileges)
                {
                    caller.Client.SendMessage(0x02, "그룹장만 봇을 부를 수 있습니다.");
                    return;
                }

                if (mine != null)
                {
                    // 이미 부른 봇이 혼수 끝에 죽어(유령) 뮤레칸에 가 있으면 다시 누른 [봇 부르기] 가 되살려 데려온다(사용자, 2026-09-26).
                    if (FindOnline(mine) is { Dead: true } fallen)
                    {
                        bot = fallen;
                    }
                    else
                    {
                        // 다시 들어온 주인은 serial 이 새로 나온다 — 짝은 이름으로 남아도 봇은 전 serial 을 찾고 있어 따라오지
                        // 못했다(2026-09-26 클라우드). 누를 때마다 지금 serial 로 다시 알린다.
                        if (FindOnline(mine) is { } tied)
                            Retell(tied, caller);

                        caller.Client.SendMessage(0x02, "이미 봇이 함께 있습니다.");
                        return;
                    }
                }
                else
                {
                    // 유령인 봇도 부를 수 있다 — 부르면 되살린다.
                    var online = (ServerContext.Config.CompanionBots ?? new List<string>())
                        .Select(FindOnline)
                        .Where(one => one != null)
                        .ToList();

                    bot = online.FirstOrDefault(one => !OwnerOf.ContainsKey(one.Username));

                    if (bot == null)
                    {
                        caller.Client.SendMessage(0x02, online.Any()
                            ? "봇이 모두 다른 분과 함께 있습니다."
                            : "지금 부를 수 있는 봇이 없습니다.");
                        return;
                    }

                    OwnerOf[bot.Username] = caller.Username;
                }

                Fallen.Remove(bot.Username);
            }

            if (bot.Dead)
            {
                bot.RemoveDebuff("skulled", true);
                bot.Client.Revive();
                caller.Client.SendMessage(0x02, $"봇 {bot.Username}님을 되살렸습니다.");
            }

            if (bot.GroupParty != null)
                Party.RemovePartyMember(bot);

            CompanionKit.Prepare(bot, LevelFor(caller.ExpLevel), caller);
            MoveBeside(bot, caller);
            Party.AddPartyMember(caller, bot);

            bot.Client.Send(MasterOf(caller));
            caller.Client.Send(new ServerFormat5E(ServerFormat5E.Companion, bot.Serial, bot.Username));
            lock (Gate)
                ToldMaster[bot.Username] = caller.Serial;
            CompanionKit.SendKit(bot, caller);
            caller.Client.SendMessage(0x02, $"봇 {bot.Username}님이 왔습니다 (Lv{bot.ExpLevel})");
        }

        /// <summary>이미 맺은 짝을 둘에게 지금 serial 로 다시 알린다 — 그룹이 갈렸으면 다시 넣고 곁으로 옮긴다.</summary>
        private static void Retell(Aisling bot, Aisling caller)
        {
            if (bot.GroupParty == null || bot.GroupParty != caller.GroupParty)
            {
                if (bot.GroupParty != null)
                    Party.RemovePartyMember(bot);

                MoveBeside(bot, caller);
                Party.AddPartyMember(caller, bot);
            }

            bot.Client.Send(MasterOf(caller));
            caller.Client.Send(new ServerFormat5E(ServerFormat5E.Companion, bot.Serial, bot.Username));
            lock (Gate)
                ToldMaster[bot.Username] = caller.Serial;
            CompanionKit.SendKit(bot, caller);
            ServerContext.Logger($"봇 {bot.Username}: 주인 {caller.Username} 을(를) 다시 알림 (serial {caller.Serial})");
        }

        /// <summary>봇에게 가는 주인 알림 — 이름 뒤에 주인이 봇 탭에서 고른 것(다섯 바이트)을 싣는다.</summary>
        private static ServerFormat5E MasterOf(Aisling caller)
        {
            lock (Gate)
                return new ServerFormat5E(ServerFormat5E.Master, caller.Serial, caller.Username)
                {
                    Orders = CompanionState.Orders.TryGetValue(caller.Username, out var orders) ? orders : new byte[] { 0x1F, 0x3F, 0, 0, 0, 0 }
                };
        }

        /// <summary>
        /// 봇 탭에서 고른 것(0xF1 6 — <c>ClientFormatF1.Orders</c>). 주인 이름으로 기억하고, 봇이 함께 있으면 주인 알림으로 곧 옮긴다.
        /// 서버 메모리에만 있다 — 앱이 기기에 남겨 두고 바꿀 때와 부를 때 보낸다.
        /// </summary>
        public static void SetOrders(Aisling caller, byte[] orders)
        {
            if (caller?.Client == null || IsBot(caller.Username) || orders is not { Length: 6 })
                return;

            lock (Gate)
                // 성직자 칸은 여섯 비트(콜라마·벨라르모 2026-10-04 — 네 비트로 잘라 둘이 봇에게 닿지 않았다), 여섯째는 회복 % (1~99, 0 = 봇 기본).
                CompanionState.Orders[caller.Username] = new[] { (byte) (orders[0] & 0x1F), (byte) (orders[1] & 0x3F), orders[2], orders[3],
                    (byte) (orders[4] <= 10 ? orders[4] : 0), (byte) (orders[5] < 100 ? orders[5] : 0) };

            if (CompanionOf(caller.Username) is { } name && FindOnline(name) is { Client: { } } bot)
                bot.Client.Send(MasterOf(caller));
        }

        /// <summary>[봇 보내기]. 파티에서 빼고 마을로.</summary>
        public static void Dismiss(Aisling caller)
        {
            if (caller?.Client == null)
                return;

            var name = CompanionOf(caller.Username);
            var bot = name == null ? null : FindOnline(name);

            if (name == null)
            {
                caller.Client.SendMessage(0x02, "함께 있는 봇이 없습니다.");
                return;
            }

            Release(name, bot, caller);
            caller.Client.SendMessage(0x02, $"봇 {name}님을 보냈습니다.");
        }

        /// <summary>
        /// 0.5초마다(<c>CompanionComponent</c>): 부른 사람이 나갔으면 봇을 보내고, 봇이 나갔으면 알리고, 맵이 갈렸거나 멀리
        /// 떨어졌으면 봇을 옆으로 옮긴다 — 봇은 워프 칸을 모르므로 주인이 워프로 사라지면 서버가 데려간다.
        /// </summary>
        public static void Tick()
        {
            CompanionStatus.TellEachTheirOwn();
            SendIdleHome();

            KeyValuePair<string, string>[] pairs;

            lock (Gate)
            {
                pairs = OwnerOf.ToArray();
            }

            foreach (var (botName, ownerName) in pairs)
            {
                var bot = FindOnline(botName);
                var owner = FindOnline(ownerName);

                if (bot == null || owner == null)
                {
                    if (owner != null)
                        owner.Client.SendMessage(0x02, $"봇 {botName}님이 떠났습니다.");

                    Release(botName, bot, owner);
                    continue;
                }

                // 쓰러진(유령) 봇은 데려오지 않는다 — 뮤레칸에서 [봇 부르기] 로 되살릴 때까지. 주인에게 한 번 알린다.
                if (bot.Dead)
                {
                    bool told;
                    lock (Gate)
                    {
                        told = !Fallen.Add(bot.Username);
                    }

                    if (!told)
                        owner.Client.SendMessage(0x02, $"봇 {bot.Username}님이 쓰러졌습니다 — [봇 부르기] 로 되살립니다.");
                }

                int toldSerial;
                lock (Gate)
                    toldSerial = ToldMaster.TryGetValue(botName, out var serial) ? serial : 0;

                if (toldSerial != owner.Serial)
                    Retell(bot, owner);

                CompanionStatus.TellPair(bot, owner);
                CompanionStatus.TellCannotWake(bot, owner);

                if (owner.Client.IsWarping || owner.Client.MapOpen || owner.Map == null || bot.Dead)
                    continue;

                if (bot.CurrentMapId != owner.CurrentMapId || Stuck(bot, owner))
                    MoveBeside(bot, owner);
            }
        }

        /// <summary>짝 없는 봇이 대기 장소 밖에서 이만큼 서 있으면 돌려보낸다 — 막 들어와 곧 불릴 봇을 쓸데없이 옮기지 않게 조금 기다린다.</summary>
        public static readonly TimeSpan IdleAwayFor = TimeSpan.FromSeconds(10);

        /// <summary>
        /// 짝 없는 봇은 대기 장소로(사용자 신고 2026-09-27 "앱 종료하고 다시 접속해도 봇이 마지막 자리에 좀비처럼 있다"). 짝은 서버 기억에만
        /// 있어 짝을 맺은 채 서버가 다시 뜨면 풀어 줄 틈(<see cref="Release" />)이 없고, 봇은 저장된 마지막 자리 — 주인 곁 — 로 들어와
        /// 주인 없이 서 있었다. 쓰러진(유령) 봇은 [봇 부르기] 로 되살릴 때까지 두므로 건드리지 않는다.
        /// </summary>
        private static void SendIdleHome()
        {
            var homeMap = HomeMap();
            var now = DateTime.UtcNow;

            foreach (var name in ServerContext.Config.CompanionBots ?? new List<string>())
            {
                bool paired;
                lock (Gate)
                    paired = OwnerOf.ContainsKey(name);

                var bot = paired ? null : FindOnline(name);

                if (bot == null || bot.Dead || bot.CurrentMapId == homeMap || bot.Client.IsWarping || bot.Client.MapOpen)
                {
                    lock (Gate)
                        IdleAway.Remove(name);
                    continue;
                }

                DateTime since;
                lock (Gate)
                {
                    if (!IdleAway.TryGetValue(name, out since))
                        IdleAway[name] = since = now;
                }

                if (now - since < IdleAwayFor)
                    continue;

                lock (Gate)
                    IdleAway.Remove(name);

                ServerContext.Logger($"봇 {name}: 짝 없이 맵 {bot.CurrentMapId} ({bot.XPos},{bot.YPos}) 에 있어 대기 장소로 보냄");
                GoHome(bot);
            }
        }

        private static int HomeMap() => ServerContext.Config.CompanionHomeMap > 0
            ? ServerContext.Config.CompanionHomeMap
            : ServerContext.Config.StartingMap;

        private static void GoHome(Aisling bot)
        {
            var home = ServerContext.Config.CompanionHomePosition ?? ServerContext.Config.StartingPosition;
            bot.Client.TransitionToMap(HomeMap(), new Position(home.X, home.Y));
        }

        /// <summary>
        /// 옮겨 줄 때인가 — <see cref="CatchUpDistance" /> 칸 넘게 떨어졌거나, <see cref="StuckFor" /> 동안 더 가까워지지 못했다
        /// (0.5초마다 부른다, Tick).
        /// </summary>
        private static bool Stuck(Aisling bot, Aisling owner)
        {
            var distance = bot.Position.DistanceFrom(owner.Position);
            var now = DateTime.UtcNow;

            lock (Gate)
            {
                if (distance <= CloseEnough)
                {
                    Progress.Remove(bot.Username);
                    return false;
                }

                if (distance > CatchUpDistance)
                {
                    Progress.Remove(bot.Username);
                    return true;
                }

                if (!Progress.TryGetValue(bot.Username, out var seen) || distance < seen.Best)
                {
                    Progress[bot.Username] = (distance, now);
                    return false;
                }

                if (now - seen.Since < StuckFor)
                    return false;

                Progress.Remove(bot.Username);
                return true;
            }
        }

        /// <summary>봇을 사람 곁 빈칸(남·동·서·북 차례)으로. 빈칸이 없으면 서버가 가까운 빈터를 찾는다.</summary>
        private static void MoveBeside(Aisling bot, Aisling owner)
        {
            var map = owner.Map;
            if (map == null || bot.Client == null)
                return;

            var spot = new[] { (0, 1), (1, 0), (-1, 0), (0, -1) }
                .Select(d => new Position(owner.XPos + d.Item1, owner.YPos + d.Item2))
                .FirstOrDefault(p => p.X < map.Cols && p.Y < map.Rows && !map.IsWall(p.X, p.Y) &&
                                     !Finder.GetObjects(map, s => s.XPos == p.X && s.YPos == p.Y,
                                         ObjectManager.Get.Aislings | ObjectManager.Get.Monsters | ObjectManager.Get.Mundanes).Any())
                       ?? owner.Position;

            bot.Client.TransitionToMap(map, spot);
        }

        /// <summary>짝을 푼다: 파티에서 빼고, 봇이 접속해 있으면 마을로 보내고, 둘에게 끝났다고 알린다.</summary>
        private static void Release(string botName, Aisling bot, Aisling owner)
        {
            lock (Gate)
            {
                OwnerOf.Remove(botName);
                ToldMaster.Remove(botName);
            }

            ServerContext.Logger($"봇 {botName}: 짝을 풂 (주인 {owner?.Username ?? "없음"}, 봇 {(bot == null ? "나감" : "접속")})");

            owner?.Client?.Send(new ServerFormat5E(ServerFormat5E.Companion, 0, string.Empty));

            if (bot?.Client == null)
                return;

            if (bot.GroupParty != null)
                Party.RemovePartyMember(bot);

            GoHome(bot);
            bot.Client.Send(new ServerFormat5E(ServerFormat5E.Master, 0, string.Empty));
        }

        // ── 혼수 깨우기 ──────────────────────────────────────────────────────

        /// <summary>
        /// 혼수인 봇을 깨운다(0xF1 4) — **코마디움 없이, 아무것도 쓰지 않고**(사용자, 2026-09-26 "코마 사지 않아도 사용 가능하게").
        /// 5.99 코마디움(<c>Item/Potion.txt</c>)처럼 앞에 선 사람에게 — 봇이 바로 옆 칸일 때만, 주인이 봇 쪽으로 돌아서서, 코마디움과
        /// 같은 결과(혼수가 풀리고 체력·마력 1000).
        /// </summary>
        public static void Wake(Aisling owner)
        {
            if (owner?.Client == null || CompanionOf(owner.Username) is not { } name || FindOnline(name) is not { } bot)
            {
                owner?.Client?.SendMessage(0x02, "함께 있는 봇이 없습니다.");
                return;
            }

            if (!bot.Skulled)
            {
                owner.Client.SendMessage(0x02, "봇이 혼수 상태가 아닙니다.");
                return;
            }

            var dx = bot.XPos - owner.XPos;
            var dy = bot.YPos - owner.YPos;
            if (bot.CurrentMapId != owner.CurrentMapId || Math.Abs(dx) + Math.Abs(dy) != 1)
            {
                owner.Client.SendMessage(0x02, "봇 바로 옆에 서야 코마디움을 쓸 수 있습니다.");
                return;
            }

            WakeUp(owner, bot);
            owner.Client.SendMessage(0x02, $"봇 {bot.Username}님을 깨웠습니다.");
        }

        /// <summary>
        /// 봇이 혼수인 주인을 깨운다(0xF1 5, 사용자 결정 2026-09-26 "내가 혼수에 빠지면 봇이 옆으로 와서 깨운다"). 아무나 못 쓰게
        /// 서버가 가린다: 부른 사람이 있는 봇 계정만, 봇이 살아 있고 혼수가 아닐 때, 주인이 혼수이고 바로 옆 칸일 때만.
        /// </summary>
        public static void WakeMaster(Aisling bot)
        {
            if (bot?.Client == null || !IsBot(bot.Username) || bot.Dead || bot.Skulled)
                return;

            string ownerName;
            lock (Gate)
            {
                if (!OwnerOf.TryGetValue(bot.Username, out ownerName))
                    return;
            }

            // 2칸 안까지 — 딱 맞닿은 칸만 받으면 봇이 믿는 자리와 조금만 어긋나도 말없이 거절해 봇이 깨우기만 되풀이했다(2026-10-05).
            // 거절하면 까닭을 남긴다.
            var target = FindOnline(ownerName);
            var reach = target == null ? -1 : Math.Abs(target.XPos - bot.XPos) + Math.Abs(target.YPos - bot.YPos);
            if (target is not { Skulled: true } owner || owner.Dead || owner.CurrentMapId != bot.CurrentMapId || reach < 1 || reach > 2)
            {
                ServerContext.Logger($"봇 {bot.Username}: 주인 깨우기 안 됨 — 혼수 {target?.Skulled} · 죽음 {target?.Dead} · " +
                                     $"맵 {target?.CurrentMapId}/{bot.CurrentMapId} · 거리 {reach}");
                return;
            }

            WakeUp(bot, owner);
            owner.Client.SendMessage(0x02, "봇이 당신을 깨웠습니다.");
        }

        /// <summary>
        /// 코마디움이 풀어 주는 것과 같은 일 — 깨우는 이가 대상 쪽으로 돌아서고, 혼수를 걷고, 체력·마력 1000(5.99 Item/Potion.txt 코마디움),
        /// 그림 5(속도 75). 아무것도 쓰지 않는다.
        /// </summary>
        private static void WakeUp(Aisling waker, Aisling sleeper)
        {
            var dx = sleeper.XPos - waker.XPos;
            var dy = sleeper.YPos - waker.YPos;
            waker.Direction = (byte) (dy < 0 ? 0 : dx > 0 ? 1 : dy > 0 ? 2 : 3);
            waker.Show(Scope.NearbyAislings, new ServerFormat11 { Serial = waker.Serial, Direction = waker.Direction });

            sleeper.RemoveDebuff("skulled", true);
            sleeper.CurrentHp = Math.Min(1000, sleeper.MaximumHp);
            sleeper.CurrentMp = Math.Min(1000, sleeper.MaximumMp);
            sleeper.Client.SendStats(StatusFlags.StructB);
            // 깨운 체력을 주변(봇)에게도 — 체력바는 맞을 때만 가서, 봇이 주인을 계속 0%(쓰러짐)로 보고 회복하지 않았다(2026-10-05).
            sleeper.Show(Scope.VeryNearbyAislings, new ServerFormat13
            {
                Serial = sleeper.Serial,
                Health = (ushort) (100.0 * sleeper.CurrentHp / Math.Max(1, sleeper.MaximumHp)),
                Sound = byte.MaxValue
            });
            waker.Show(Scope.NearbyAislings, new ServerFormat29((uint) waker.Serial, (uint) sleeper.Serial, 5, 0, 75));
        }
    }
}
