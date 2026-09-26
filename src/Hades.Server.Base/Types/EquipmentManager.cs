#region

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;


#endregion

namespace Darkages.Types
{
    public class EquipmentManager
    {
        public EquipmentManager(GameClient client)
        {
            Client = client;
            Equipment = new Dictionary<int, EquipmentSlot>();

            for (byte i = 1; i < 18; i++)
                Equipment[i] = null;
        }

        public EquipmentSlot Belt => this[ItemSlots.Waist];
        public EquipmentSlot Boots => this[ItemSlots.Foot];
        [JsonIgnore] public GameClient Client { get; set; }
        public EquipmentSlot DisplayHelm => this[ItemSlots.Coat];
        public Dictionary<int, EquipmentSlot> Equipment { get; set; }
        public EquipmentSlot Greaves => this[ItemSlots.Leg];
        [JsonIgnore] public int Length => Equipment?.Count ?? 0;
        public EquipmentSlot LGauntlet => this[ItemSlots.LArm];
        public EquipmentSlot LRing => this[ItemSlots.LHand];
        public EquipmentSlot Overcoat => this[ItemSlots.Trousers];
        public EquipmentSlot RGauntlet => this[ItemSlots.RArm];
        public EquipmentSlot RRing => this[ItemSlots.RHand];
        public EquipmentSlot Armor => this[ItemSlots.Armor];
        public EquipmentSlot Earring => this[ItemSlots.Earring];
        public EquipmentSlot FirstAcc => this[ItemSlots.FirstAcc];
        public EquipmentSlot Helmet => this[ItemSlots.Helmet];
        public EquipmentSlot Necklace => this[ItemSlots.Necklace];
        public EquipmentSlot SecondAcc => this[ItemSlots.SecondAcc];
        public EquipmentSlot Shield => this[ItemSlots.Shield];
        public EquipmentSlot Weapon => this[ItemSlots.Weapon];
        public EquipmentSlot this[byte idx] => Equipment.ContainsKey(idx) ? Equipment[idx] : null;

        public void Add(int displayslot, Item item)
        {
            if (Client == null)
                return;

            if (displayslot <= 0 || displayslot > 17)
                return;

            if (item?.Template == null)
                return;

            if (!item.Template.Flags.HasFlag(ItemFlags.Equipable))
                return;

            if (Equipment == null)
                Equipment = new Dictionary<int, EquipmentSlot>();

            if (RemoveFromExisting(displayslot)) AddEquipment(displayslot, item);
        }

        public void AddEquipment(int displayslot, Item item, bool remove = true)
        {
            Equipment[displayslot] = new EquipmentSlot(displayslot, item);

            if (remove)
                RemoveFromInventory(item);

            DisplayToEquipment((byte) displayslot, item);

            OnEquipmentAdded((byte) displayslot);
        }

        public void DecreaseDurability()
        {
            var broken = new List<Item>();

            foreach (var item in Equipment.Select(equipment => equipment.Value?.Item)
                .Where(item => item?.Template != null))
            {
                if (item.Template.Flags.HasFlag(ItemFlags.Repairable))
                {
                    item.Durability--;

                    if (item.Durability <= 0)
                        item.Durability = 0;
                }

                ManageDurabilitySignals(item);

                if (item.Durability == 0)
                    broken.Add(item);
            }

            foreach (var item in broken.Where(item => item?.Template != null)
                .Where(item => RemoveFromExisting(item.Template.EquipmentSlot, false)))
            {
                // RemoveFromExisting(.., false) 가 무게를 깎지만 그 앞에서 이미 한 번 보낸 상태값(OnEquipmentRemoved)
                // 에는 깎이기 전 무게가 실려 있다 — 다시 보내야 클라이언트가 줄어든 무게를 본다.
                Client.SendStats(StatusFlags.StructA);
                Client.SendMessage(0x02,
                    $"{item.Template.Name}이(가) 부서졌습니다.");
            }
        }

        public void DisplayToEquipment(byte displayslot, Item item)
        {
            if (item != null)
                Client.Send(new ServerFormat37(item, displayslot));
        }

