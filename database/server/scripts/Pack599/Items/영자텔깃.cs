using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 영자텔깃 — 5.99 `CashI.txt` 의 아이템 사용 스크립트를 그대로 옮긴 것. 아이템이 스스로 지운다(`item_del`).
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("ITEM_영자텔깃", "5.99표")]
    public class ItemC601C790D154AE43 : ItemScript
    {
        public ItemC601C790D154AE43(Item item) : base(item)
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
            V h_teleport = 0;
            V v_myid = 0;
            V v_side = 0;

            v_myid = p.Call("get_myid");
            if (V.T(V.B(V.T(p.Call("get_map_pk")) || V.T(p.Call("get_map_stage", v_myid)))))
            {
                p.Call("message", (V)3L, (V)"{=q안내 : 사용이 불가능 합니다.");
                return;
            }
            if (V.T(((V)(((V)(p.Call("time")) - (V)(h_teleport))) < (V)((V)1L))))
            {
                return;
            }
            v_side = p.Call("get_side", v_myid);
            h_teleport = p.Call("time");
            if (V.T(((V)(v_side) == (V)((V)0L))))
            {
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) - (V)((V)1L)))))
                {
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) - (V)((V)2L)))))
                {
                    p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) - (V)((V)1L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) - (V)((V)3L)))))
                {
                    p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) - (V)((V)2L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) - (V)((V)4L)))))
                {
                    p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) - (V)((V)3L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) - (V)((V)5L)))))
                {
                    p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) - (V)((V)4L)));
                    goto L_go;
                }
                p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) - (V)((V)5L)));
            }
            if (V.T(((V)(v_side) == (V)((V)1L))))
            {
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) + (V)((V)1L)), p.Call("get_ys", v_myid))))
                {
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) + (V)((V)2L)), p.Call("get_ys", v_myid))))
                {
                    p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) + (V)((V)1L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) + (V)((V)3L)), p.Call("get_ys", v_myid))))
                {
                    p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) + (V)((V)2L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) + (V)((V)4L)), p.Call("get_ys", v_myid))))
                {
                    p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) + (V)((V)3L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) + (V)((V)5L)), p.Call("get_ys", v_myid))))
                {
                    p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) + (V)((V)4L)));
                    goto L_go;
                }
                p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) + (V)((V)5L)));
            }
            if (V.T(((V)(v_side) == (V)((V)2L))))
            {
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) + (V)((V)1L)))))
                {
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) + (V)((V)2L)))))
                {
                    p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) + (V)((V)1L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) + (V)((V)3L)))))
                {
                    p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) + (V)((V)2L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) + (V)((V)4L)))))
                {
                    p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) + (V)((V)3L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, p.Call("get_xs", v_myid), ((V)(p.Call("get_ys", v_myid)) + (V)((V)5L)))))
                {
                    p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) + (V)((V)4L)));
                    goto L_go;
                }
                p.Call("set_ys", ((V)(p.Call("get_ys", v_myid)) + (V)((V)5L)));
            }
            if (V.T(((V)(v_side) == (V)((V)3L))))
            {
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) - (V)((V)1L)), p.Call("get_ys", v_myid))))
                {
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) - (V)((V)2L)), p.Call("get_ys", v_myid))))
                {
                    p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) - (V)((V)1L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) - (V)((V)3L)), p.Call("get_ys", v_myid))))
                {
                    p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) - (V)((V)2L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) - (V)((V)4L)), p.Call("get_ys", v_myid))))
                {
                    p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) - (V)((V)3L)));
                    goto L_go;
                }
                if (V.T(p.Call("get_xy_block", v_myid, ((V)(p.Call("get_xs", v_myid)) - (V)((V)5L)), p.Call("get_ys", v_myid))))
                {
                    p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) - (V)((V)4L)));
                    goto L_go;
                }
                p.Call("set_xs", ((V)(p.Call("get_xs", v_myid)) - (V)((V)5L)));
            }
            L_go: ;
            p.Call("effect", v_myid, (V)91L, (V)0L, (V)40L);
        }
    }
}
