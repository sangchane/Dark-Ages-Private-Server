#region

using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Items
{
    [Script("Weapon", "Dean")]
    public class Weapon : ItemScript
    {
        public Weapon(Item item) : base(item)
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

                client.Aisling.Weapon = Item.Template.Image;
                client.Aisling.UsingTwoHanded = Item.Template.Flags.HasFlag(ItemFlags.TwoHanded);
                GearElements.Refresh(client.Aisling);
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

            if (Item.Template.Flags.HasFlag(ItemFlags.TwoHanded)
                && sprite is Aisling)
            {
                var obj = sprite as Aisling;
                if (obj.EquipmentManager.Shield != null)
                    if (!obj.EquipmentManager.RemoveFromExisting(obj.EquipmentManager.Shield.Slot))
                    {
                        obj.Client.SendMessage(0x02, "두 손을 모두 써야 하는 물건입니다.");
                        return;
                    }
            }

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
            if (sprite is Aisling)
            {
                var client = (sprite as Aisling).Client;

                if (Item.Template == null)
                    return;

                client.Aisling.Weapon = ushort.MinValue;
                client.Aisling.UsingTwoHanded = false;
                GearElements.Refresh(client.Aisling, displayslot);

                Item.RemoveModifiers(client);
            }
        }
    }

    /// <summary>
    /// 사람의 공격·방어 속성을 지금 걸친 장비로 다시 정한다 — 공격 = 목걸이, 방어 = 허리띠(사용자 2026-10-07 「공격 속성은 목걸이
    /// 방어 속성은 벨트로 결정되는거야」). 무기·옷의 수·토·풍·화는 수치만 바꾸고 속성을 주지 않는다(10-04 에는 무기·옷을 먼저 봤다).
    /// 원작 5.99 도 목걸이 공격속성만 본다(`0x415cff`).
    /// <paramref name="leaving" /> 는 지금 비우는 자리다 — 물건이 아직 남아 있다(EquipmentManager 가 UnEquipped 뒤에 비운다).
    /// </summary>
    internal static class GearElements
    {
        public static void Refresh(Aisling aisling, int leaving = 0)
        {
            var gear = aisling.EquipmentManager;
            if (gear == null)
                return;

            aisling.OffenseElement = Offense(gear.Necklace, leaving);
            aisling.DefenseElement = Defense(gear.Belt, leaving);
        }

        private static ElementManager.Element Offense(EquipmentSlot slot, int leaving) =>
            Worn(slot, leaving)?.OffenseElement ?? ElementManager.Element.None;

        private static ElementManager.Element Defense(EquipmentSlot slot, int leaving) =>
            Worn(slot, leaving)?.DefenseElement ?? ElementManager.Element.None;

        private static ItemTemplate Worn(EquipmentSlot slot, int leaving) =>
            slot?.Item == null || slot.Slot == leaving ? null : slot.Item.Template;
    }
}