#region

using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Items
{
    [Script("Armor", "Dean")]
    public class Armor : ItemScript
    {
        public Armor(Item item) : base(item)
        {
        }

        public override void Equipped(Sprite sprite, byte displayslot)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                if (Item.Template == null)
                    return;

                Item.ApplyModifers(client);

                client.Aisling.Pants = (byte) (Item.Template.HasPants ? 1 : 0);
                client.Aisling.Armor = Item.Image;
            }
        }

        public override void OnUse(Sprite sprite, byte slot)
        {
            if (sprite == null)
                return;
            if (Item == null)
                return;
            if (Item.Template == null)
                return;

            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                // 5.99 서버(Novaonline.exe 0x41cc49): 공격모션 132(주먹) 옷 — 도복 — 은 신발과 함께 입지 못한다.
                if (Item.Template.AttackMotion == 132 && client.Aisling.EquipmentManager[ItemSlots.Foot] != null)
                {
                    client.SendMessage(0x02, "신발이 불편하여 입을수가 없습니다.");
                    return;
                }

                if (Item.Template.Flags.HasFlag(ItemFlags.Equipable))
                    if (client.CheckReqs(client, Item))
                        client.Aisling.EquipmentManager.Add(Item.Template.EquipmentSlot, Item);
            }
        }

        public override void UnEquipped(Sprite sprite, byte displayslot)
        {
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                if (Item.Template == null)
                    return;

                client.Aisling.Pants = byte.MinValue;
                client.Aisling.Armor = ushort.MinValue;

                Item.RemoveModifiers(client);
            }
        }
    }
}