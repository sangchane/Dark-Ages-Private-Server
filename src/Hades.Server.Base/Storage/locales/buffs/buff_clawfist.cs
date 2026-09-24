#region

using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Buffs
{
    public class buff_clawfist : Buff
    {
        public override byte Icon => 13;
        public override int Length => 9;
        public override string Name => "Claw Fist";

        public override void OnApplied(Sprite Affected, Buff buff)
        {
            if (Affected is Aisling)
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "두 손에 힘이 깃듭니다!");

            Affected.EmpoweredAssail = true;

            base.OnApplied(Affected, buff);
        }

        public override void OnDurationUpdate(Sprite Affected, Buff buff)
        {
            base.OnDurationUpdate(Affected, buff);
        }

        public override void OnEnded(Sprite Affected, Buff buff)
        {
            if (Affected is Aisling)
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "두 손이 원래대로 돌아옵니다.");

            Affected.EmpoweredAssail = false;

            base.OnEnded(Affected, buff);
        }
    }
}