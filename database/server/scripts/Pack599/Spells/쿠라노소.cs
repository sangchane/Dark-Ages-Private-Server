using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 쿠라노소 — 5.99 `성직자(비전직).txt` 의 SPELL_쿠라노소 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("쿠라노소", "5.99표")]
    public class SpellCFE0B77CB178C18C : SpellScript
    {
        public SpellCFE0B77CB178C18C(Spell spell) : base(spell)
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
            V v_hill = 0;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)70L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 70이상]");
                return;
            }
            p.Call("manal_del", (V)"70");
            v_hill = ((V)(p.Call("get_wis", v_myid)) * (V)((V)30L));
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            p.Call("motion", (V)128L, (V)55L);
            if (V.T(((V)(v_type) == (V)((V)3L))))
            {
                p.Call("set_vita", v_target, ((V)(p.Call("get_vita", v_target)) + (V)(v_hill)));
                p.Call("message", (V)3L, (V)"쿠라노소를 외웠습니다.");
                if (V.T(((V)(v_target) != (V)(v_myid))))
                {
                    p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 쿠라노소를 외워주셨습니다.")));
                    p.Call("effect", v_target, (V)21L, (V)21L, (V)120L);  // 노바 이펙트(5.99: 0, 267, 속도 120)
                }
                else
                {
                    p.Call("effect", v_target, (V)21L, (V)21L, (V)120L);  // 노바 이펙트(5.99: 267, 0, 속도 120)
                }
                p.Call("game_sound", (V)37L, (V)0L);
            }
        }
    }
}
