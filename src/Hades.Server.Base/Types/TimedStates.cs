using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Darkages.Types
{
    /// <summary>
    /// 5.99 스크립트가 거는 시간 상태(호르라마 horrama · 에나르마 enare · 수페라에나르마 suenare · focus · phoenix · sosusin) —
    /// (serial, 이름) → 끝나는 때. 전에는 스크립트(<c>Pack599.States</c>) 안에만 있어 서버가 볼 수 없었다. 봇이 주인에게
    /// 버프가 실제로 있는지 알도록(<see cref="Companions" />, 0x5E 종류 3) 서버 쪽으로 옮겼다. 스크립트는 같은 사전을 쓴다.
    /// </summary>
    public static class TimedStates
    {
        public static readonly ConcurrentDictionary<(int, string), DateTime> All =
            new ConcurrentDictionary<(int, string), DateTime>();

        /// <summary>이 사람에게 지금 걸린 것과 남은 초.</summary>
        public static IEnumerable<(string Name, int Seconds)> Of(Sprite who)
        {
            var now = DateTime.UtcNow;

            return All.Where(pair => pair.Key.Item1 == who.Serial && pair.Value > now)
                .Select(pair => (pair.Key.Item2, (int) Math.Ceiling((pair.Value - now).TotalSeconds)));
        }
    }
}
