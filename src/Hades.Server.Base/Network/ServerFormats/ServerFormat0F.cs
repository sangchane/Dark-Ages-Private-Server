#region

using Darkages.Types;

#endregion

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat0F : NetworkFormat
    {
        public ServerFormat0F(Item item) : this()
        {
            Item = item;
        }

        public ServerFormat0F()
        {
            Secured = true;
            Command = 0x0F;
        }

        public Item Item { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(Item.Slot);
            writer.Write(Item.DisplayImage);
            writer.Write(Item.Color);
            writer.WriteStringA(Item.DisplayName);
            writer.Write((uint) Item.Stacks);
            writer.Write((byte) Item.Stacks > 1);
            writer.Write(Item.Template.MaxDurability);
            writer.Write(Item.Durability);
            writer.Write((uint) 0x00);
            WriteNumbers(writer, Item);
        }

        /// <summary>
        /// 우리 확장(모바일 앱): 원작 끝 뒤에 표식 1 과 아이템 수치를 덧붙인다 — 앞부분은 그대로라 옛 앱도 읽는다. 소지품(0x0F)과
        /// 장비(0x37)가 같이 쓴다. 순서: 방어·명중·타격·힘·지능·지혜·체력·민첩·마법방어(short) · HP·MP·최소·최대 공격력(int) ·
        /// 요구 레벨·직업·직업 단계·무게·공격 속성·방어 속성(byte) · 값(uint) · 들어갈 장비 칸(byte, 0x37 칸 번호) ·
        /// 체력·마력 회복(int, 물약 — 2026-10-05 덧붙임, 옛 앱은 앞만 읽는다).
        /// 앱 WorldClient.ReadItemStats 가 읽는다.
        /// </summary>
        public static void WriteNumbers(NetworkPacketWriter writer, Item item) => WriteNumbers(writer, item.Template);

        /// <summary>상점 목록(0x2F ItemShopData)도 템플릿만으로 같은 수치를 쓴다.</summary>
        public static void WriteNumbers(NetworkPacketWriter writer, ItemTemplate t)
        {
            writer.Write((byte) 1);
            foreach (var modifier in new[] { t.AcModifer, t.HitModifer, t.DmgModifer, t.StrModifer, t.IntModifer,
                         t.WisModifer, t.ConModifer, t.DexModifer, t.MrModifer })
                writer.Write((short) Signed(modifier));
            writer.Write(Signed(t.HealthModifer));
            writer.Write(Signed(t.ManaModifer));
            writer.Write(t.DmgMin);
            writer.Write(t.DmgMax);
            writer.Write(t.LevelRequired);
            writer.Write((byte) t.Class);
            writer.Write((byte) t.StageRequired);
            writer.Write(t.CarryWeight);
            writer.Write((byte) t.OffenseElement);
            writer.Write((byte) t.DefenseElement);
            writer.Write(t.Value);
            // 템플릿 자료는 칸 번호를 EquipmentSlot 에 적는다(도복 2) — EquipSlot 은 비어 있다.
            writer.Write((byte) (t.EquipmentSlot > 0 ? t.EquipmentSlot : (int) t.EquipSlot));
            writer.Write(t.HealthRestore);
            writer.Write(t.ManaRestore);
        }

        private static int Signed(StatusOperator modifier) =>
            modifier == null ? 0 : modifier.Option == Operator.Remove ? -modifier.Value : modifier.Value;
    }
}