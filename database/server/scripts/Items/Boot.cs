#region

using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Items
{
    [Script("Boot", "Dean")]
    public class Boot : ItemScript
    {
        public Boot(Item item) : base(item)
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

                client.Aisling.Boots = (byte) Item.Image;
                client.Aisling.BootColor = (byte) Item.Template.Color;
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

                // 5.99 서버(Novaonline.exe 0x41d387): 공격모션 132(주먹) 옷 — 도복 — 을 입고는 신발을 신지 못한다.
                if (client.Aisling.EquipmentManager.Armor?.Item?.Template?.AttackMotion == 132)
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

                client.Aisling.Boots = byte.MinValue;

                Item.RemoveModifiers(client);
            }
        }
    }
}