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

        // 상태 이름 → 그것을 거는 5.99 마법(그 템플릿의 Icon 이 상태 아이콘이다 — 앱의 상태 아이콘 줄, 0x5E 종류 3 뒤의 그림 번호).
        private static readonly Dictionary<string, string> CastBy = new Dictionary<string, string>
        {
            ["horrama"] = "호르라마",
            ["enare"] = "에나르마",
            ["suenare"] = "수페라에나르마",
        };

        /// <summary>상태의 그림 번호(스펠 시트) — 거는 마법의 템플릿 Icon. 모르면 0(앱이 그리지 않는다).</summary>
        public static ushort IconOf(string name) =>
            name != null && CastBy.TryGetValue(name, out var spell)
                         && ServerContext.GlobalSpellTemplateCache.TryGetValue(spell, out var template) && template != null
                ? template.Icon
                : (ushort) 0;

        /// <summary>이 사람에게 지금 걸린 것과 남은 초.</summary>
        public static IEnumerable<(string Name, int Seconds)> Of(Sprite who)
        {
            var now = DateTime.UtcNow;

            return All.Where(pair => pair.Key.Item1 == who.Serial && pair.Value > now)
                .Select(pair => (pair.Key.Item2, (int) Math.Ceiling((pair.Value - now).TotalSeconds)));
        }
    }
}
