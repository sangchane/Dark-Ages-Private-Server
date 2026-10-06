#region

using System;
using System.Linq;
using System.Net;
using Darkages.Network.Game;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 생태계 봇 — 같은 기계의 봇 프로그램(<c>mobile/bots/Lod.EcoBots</c>, 서비스 lod-eco)이 접속해 혼자 사냥·장사하며 자라는
    /// 캐릭터(설계 <c>autopilot/eco-bots/</c>). 판단은 봇 프로그램이 하고, 서버는 봇 이름을 가려 같은 기계에서만 받고
    /// 순간이동 한 가지를 더 허락할 뿐이다. 이름은 설정 <c>EcoBots</c>.
    /// </summary>
    public static class EcoBots
    {
        /// <summary>설정 <c>EcoBots</c> 에 있는 이름. 동료 봇 이름과 겹치면 동료 쪽이다.</summary>
        public static bool IsEcoBot(string name) =>
            !string.IsNullOrEmpty(name)
            && (ServerContext.Config.EcoBots?.Any(n => string.Equals(n, name, StringComparison.OrdinalIgnoreCase)) ?? false)
            && !Companions.IsBot(name);

        /// <summary>생태계 봇 이름은 같은 기계에서만 로그인·만들기가 된다. 그 밖의 이름은 늘 참.</summary>
        public static bool MayEnter(string name, IPAddress remote) =>
            !IsEcoBot(name) || (remote != null && ProxyHunt.IsLoopback(remote));

        /// <summary>
        /// 0xF1 8 — 생태계 봇이 같은 기계에서 보냈을 때만 그 맵의 (x,y) 곁 빈 칸으로 옮긴다(사냥터·가게 사이를 걷지 않고 오간다 —
        /// playerbots 처럼). 그 밖에는 조용히 버린다.
        /// </summary>
        public static void Move(GameClient client, int map, int x, int y)
        {
            if (client?.Aisling == null || !IsEcoBot(client.Aisling.Username) || !MayEnter(client.Aisling.Username, Remote(client)))
                return;

            if (!ServerContext.GlobalMapCache.ContainsKey(map))
                return;

            client.TransitionToMap(map, new Position(x, y));
        }

        /// <summary>
        /// 0xF1 9 — 생태계 봇(성직자)이 혼수인 같은 그룹 파티원을 깨운다. 동료 봇의 주인 깨우기(0xF1 5)와 같은 일·같은 거리(2칸 안),
        /// 대상만 주인 대신 그룹원(그룹 사냥 결정 19 — 성직자가 혼수인 파티원을 깨워야 파티가 버틴다).
        /// </summary>
        public static void Wake(GameClient client, uint target)
        {
            var bot = client?.Aisling;
            if (bot == null || !IsEcoBot(bot.Username) || !MayEnter(bot.Username, Remote(client)) || bot.Dead || bot.Skulled
                || bot.GroupParty == null)
                return;

            var sleeper = bot.GroupParty.PartyMembers.FirstOrDefault(member => (uint) member.Serial == target);
            var reach = sleeper == null ? -1 : Math.Abs(sleeper.XPos - bot.XPos) + Math.Abs(sleeper.YPos - bot.YPos);
            if (sleeper is not { Skulled: true } || sleeper.Dead || sleeper.Client == null || sleeper.CurrentMapId != bot.CurrentMapId
                || reach < 1 || reach > 2)
                return;

            CompanionPairing.WakeUp(bot, sleeper);
            sleeper.Client.SendMessage(0x02, $"{bot.Username} 님이 당신을 깨웠습니다.");
        }

        private static IPAddress Remote(GameClient client)
        {
            try
            {
                return (client.Socket?.RemoteEndPoint as IPEndPoint)?.Address;
            }
            catch (ObjectDisposedException)
            {
                return null;
            }
        }
    }
}
