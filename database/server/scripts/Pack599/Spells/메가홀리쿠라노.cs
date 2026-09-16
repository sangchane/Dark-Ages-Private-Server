using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 메가홀리쿠라노 — 5.99 `Jigja.txt` 의 SPELL_메가홀리쿠라노 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("메가홀리쿠라노", "5.99표")]
    public class SpellBA54AC00D640B9ACCFE0B77CB178 : SpellScript
    {
        public SpellBA54AC00D640B9ACCFE0B77CB178(Spell spell) : base(spell)
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
            V v_user = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
            }
            else
            {
                v_target = p.Call("spell_target");
                v_type = p.Call("istype", v_target);
                p.Call("motion", (V)128L, (V)55L);
                if (V.T(((V)(v_type) == (V)((V)3L))))
                {
                    p.Call("set_vita", v_target, ((V)(p.Call("get_vita", v_target)) + (V)((V)250000L)));
                    v_user = ((V)(((((V)(p.Call("get_body")) + (V)((V)1L))))) + (V)((V)1L));
                    p.Call("char_damaged", v_target, v_user, v_user);
                    p.Call("effect", v_target, (V)71L, (V)71L, (V)150L);
                    p.Call("game_sound", (V)36L, (V)0L);
                    p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 메가홀리쿠라노를 외워주셨습니다.")));
                    p.Call("message", (V)3L, (V)"메가홀리쿠라노를 외웠습니다.");
                }
                else
                    if (V.T(V.B(V.T(((V)(v_type) == (V)((V)1L))) || V.T(((V)(v_type) == (V)((V)2L))))))
                    {
                        p.Call("effect", v_target, (V)71L, (V)71L, (V)150L);
                        p.Call("game_sound", (V)36L, (V)0L);
                    }
            }
        }
    }
}
