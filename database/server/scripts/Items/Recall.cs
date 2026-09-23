#region

using System;
using System.Linq;
using Darkages.Scripting;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Items
{
    /// <summary>
    /// 이름 없는 리콜 — 아무 마을로 돌아간다. 원작은 "마을 이름이 붙은 리콜은 그 마을의 정해진 자리로, 그냥 리콜은
    /// 정해지지 않은 마을의 정해진 자리로" 보낸다(사용자 설명). 어느 자리인지는 여기 적지 않는다 — 마을 이름 리콜
    /// 템플릿(<c>RecallArea</c>·<c>RecallX</c>·<c>RecallY</c>, scripts/build-pack-consumables.py)을 그대로 빌린다.
    /// 쓰고 나면 하나가 줄어드는 것은 <c>Format1CHandler</c> 가 한다(Stackable·Consumable 깃발). 템플릿은 scripts/build-recall.py 가 쓴다.
    /// </summary>
    [Script("Recall")]
    public class Recall : ItemScript
    {
        /// <summary>
        /// 리콜이 고르는 마을 — 이 서버에서 걸어 닿는 마을만. **마을을 더하면 여기에 그 마을 리콜 이름 한 줄.**
        /// 목록은 여기 하나뿐이다.
        /// </summary>
        public static readonly string[] Villages =
        {
            "노비스마을리콜",
            "수오미마을리콜",
        };

        public Recall(Item item) : base(item)
        {
        }

        public override void Equipped(Sprite sprite, byte displayslot)
        {
        }

        public override void OnUse(Sprite sprite, byte slot)
        {
            if (sprite is not Aisling aisling)
                return;

            var places = Villages
                .Select(name => ServerContext.GlobalItemTemplateCache.TryGetValue(name, out var t) ? t : null)
                .Where(t => t is { RecallArea: > 0 })
                .ToArray();

            if (places.Length == 0)
                return;

            var place = places[Random.Shared.Next(places.Length)];
            aisling.Client.TransitionToMap(place.RecallArea, new Position(place.RecallX, place.RecallY));
        }

        public override void UnEquipped(Sprite sprite, byte displayslot)
        {
        }
    }
}
