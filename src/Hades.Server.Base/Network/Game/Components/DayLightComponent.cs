using System;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;

namespace Darkages.Server.Network.Game.Components
{
    public class DayLightComponent : GameServerComponent
    {
        private readonly GameServerTimer _timer;
        private byte _shade = 0;

        public DayLightComponent(GameServer server)
            : base(server)
        { 
            _timer = new GameServerTimer(TimeSpan.FromSeconds(20.0f));
        }

        protected internal override void Update(TimeSpan elapsedTime)
        {
            if (_timer.Update(elapsedTime))
            {
                var format20 = new ServerFormat20 {Shade = _shade};

                lock (Server.Clients)
                {
                    foreach (var client in Server.Clients)
                    {
                        client?.Send(format20);
                    }
                }

                // 원작 2005 클라이언트는 이 값을 2시간 단위 시각으로 읽어 min(2v, 24) 로 Light 표를 찾는다(0x53ccdc) —
                // 12 를 넘는 값은 모두 24시라 18 로 돌리면 하루 1/3 이 같은 밤에 멈췄다. 0~11 이 하루다.
                _shade += 1;
                _shade %= 12;
            }
        }
    }
}