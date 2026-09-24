#region

using Darkages.Network.ServerFormats;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Buffs
{
    public class BuffAite : Buff
    {
        public override byte Icon => 11;
        public override int Length => 3000;
        public override string Name => "aite";

        public override void OnApplied(Sprite affected, Buff buff)
        {
            if (affected is Aisling)
                (affected as Aisling)
                    .Client
                    .SendMessage(0x02, "아이테! 몸이 금빛으로 빛나며 힘이 솟습니다.");

            base.OnApplied(affected, buff);
        }

        public override void OnDurationUpdate(Sprite affected, Buff buff)
        {
            affected.Show(Scope.NearbyAislings,
                new ServerFormat29((uint) affected.Serial,
                    (uint) affected.Serial, 168, 168, 100));

            base.OnDurationUpdate(affected, buff);
        }

        public override void OnEnded(Sprite affected, Buff buff)
        {
            if (affected is Aisling)
                (affected as Aisling)
                    .Client
                    .SendMessage(0x02, "아이테가 끝났습니다. 방어력이 원래대로 돌아옵니다.");

            base.OnEnded(affected, buff);
        }
    }
}