using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 콜라마 — 5.99 `성직자(비전직).txt` 의 SPELL_콜라마 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("콜라마", "5.99표")]
    public class SpellCF5CB77CB9C8 : SpellScript
    {
        public SpellCF5CB77CB9C8(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
                return;
            }
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) != (V)((V)3L))))
            {
                return;
            }
            v_hill = (((V)(p.Call("get_int", v_myid)) + (V)((V)400L)));
            if (V.T(((V)(v_hill) > (V)((V)1500L))))
            {
                v_hill = (V)1500L;
            }
            p.Call("hprecovery", v_target, (V)10L, v_hill);
            p.Call("message", (V)3L, (V)"콜라마를 외웠습니다.");
            p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 콜라마를 외워주셨습니다.")));
            p.Call("game_sound", (V)8L, (V)0L);
        }
    }
}
