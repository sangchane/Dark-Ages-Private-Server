using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 매직프로텍션 — 5.99 `Wizard.txt` 의 SPELL_매직프로텍션 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("매직프로텍션", "5.99표")]
    public class SpellB9E4C9C1D504B85CD14DC158 : SpellScript
    {
        public SpellB9E4C9C1D504B85CD14DC158(Spell spell) : base(spell)
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
            V v_rnd = 0;
            V v_target = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
            }
            else
            {
                v_target = p.Call("spell_target");
                v_rnd = p.Call("rand", (V)1L, (V)10L);
                if (V.T(((V)(v_rnd) <= (V)((V)2L))))
                {
                    p.Call("immortal", v_myid, (V)10L);
                    p.Call("effect", v_myid, (V)87L, (V)0L, (V)100L);
                    p.Call("game_sound", (V)8L, (V)0L);
                    p.Call("message", (V)3L, (V)"메직프로텍션을 외웠습니다.");
                }
                else
                {
                    p.Call("message", (V)3L, (V)"실패했습니다.");
                }
            }
        }
    }
}
