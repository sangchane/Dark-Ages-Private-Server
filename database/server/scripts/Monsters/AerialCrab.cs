#region

using System;
using System.Collections.Concurrent;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Monsters
{
    /// <summary>
    /// 에리얼 퀘스트 게길의 게(사용자 2026-10-11, <c>autopilot/aerial-quest/SPEC.md</c>). 사람을 때리지 않고 천천히 돌아다니다, 옆 칸(가로·세로
    /// 1칸 안)에 선 사람에게서 제물 한 벌(블루하콘무기·레드하콘다리 하나씩)을 빼앗는다. 킹아크퍼스의팬던트가 있으면 건드리지 않고, 제물이
    /// 없으면 3-D 입구로 쫓아낸다. 원작 블로그: 「게길 … 안걸리고 지나갈수있다」 · 「게길에서 실패하여 재료날렸음」.
    /// </summary>
    [Script("에리얼게", "에리얼퀘스트")]
    public class AerialCrab : MonsterScript
    {
        public const string Pendant = "킹아크퍼스의팬던트";
        public static readonly string[] Offering = { "블루하콘무기", "레드하콘다리" };
        public const string Shore = "뤼케시온해안3-D";
        public static readonly Position ShoreGate = new Position(55, 25);

        /// <summary>한 사람을 다시 집기까지 — 옆에 선 채로 한꺼번에 다 털리지 않게.</summary>
        private static readonly TimeSpan Gap = TimeSpan.FromSeconds(3);

        private static readonly ConcurrentDictionary<int, DateTime> Pinched = new ConcurrentDictionary<int, DateTime>();

        public AerialCrab(Monster monster, Area map) : base(monster, map)
        {
        }

        /// <summary>제물이 몇 벌인가 — 두 재료 중 적은 쪽.</summary>
        public static int Sets(Aisling who) =>
            Offering.Min(name => who.Inventory.Items.Values.Where(i => i?.Template?.Name == name).Sum(i => Math.Max(1, (int) i.Stacks)));

        public static bool HasPendant(Aisling who) =>
            who.Inventory.Items.Values.Any(i => i?.Template?.Name == Pendant);

        public override void Update(TimeSpan elapsedTime)
        {
            if (Monster == null || !Monster.IsAlive || Monster.Map == null)
                return;

            if (Monster.WalkTimer.Update(elapsedTime))
                Monster.Wander();

            foreach (var who in GetObjects<Aisling>(Monster.Map, a => a.LoggedIn && a.CurrentMapId == Monster.CurrentMapId
                         && Math.Max(Math.Abs(a.XPos - Monster.XPos), Math.Abs(a.YPos - Monster.YPos)) <= 1).ToList())
                Pinch(who);
        }

        private static void Pinch(Aisling who)
        {
            var now = DateTime.UtcNow;
            if (Pinched.TryGetValue(who.Serial, out var last) && now - last < Gap)
                return;
            Pinched[who.Serial] = now;

            if (HasPendant(who))
                return;

            if (Sets(who) == 0)
            {
                var shore = ServerContext.GlobalMapCache.Values.FirstOrDefault(map => map.Name == Shore);
                who.Client.SendMessage(0x03, "제물이 없어 게에게 쫓겨났습니다.");
                if (shore != null)
                    who.Client.TransitionToMap(shore, ShoreGate);
                return;
            }

            foreach (var name in Offering)
            {
                var item = who.Inventory.Items.Values.FirstOrDefault(i => i?.Template?.Name == name);
                if (item != null)
                    who.Inventory.RemoveRange(who.Client, item, 1);
            }

            who.Client.SendMessage(0x03, $"게가 제물(블루하콘무기·레드하콘다리)을 빼앗아 갔습니다. 남은 제물 {Sets(who)}벌");
        }

        public override void OnApproach(GameClient client)
        {
        }

        public override void OnAttacked(GameClient client)
        {
        }

        public override void OnCast(GameClient client)
        {
        }

        public override void OnClick(GameClient client)
        {
        }

        /// <summary>잡혀도 상은 없다 — 피하는 길이다.</summary>
        public override void OnDeath(GameClient client)
        {
            Monster.Remove();
            DelObject(Monster);
        }

        public override void OnLeave(GameClient client)
        {
        }

        public override void OnSkulled(GameClient client)
        {
        }
    }
}
