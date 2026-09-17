#region

using System;
using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Items
{
    /// <summary>
    /// 5.99 소모품 — 물약·음식·귀환 주문서. 5.99 는 이것들을 스크립트가 아니라 아이템 칸으로 움직인다: 체력변화·마력변화는
    /// 쓰면 그만큼 돌아오고(최대치를 넘지 않는다), 이동맵·이동좌표는 쓰면 그리로 옮긴다. 쓰고 나면 하나가 줄어드는 것은
    /// <c>Format1CHandler</c> 가 한다(Stackable·Consumable 깃발). 템플릿은 scripts/build-pack-consumables.py 가 쓴다.
    /// </summary>
    [Script("Consumable")]
    public class Consumable : ItemScript
    {
        public Consumable(Item item) : base(item)
        {
        }

        public override void Equipped(Sprite sprite, byte displayslot)
        {
        }

        public override void OnUse(Sprite sprite, byte slot)
        {
            if (sprite is not Aisling aisling || Item?.Template is not { } template)
                return;

            if (template.HealthRestore != 0)
                aisling.CurrentHp = Math.Clamp(aisling.CurrentHp + template.HealthRestore, 0, aisling.MaximumHp);

            if (template.ManaRestore != 0)
                aisling.CurrentMp = Math.Clamp(aisling.CurrentMp + template.ManaRestore, 0, aisling.MaximumMp);

            aisling.Client.SendStats(StatusFlags.StructB);

            if (template.RecallArea > 0)
                aisling.Client.TransitionToMap(template.RecallArea, new Position(template.RecallX, template.RecallY));
        }

        public override void UnEquipped(Sprite sprite, byte displayslot)
        {
        }
    }
}
