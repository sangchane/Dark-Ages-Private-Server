using System;
using Darkages.Types;

namespace Darkages.Network.Game.Components
{
    /// <summary>0.5초마다 봇 짝을 살핀다 — 누가 나갔나, 맵이 갈렸나(<see cref="Companions.Tick" />).</summary>
    public class CompanionComponent : GameServerComponent
    {
        public CompanionComponent(GameServer server) : base(server)
        {
            // 0.5초 — 주인이 잠들면(수면) 봇이 늦지 않게 알도록(사용자, 2026-09-26 "바로 못 푸는 경우가 많다").
            Timer = new GameServerTimer(TimeSpan.FromMilliseconds(500));
        }

        public GameServerTimer Timer { get; set; }

        protected internal override void Update(TimeSpan elapsedTime)
        {
            if (Timer.Update(elapsedTime))
                Companions.Tick();
        }
    }
}
