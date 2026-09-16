using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 리젠(Lev1) — 5.99 `Jigja.txt` 의 SPELL_리젠(Lev1) 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("리젠(Lev1)", "5.99표")]
    public class SpellB9ACC8200028004C0065007600310029 : SpellScript
    {
        public SpellB9ACC8200028004C0065007600310029(Spell spell) : base(spell)
        {
        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {
        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {
        }

        public override void OnUse(Sprite sprite, Sprite target)
        {
            var p = new Pack599(sprite, target);
            if (!p.Ready)
                return;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
                return;
            }
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)5L))))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 5%]");
                return;
            }
            p.Call("manal_del", ((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)5L)));
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) != (V)((V)3L))))
            {
                return;
            }
            p.Call("hprecovery", v_target, (V)15L, ((V)(((V)(p.Call("get_basevita2", v_target)) / (V)((V)100L))) * (V)((V)10L)));
            p.Call("message", (V)3L, (V)"리젠(Lev1)을 외웠습니다.");
            if (V.T(((V)(v_target) != (V)(v_myid))))
            {
                p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 리젠(Lev1)를 외워주셨습니다.")));
            }
            p.Call("game_sound", (V)8L, (V)0L);
        }
    }
}
