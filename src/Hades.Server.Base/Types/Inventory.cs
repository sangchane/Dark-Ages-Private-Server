#region

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;


#endregion

namespace Darkages.Types
{
    public class Inventory : ObjectManager
    {
        [JsonIgnore] public ActivitySession Activity { get; set; }
        // 원작은 59칸. 모바일은 5탭 × 30칸 = 150칸(사용자 2026-10-05). 저장된 59칸 캐릭터는 생성자가 1~150 을 깔고 JSON 이 앞쪽만 채운다.
        public static readonly int LENGTH = 150;

        public Dictionary<int, Item> Items = new Dictionary<int, Item>();

        public Inventory()
        {
            for (var i = 0; i < LENGTH; i++) Items[i + 1] = null;
        }

        [JsonIgnore]
        public IEnumerable<byte> BankList => (Items?.Where(i =>
                i.Value != null && i.Value.Template != null && i.Value.Template.Flags.HasFlag(ItemFlags.Bankable)))
            .Select(i => i.Value.Slot);

        public int Length => Items.Count;

        public void Assign(Item Item)
        {
            if (Item != null) Set(Item);
        }

        public bool CanPickup(Aisling player, Item LpItem)
        {
            if (player == null || LpItem == null)
                return false;

            if (LpItem.Template == null)
                return false;

            return player.CurrentWeight + LpItem.Template.CarryWeight < player.MaximumWeight &&
                   FindEmpty() != byte.MaxValue;
        }

        public byte FindEmpty()
        {
            byte idx = 1;

            foreach (var slot in Items)
            {
                if (slot.Value == null)
                    return idx;

                idx++;
            }

            return byte.MaxValue;
        }

        public Item FindInSlot(int Slot)
        {
            if (Items.ContainsKey(Slot)) return Items[Slot];

            return null;
        }

        public new Item[] Get(Predicate<Item> prediate)
        {
            return Items.Values.Where(i => i != null && prediate(i)).ToArray();
        }

        public Item Has(Predicate<Item> prediate)
        {
            return Items.Values.FirstOrDefault(i => i != null && prediate(i));
        }

        public int Has(Template templateContext)
        {
            var items = Items.Where(i => i.Value != null && i.Value.Template.Name == templateContext.Name)
                .Select(i => i.Value).ToList();

            var anyItem = items.FirstOrDefault();

            if (anyItem?.Template == null)
                return 0;

            var result = anyItem.Template.CanStack ? items.Sum(i => i.Stacks) : items.Count;

            return result;
        }

        public int HasCount(Template templateContext)
        {
            var items = Items.Where(i => i.Value != null && i.Value.Template.Name == templateContext.Name)
                .Select(i => i.Value).ToList();

            return items.Count;
        }

        public void Remove(GameClient client, Item item)
        {
            if (item == null)
                return;

            if (Remove(item.Slot) != null) client.Send(new ServerFormat10(item.Slot));
        }

        public Item Remove(byte movingFrom)
        {
            using var mutation = ActivitySession.BeginMutation(Activity, "Inventory.Remove");
            if (Items.ContainsKey(movingFrom))
            {
                var copy = Items[movingFrom];
                Items[movingFrom] = null;
                Activity?.ItemsChanged();
                return copy;
            }

            return null;
        }

        public void RemoveRange(GameClient client, Item item, int range)
        {
            using var mutation = ActivitySession.BeginMutation(Activity, "Inventory.RemoveRange");
            var remaining = item.Stacks - range;

            if (remaining <= 0)
            {
                Remove(item.Slot);
                client.Send(new ServerFormat10(item.Slot));

                client.Aisling.CurrentWeight -= item.Template.CarryWeight;

                if (client.Aisling.CurrentWeight < 0)
                    client.Aisling.CurrentWeight = 0;

                client.SendStats(StatusFlags.StructA);
            }
            else
            {
                item.Stacks = (ushort) remaining; // byte 로 자르면 256 개 이상 묶음이 줄어든다(은행에 일부 맡기기)
                client.Aisling.Inventory.Set(item, false);

                client.Send(new ServerFormat0F(item));
            }
        }

        public void Set(Item s)
        {
            using var mutation = ActivitySession.BeginMutation(Activity, "Inventory.Set");
            if (s == null)
                return;

            if (Items.ContainsKey(s.Slot)) { Items[s.Slot] = Clone<Item>(s); Activity?.ItemsChanged(); }
        }

        public void Set(Item s, bool clone = false)
        {
            using var mutation = ActivitySession.BeginMutation(Activity, "Inventory.Set");
            if (s == null)
                return;

            if (Items.ContainsKey(s.Slot)) { Items[s.Slot] = clone ? Clone<Item>(s) : s; Activity?.ItemsChanged(); }
        }

        public void UpdateSlot(GameClient client, Item item)
        {
            client.Send(new ServerFormat0F(item));
        }
    }
}