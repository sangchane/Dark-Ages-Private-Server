#region

using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Buffs
{
    public class buff_armachd : Buff
    {
        public buff_armachd()
        {
            Name = "armachd";
            Length = 60;
            Icon = 0;
        }

        public StatusOperator AcModifer => new StatusOperator(Operator.Remove, 25);

        public override void OnApplied(Sprite Affected, Buff buff)
        {
            if (AcModifer.Option == Operator.Add)
                Affected.BonusAc += AcModifer.Value;
            else if (AcModifer.Option == Operator.Remove)
                Affected.BonusAc -= AcModifer.Value;

            if (Affected is Aisling)
            {
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "방어력이 올랐습니다.");
                (Affected as Aisling)
                    .Client.SendStats(StatusFlags.All);
            }

            base.OnApplied(Affected, buff);
        }

        public override void OnDurationUpdate(Sprite Affected, Buff buff)
        {
            base.OnDurationUpdate(Affected, buff);
        }

        public override void OnEnded(Sprite Affected, Buff buff)
        {
            if (AcModifer.Option == Operator.Add)
                Affected.BonusAc -= AcModifer.Value;
            else if (AcModifer.Option == Operator.Remove)
                Affected.BonusAc += AcModifer.Value;

            if (Affected is Aisling)
            {
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "방어력이 원래대로 돌아옵니다.");
                (Affected as Aisling)
                    .Client.SendStats(StatusFlags.All);
            }

            base.OnEnded(Affected, buff);
        }
    }
}