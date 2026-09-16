using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 속성강화 — 5.99 `Wizard.txt` 의 SPELL_속성강화 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("속성강화", "5.99표")]
    public class SpellC18DC131AC15D654 : SpellScript
    {
        public SpellC18DC131AC15D654(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)320L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 320이상]");
                return;
            }
            p.Call("manal_del", (V)"320");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_myid) == (V)(v_target))))
            {
                p.Call("sokup_delay", v_target, (V)3000L);
                p.Call("message", (V)3L, (V)"속성강화를 외었다!");
                p.Call("effect", v_target, (V)0L, (V)263L, (V)100L);
                return;
            }
            if (V.T(((V)(v_type) == (V)((V)3L))))
            {
                p.Call("sokup_delay", v_target, (V)3000L);
                p.Call("message", (V)3L, (V)"속성강화를 외었다!");
                p.Call("effect", v_target, (V)0L, (V)263L, (V)100L);
                p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 속성강화를 외워주셨습니다.")));
                return;
            }
        }
    }
}
