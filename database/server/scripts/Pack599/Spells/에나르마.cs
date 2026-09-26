using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 에나르마 — 5.99 `성직자(비전직).txt` 의 SPELL_에나르마 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("에나르마", "5.99표")]
    public class SpellC5D0B098B974B9C8 : SpellScript
    {
        public SpellC5D0B098B974B9C8(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)40L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 40]");
                return;
            }
            p.Call("manal_del", (V)"40");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) != (V)((V)3L))))
            {
                return;
            }
            if (V.T(((V)(p.Call("enare", v_target, (V)150L)) == (V)((V)1L))))
            {
                p.Call("game_sound", (V)81L, (V)0L);
                p.Call("message", (V)3L, (V)"에나르마를 외웠습니다.");
                if (V.T(((V)(v_target) == (V)(v_myid))))
                {
                    p.Call("effect", v_target, (V)195L, (V)195L, (V)100L);  // 노바 이펙트(5.99: 271, 0, 속도 100)
                    return;
                }
                p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 에나르마를 외워주셨습니다.")));
                p.Call("effect", v_target, (V)195L, (V)195L, (V)100L);  // 노바 이펙트(5.99: 271, 0, 속도 100)
            }
        }
    }
}
