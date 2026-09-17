using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 엑스코마디움 — 5.99 `Potion.txt` 의 아이템 사용 스크립트를 그대로 옮긴 것. 아이템이 스스로 지운다(`item_del`).
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("ITEM_엑스코마디움", "5.99표")]
    public class ItemC5D1C2A4CF54B9C8B514C6C0 : ItemScript
    {
        public ItemC5D1C2A4CF54B9C8B514C6C0(Item item) : base(item)
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
            p.Call("item_del", (V)"엑스코마디움", (V)1L);
            if (V.T(V.B(V.T(((V)(p.Call("get_state1", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_coma", v_myid)) == (V)((V)1L))))))
            {
                p.Call("set_state1", v_myid, (V)0L);
                p.Call("set_coma", v_myid, (V)0L);
                p.Call("coma_delay", v_myid, (V)0L);
                p.Call("set_vita", v_myid, (V)1000L);
                p.Call("set_mana", v_myid, (V)1000L);
                p.Call("del_coma", v_myid);
                p.Call("effect", v_myid, (V)5L, (V)0L, (V)75L);
            }
            return;
        }
    }
}
