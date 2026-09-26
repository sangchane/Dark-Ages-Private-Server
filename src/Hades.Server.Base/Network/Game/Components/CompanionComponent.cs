using System;
using Darkages.Types;

namespace Darkages.Network.Game.Components
{
    /// <summary>1초마다 동료 짝을 살핀다 — 누가 나갔나, 맵이 갈렸나(<see cref="Companions.Tick" />).</summary>
    public class CompanionComponent : GameServerComponent
    {
        public CompanionComponent(GameServer server) : base(server)
        {
            Timer = new GameServerTimer(TimeSpan.FromSeconds(1));
        }

        public GameServerTimer Timer { get; set; }

        protected internal override void Update(TimeSpan elapsedTime)
        {
            if (Timer.Update(elapsedTime))
                Companions.Tick();
        }
    }
}
