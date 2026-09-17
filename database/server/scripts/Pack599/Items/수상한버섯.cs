using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 수상한버섯 — 5.99 `E.T.C.txt` 의 아이템 사용 스크립트를 그대로 옮긴 것. 아이템이 스스로 지운다(`item_del`).
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("ITEM_수상한버섯", "5.99표")]
    public class ItemC218C0C1D55CBC84C12F : ItemScript
    {
        public ItemC218C0C1D55CBC84C12F(Item item) : base(item)
        {
        }

        public override void Equipped(Sprite sprite, byte displayslot)
        {
        }

        public override void UnEquipped(Sprite sprite, byte displayslot)
        {
        }

        public override void OnUse(Sprite sprite, byte slot)
        {
            var p = new Pack599(sprite, null);
            if (!p.Ready)
                return;
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            p.Call("set_vital", (V)1L);
            p.Call("message", (V)3L, (V)"체력이 1이 되었습니다.");
        }
    }
}
