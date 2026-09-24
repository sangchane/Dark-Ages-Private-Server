#region

using Darkages.Network.ServerFormats;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Buffs
{
    public class buff_hide : Buff
    {
        public override byte Icon => 10;
        public override int Length => 10;
        public override string Name => "Hide";

        public override void OnApplied(Sprite affected, Buff buff)
        {
            if (affected is Aisling)
            {
                var client = (affected as Aisling).Client;
                if (client.Aisling != null && !client.Aisling.Dead)
                {
                    client.Aisling.Invisible = true;

                    if (client.Aisling.Invisible) client.SendMessage(0x02, "그림자 속으로 몸을 숨깁니다.");

                    var sound = new ServerFormat13
                    {
                        Sound = 43
                    };

                    affected.Show(Scope.NearbyAislings, sound);
                    client.UpdateDisplay();

                    base.OnApplied(affected, buff);
                }
            }
        }

        public override void OnDurationUpdate(Sprite Affected, Buff buff)
        {
            Affected.Show(Scope.NearbyAislings,
                new ServerFormat29((uint) Affected.Serial,
                    (uint) Affected.Serial, 0, 0, 100));

            base.OnDurationUpdate(Affected, buff);
        }

        public override void OnEnded(Sprite Affected, Buff buff)
        {
            if (Affected is Aisling)
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "그림자 밖으로 모습을 드러냅니다.");
            {
                var client = (Affected as Aisling).Client;
                {
                    client.Aisling.Invisible = false;
                    client.UpdateDisplay();

                    base.OnEnded(Affected, buff);
                }
            }
        }
    }
}