using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 안티매직 — 5.99 `성직자(비전직).txt` 의 SPELL_안티매직 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("안티매직", "5.99표")]
    public class SpellC548D2F0B9E4C9C1 : SpellScript
    {
        public SpellC548D2F0B9E4C9C1(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)55L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 55]");
                return;
            }
            p.Call("manal_del", (V)"55");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) != (V)((V)3L))))
            {
                return;
            }
            if (V.T(((V)(p.Call("antimagic", v_target, (V)160L)) == (V)((V)1L))))
            {
                p.Call("game_sound", (V)81L, (V)0L);
                p.Call("message", (V)3L, (V)"안티매직을 외웠습니다.");
                if (V.T(((V)(v_target) == (V)(v_myid))))
                {
                    p.Call("effect", v_target, (V)0L, (V)168L, (V)100L);
                    return;
                }
                p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 안티매직을 외워주셨습니다.")));
                p.Call("effect", v_target, (V)168L, (V)0L, (V)100L);
                p.Call("game_sound", (V)39L, (V)0L);
            }
        }
    }
}