        public bool RemoveFromExisting(int displayslot, bool returnit = true)
        {
            if (Equipment[displayslot] == null)
                return true;

            var itemObj = Equipment[displayslot].Item;

            if (itemObj == null)
                return false;

            RemoveFromSlot(displayslot);

            if (!returnit)
                return HandleUnreturnedItem(itemObj);

            return itemObj.GiveTo(Client.Aisling, false) || HandleUnreturnedItem(itemObj);
        }

        public bool RemoveFromInventory(Item item, bool handleWeight = false)
        {
            if (item != null && Client.Aisling.Inventory.Remove(item.Slot) == null)
                return true;

            if (item != null)
            {
                Client.Send(new ServerFormat10(item.Slot));

                if (handleWeight)
                {
                    Client.Aisling.CurrentWeight -= item.Template.CarryWeight;
                    if (Client.Aisling.CurrentWeight < 0)
                        Client.Aisling.CurrentWeight = 0;

                    Client.SendStats(StatusFlags.StructA);
                }

                Client.LastItemDropped = item;
            }

            return true;
        }

        private bool HandleUnreturnedItem(Item itemObj)
        {
            if (itemObj == null)
                return true;

            Client.Aisling.CurrentWeight -= itemObj.Template.CarryWeight;

            if (Client.Aisling.CurrentWeight < 0)
                Client.Aisling.CurrentWeight = 0;

            Client.DelObject(itemObj);

            return true;
        }

        private void ManageDurabilitySignals(Item item)
        {
            if (item.Durability > item.Template.MaxDurability)
                item.Template.MaxDurability = item.Durability;

            var p10 = Math.Abs(item.Durability * 100 / item.Template.MaxDurability);

            if (item.Warnings == null || item.Warnings.Length <= 0)
                return;

            if (p10 <= 10 && !item.Warnings[0])
            {
                Client.SendMessage(0x02,
                    $"{item.Template.Name}이(가) 곧 부서집니다. 어서 고치십시오. (10% 미만)");
                item.Warnings[0] = true;
            }
            else if (p10 <= 30 && p10 > 10 && !item.Warnings[1])
            {
                Client.SendMessage(0x02,
                    $"{item.Template.Name}이(가) 많이 닳았습니다. 되도록 빨리 고치십시오. (30% 미만)");
                item.Warnings[1] = true;
            }
            else if (p10 <= 50 && p10 > 30 && !item.Warnings[2])
            {
                Client.SendMessage(0x02, $"{item.Template.Name}을(를) 곧 고쳐야 합니다. (50% 미만)");
                item.Warnings[2] = true;
            }
        }

        private void OnEquipmentAdded(byte displayslot)
        {
            var scripts = Equipment[displayslot].Item?.Scripts;
            if (scripts != null)
            {
                var scriptsValues = scripts?.Values;
                if (scriptsValues != null)
                    foreach (var script in scriptsValues)
                        script.Equipped(Client.Aisling, displayslot);
            }

            var item = Equipment[displayslot].Item;
            if (item != null) item.Equipped = true;

            Client.SendStats(StatusFlags.All);
            Client.UpdateDisplay();
        }

        private void OnEquipmentRemoved(byte displayslot)
        {
            if (Equipment[displayslot] == null)
                return;

            var itemScripts
                = Equipment[displayslot].Item?.Scripts;
            if (itemScripts != null)
            {
                var scripts = itemScripts?.Values;
                if (scripts != null)
                    foreach (var script in scripts)
                        script.UnEquipped(Client.Aisling, displayslot);
            }

            var item = Equipment[displayslot].Item;

            if (item != null)
                item.Equipped = false;

            Client.SendStats(StatusFlags.All);
            Client.UpdateDisplay();
        }

        /// <summary>
        /// 자리에서 벗겨 그 물건을 돌려준다 — 가방에 넣지도 지우지도 않는다(봇 장비를 주인 가방으로 옮길 때, Companions).
        /// </summary>
        public Item TakeOff(int displayslot)
        {
            if (displayslot <= 0 || displayslot > 17 || Equipment[displayslot]?.Item == null)
                return null;

            var item = Equipment[displayslot].Item;
            RemoveFromSlot(displayslot);
            return item;
        }

        #region Core Methods

        private void RemoveFromSlot(int displayslot)
        {
            Client.Aisling.Show(Scope.Self, new ServerFormat38((byte) displayslot));

            OnEquipmentRemoved((byte) displayslot);

            Equipment[displayslot] = null;
        }

        #endregion
    }
}