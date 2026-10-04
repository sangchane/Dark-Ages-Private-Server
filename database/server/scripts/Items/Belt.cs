#region

using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Items
{
    [Script("Belt", "Dean")]
    public class Belt : ItemScript
    {
        public Belt(Item item) : base(item)
        {
        }

        public override void Equipped(Sprite sprite, byte displayslot)
        {
            // 옷 속성이 먼저고, 없을 때 허리띠다(사용자 2026-10-04).
            GearElements.Refresh((Aisling) sprite);

            Item.ApplyModifers((sprite as Aisling).Client);
            (sprite as Aisling).Client.SendStats(StatusFlags.StructD);
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

                if (Item.Template.Flags.HasFlag(ItemFlags.Equipable))
                    if (client.CheckReqs(client, Item))
                        client.Aisling.EquipmentManager.Add(Item.Template.EquipmentSlot, Item);
            }
        }

        public override void UnEquipped(Sprite sprite, byte displayslot)
        {
            GearElements.Refresh((Aisling) sprite, displayslot);

            (sprite as Aisling).Client.SendStats(StatusFlags.StructD);
            Item.RemoveModifiers((sprite as Aisling).Client);
        }
    }
}