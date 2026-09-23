#region

using System;
using Darkages.Common;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;


#endregion

namespace Darkages.Types
{
    public class Debuff
    {
        public Debuff()
        {
            Timer = new GameServerTimer(TimeSpan.FromSeconds(1));
        }

          public ushort Animation { get; set; }
          public virtual bool Cancelled { get; set; }
          public virtual byte Icon { get; set; }
          public virtual int Length { get; set; }
          public virtual string Name { get; set; }
          public GameServerTimer Timer { get; set; }

        public void Display(Sprite affected)
        {
            var colorInt = 0;

            if ((Length - Timer.Tick).IsWithin(0, 10))
                colorInt = 1;
            else if ((Length - Timer.Tick).IsWithin(10, 20))
                colorInt = 2;
            else if ((Length - Timer.Tick).IsWithin(20, 30))
                colorInt = 3;
            else if ((Length - Timer.Tick).IsWithin(30, 60))
                colorInt = 4;
            else if ((Length - Timer.Tick).IsWithin(60, 90))
                colorInt = 5;
            else if ((Length - Timer.Tick).IsWithin(90, short.MaxValue))
                colorInt = 6;

            (affected as Aisling)?.Client
                .Send(new ServerFormat3A(Icon, (byte) colorInt));

            // 둘레 사람에게도 알린다(0x5C, 우리 확장) — 등급이 바뀔 때만. 매초 보내면 둘레가 시끄럽다.
            if (Grade != colorInt)
            {
                Grade = (byte) colorInt;
                ServerFormat5C.Tell(affected, Icon, Grade, true, Animation);
            }
        }

        /// <summary>
        /// The time grade last told to everybody else (0x5C): 6 is over ninety seconds … 1 under ten, 0 not yet told.
        /// </summary>
        public byte Grade { get; private set; }

        /// <summary>Tells everybody around again, as when the picture this debuff is drawn with becomes known.</summary>
        public void Retell(Sprite affected)
        {
            Grade = 0;
            Display(affected);
        }

        public bool Has(string name)
        {
            return Name.Equals(name);
        }

        public virtual void OnApplied(Sprite affected, Debuff debuff)
        {
            if (affected.Debuffs.TryAdd(debuff.Name, debuff)) Display(affected);
        }

        public virtual void OnDurationUpdate(Sprite affected, Debuff buff)
        {
            Display(affected);
        }

        public virtual void OnEnded(Sprite affected, Debuff debuff)
        {
            if (affected.Debuffs.TryRemove(debuff.Name, out var removed))
            {
                (affected as Aisling)?.Client
                    .Send(new ServerFormat3A(Icon, byte.MinValue));
                ServerFormat5C.Tell(affected, Icon, byte.MinValue, true, Animation);
            }
        }

        internal void Update(Sprite affected, TimeSpan elapsedTime)
        {
            if (Timer.Disabled)
                return;

            if (Timer.Update(elapsedTime))
            {
                if (Length - Timer.Tick > 0)
                    OnDurationUpdate(affected, this);
                else
                    OnEnded(affected, this);

                Timer.Tick++;
            }
        }
    }
}