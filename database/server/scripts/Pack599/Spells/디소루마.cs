using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 디소루마 — 5.99 `성직자(비전직).txt` 의 SPELL_디소루마 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("디소루마", "5.99표")]
    public class SpellB514C18CB8E8B9C8 : SpellScript
    {
        public SpellB514C18CB8E8B9C8(Spell spell) : base(spell)
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
            v_target = p.Call("spell_target");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)30L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 30]");
                return;
            }
            p.Call("manal_del", (V)"30");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) == (V)((V)3L))))
            {
                p.Call("mobsor_end", v_target);
                p.Call("message", (V)3L, (V)"디소루마를 외웠습니다.");
                p.Call("game_sound", (V)39L, (V)0L);
                if (V.T(((V)(v_target) != (V)(v_myid))))
                {
                    p.Call("effect", v_target, (V)0L, (V)200L, (V)100L);  // 노바 이펙트(5.99: 0, 282)
                    p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 디소르마를 시전해주셧습니다.")));
                    return;
                }
                p.Call("effect", v_target, (V)0L, (V)200L, (V)100L);  // 노바 이펙트(5.99: 282, 0)
            }
        }
    }
}
