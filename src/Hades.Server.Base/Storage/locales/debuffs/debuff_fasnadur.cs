#region

using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.debuffs
{
    public class debuff_fasnadur : Debuff
    {
        public override byte Icon => 119;
        public override int Length => 90;
        public override string Name => "fas nadur";

        public override void OnApplied(Sprite Affected, Debuff debuff)
        {
            base.OnApplied(Affected, debuff);

            Affected.Amplified = 1;
        }

        public override void OnDurationUpdate(Sprite Affected, Debuff debuff)
        {
            base.OnDurationUpdate(Affected, debuff);
        }

        public override void OnEnded(Sprite Affected, Debuff debuff)
        {
            Affected.Amplified = 0;

            if (Affected is Aisling)
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "원래대로 돌아왔습니다.");

            base.OnEnded(Affected, debuff);
        }
    }
}