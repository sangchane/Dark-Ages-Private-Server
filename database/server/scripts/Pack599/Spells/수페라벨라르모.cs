using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 수페라벨라르모 — 노바 `성직자(비전직).txt` 의 SPELL_수페라벨라르모 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("수페라벨라르모", "5.99표")]
    public class SpellC218D398B77CBCA8B77CB974BAA8 : SpellScript
    {
        public SpellC218D398B77CBCA8B77CB974BAA8(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)120L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 120]");
                return;
            }
            p.Call("manal_del", (V)"120");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) != (V)((V)3L))))
            {
                return;
            }
            p.Call("belra", v_target, (V)120L, (V)14L);
            p.Call("game_sound", (V)8L, (V)0L);
            p.Call("message", (V)3L, (V)"수페라벨라르모를 외웠습니다.");
            p.Call("effect", v_target, (V)93L, (V)93L, (V)100L);
            if (V.T(((V)(v_target) == (V)(v_myid))))
            {
                return;
            }
            p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 수페라벨라르모를 외워주셨습니다.")));
        }
    }
}
