using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 이벤트가호 — 5.99 `Blessing.txt` 의 아이템 사용 스크립트를 그대로 옮긴 것. 아이템이 스스로 지운다(`item_del`).
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("ITEM_이벤트가호", "5.99표")]
    public class ItemC774BCA4D2B8AC00D638 : ItemScript
    {
        public ItemC774BCA4D2B8AC00D638(Item item) : base(item)
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
            V h_expba = 0;
            V h_exptime = 0;
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(h_exptime) > (V)((V)0L))))
            {
                p.Call("message", (V)1L, (V)"이미 가호를 받고있습니다.");
                return;
            }
            p.Call("exp_per", v_myid, (V)1800L, (V)2L, (V)3L);
            h_expba = (V)2L;
            p.Call("item_del", (V)"이벤트가호", (V)1L);
            p.Call("message", (V)1L, (V)"경험치획득량이 30분동안 2배가 됩니다.");
        }
    }
}
