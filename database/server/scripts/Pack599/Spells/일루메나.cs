using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 일루메나 — 5.99 `성직자(비전직).txt` 의 SPELL_일루메나 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("일루메나", "5.99표")]
    public class SpellC77CB8E8BA54B098 : SpellScript
    {
        public SpellC77CB8E8BA54B098(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)70L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 70이상]");
                return;
            }
            p.Call("manal_del", (V)"70");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)3L))))))
            {
                p.Call("set_delstrabismus", v_target);
                p.Call("game_sound", (V)40L, (V)0L);
                if (V.T(((V)(v_target) != (V)(v_myid))))
                {
                    p.Call("effect", v_target, (V)0L, (V)281L, (V)75L);
                    p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 일루메나를 외워주셧습니다.")));
                }
                else
                {
                    p.Call("effect", v_target, (V)281L, (V)0L, (V)75L);
                }
                p.Call("message", (V)3L, (V)"일루메나를 외웠습니다.");
            }
            else
            {
                p.Call("message", (V)3L, (V)"사용할수 없는 대상 입니다.");
            }
        }
    }
}
