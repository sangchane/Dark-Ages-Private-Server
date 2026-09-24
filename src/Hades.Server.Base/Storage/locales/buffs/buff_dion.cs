#region

using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Buffs
{
    public class buff_dion : Buff
    {
        public override byte Icon => 53;
        public override int Length => 6;
        public override string Name => "dion";

        public override void OnApplied(Sprite Affected, Buff buff)
        {
            if (Affected is Aisling)
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "피부가 돌처럼 단단해집니다.");

            Affected.Immunity = true;

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
                    .SendMessage(0x02, "피부가 원래대로 돌아옵니다.");

            Affected.Immunity = false;

            base.OnEnded(Affected, buff);
        }
    }
}