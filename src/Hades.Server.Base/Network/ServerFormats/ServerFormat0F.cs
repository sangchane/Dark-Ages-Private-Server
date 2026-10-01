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

            // 우리 확장(모바일 앱): 원작 끝 뒤에 표식 1 과 아이템 수치를 덧붙인다 — 앞부분은 그대로라 옛 앱도 읽는다.
            // 순서: 방어·명중·타격·힘·지능·지혜·체력·민첩·마법방어(short) · HP·MP·최소·최대 공격력(int) ·
            // 요구 레벨·직업·직업 단계·무게·공격 속성·방어 속성(byte) · 값(uint). 앱 WorldClient.ReadPackItem 이 읽는다.
            var t = Item.Template;
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
        }

        private static int Signed(StatusOperator modifier) =>
            modifier == null ? 0 : modifier.Option == Operator.Remove ? -modifier.Value : modifier.Value;
    }
}