using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Scripting;
using Darkages.Systems;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 5.99 가 1초마다 도는 스크립트(`script/Dungeon.txt` 의 `Dungeon__Script`)를 개인 사본 맵에서 돌린다. 사본 안에 선 사람마다 한 번씩,
    /// 그 사람을 `get_myid` 로 — 보스방 괴물이 다 죽었는지 세고, 5초를 센 뒤 경험치를 주고 내보낸다. `scripts/build-pack-npcs.py` 가
    /// 스크립트 본문을 이것을 상속한 클래스로 옮긴다(`map_create` 가 사본에 붙인다).
    /// </summary>
    /// <remarks>
    /// 사람이 들어왔다가 1분 동안 없는 사본은 없앤다(<see cref="Instances.Remove" />). 한 던전의 방 셋(오솔길·대기실·보스방)은 입장할 때
    /// 한꺼번에 지어지므로, 아직 아무도 안 들어간 방을 같은 1분으로 치우면 오솔길을 걷는 사이 보스방이 사라진다 — 그런 방은 30분을 둔다.
    /// 5.99 가 언제 없애는지는 팩에 없다.
    /// </remarks>
    public abstract class PackRoutine : AreaScript
    {
        private static readonly TimeSpan Every = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan EmptyFor = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan NeverVisitedFor = TimeSpan.FromMinutes(30);

        private TimeSpan _waited;
        private DateTime? _emptySince;
        private bool _visited;

        protected PackRoutine(Area area) : base(area)
        {
        }

        /// <summary>스크립트 본문. `end` 가 <c>yield break</c> 라 이터레이터다 — 돌리는 쪽이 끝까지 훑는다.</summary>
        protected abstract IEnumerable<int> Run(Pack599 p);

        public override void Update(TimeSpan elapsedTime)
        {
            _waited += elapsedTime;
            if (_waited < Every)
                return;
            _waited = TimeSpan.Zero;

            var people = Area.GetObjects<Aisling>(Area, person => person.LoggedIn && person.CurrentMapId == Area.Id).ToList();

            if (people.Count == 0)
            {
                _emptySince ??= DateTime.UtcNow;
                if (DateTime.UtcNow - _emptySince.Value > (_visited ? EmptyFor : NeverVisitedFor))
                    Instances.Remove(Area);
                return;
            }

            _emptySince = null;
            _visited = true;

            foreach (var person in people)
            {
                try
                {
                    foreach (var _ in Run(new Pack599(person, null)))
                    {
                    }
                }
                catch (Exception error)
                {
                    ServerContext.Logger($"[5.99] {Area.Name} 던전 스크립트가 멈췄습니다: {error.Message}");
                }
            }
        }
    }
}
