#region

using System;
using System.Linq;
using Darkages.Common;
using Darkages.Network.ServerFormats;

#endregion

namespace Darkages.Types
{
    public class Money : Sprite
    {
        public int Amount { get; set; }

        public ushort Image { get; set; }
        public MoneySprites Type { get; set; }

        // 죽은 자리 금화 — 보호가 풀릴 때까지 AuthenticatedAislings 만 줍는다(Item 과 같은 꼴, 사용자 2026-10-11).
        public bool Cursed { get; set; }
        public Sprite[] AuthenticatedAislings { get; set; }
        public DateTime CurseLiftsAt { get; set; }

        public static Money Create(Sprite parent, int amount, Position location, bool join = true)
        {
            if (parent == null)
                return null;

            // The original stacks coins: money dropped where money already lies becomes one larger pile.
            // The sprite grade comes from the amount (CalcAmount), so adding to the old pile in place would
            // leave the wrong picture on the ground. Take what is there, add it in, and lay one pile down.
            // join=false — 이 칸의 남의 금화를 끌어오지 않고 따로 놓는다(죽은 자리 금화가 남의 금화까지 잠그지 않게).
            var lying = parent.GetObjects<Money>(parent.Map, m =>
                join &&
                m.CurrentMapId == parent.CurrentMapId &&
                m.XPos == location.X &&
                m.YPos == location.Y &&
                // 잠긴 죽은 자리 금화는 합치지 않는다 — 합친 새 더미는 잠금이 없어 남이 주워 간다(리뷰 H1).
                !(m.Cursed && DateTime.UtcNow < m.CurseLiftsAt)).ToArray();

            foreach (var pile in lying)
            {
                // A pile large enough to overflow is not something this server can carry anyway, and
                // silently wrapping to a negative amount would be worse than stopping at the ceiling.
                amount = (int) Math.Min((long) amount + pile.Amount, int.MaxValue);
                pile.Remove();
            }

            var money = new Money();
            money.CalcAmount(amount);

            lock (Generator.Random)
            {
                money.Serial = Generator.GenerateNumber();
            }

            money.AbandonedDate = DateTime.UtcNow;
            money.CurrentMapId = parent.CurrentMapId;
            money.XPos = location.X;
            money.YPos = location.Y;

            var mt = (int) money.Type;

            if (mt > 0) money.Image = (ushort) (mt + 0x8000);

            parent.AddObject(money);
            return money;
        }

        public void GiveTo(int amount, Aisling aisling)
        {
            if (aisling.GoldPoints + amount < ServerContext.Config.MaxCarryGold)
            {
                aisling.GoldPoints += amount;

                if (aisling.GoldPoints > ServerContext.Config.MaxCarryGold)
                    aisling.GoldPoints = int.MaxValue;

                aisling.Client.SendMessage(0x03, $"금전 {amount}전을 주웠습니다.");
                aisling.Client.Send(new ServerFormat08(aisling, StatusFlags.StructC));

                Remove();
            }
        }

        private void CalcAmount(int amount)
        {
            Amount = amount;

            if (Amount > 0 && Amount < 10)
                Type = MoneySprites.SilverCoin;

            if (Amount >= 10 && Amount < 100)
                Type = MoneySprites.GoldCoin;

            if (Amount >= 100 && Amount < 1000)
                Type = MoneySprites.SilverPile;

            if (Amount >= 1000)
                Type = MoneySprites.GoldPile;
        }
    }
}