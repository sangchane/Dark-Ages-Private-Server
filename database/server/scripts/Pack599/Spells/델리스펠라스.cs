using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 델리스펠라스 — 5.99 `Wizard.txt` 의 SPELL_델리스펠라스 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("델리스펠라스", "5.99표")]
    public class SpellB378B9ACC2A4D3A0B77CC2A4 : SpellScript
    {
        public SpellB378B9ACC2A4D3A0B77CC2A4(Spell spell) : base(spell)
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

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)350L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 350]");
                return;
            }
            p.Call("manal_del", (V)"350");
            L_re: ;
            v_rnd = p.Call("rand", (V)1L, (V)10L);
            if (V.T(V.B(V.T(((V)(v_rnd) < (V)((V)1L))) || V.T(((V)(v_rnd) > (V)((V)10L))))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_rnd) <= (V)((V)5L))))
            {
                p.Call("motion", (V)1L, (V)75L);
                p.Call("dell", v_myid, (V)17L);
                p.Call("effect", v_myid, (V)157L, (V)0L, (V)100L);
                p.Call("game_sound", (V)8L, (V)0L);
                p.Call("message", (V)3L, (V)"델리스펠라스를 외웠습니다.");
                return;
            }
            p.Call("message", (V)3L, (V)"실패했습니다.");
        }
    }
}
