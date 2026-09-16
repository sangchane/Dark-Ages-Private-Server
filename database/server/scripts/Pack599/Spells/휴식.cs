using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 휴식 — 5.99 `공통스킬.txt` 의 SPELL_휴식 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("휴식", "5.99표")]
    public class SpellD734C2DD : SpellScript
    {
        public SpellD734C2DD(Spell spell) : base(spell)
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
            V h_husick = 0;
            V v_myid = 0;
            V v_rand = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(((V)(p.Call("time")) - (V)(h_husick))) < (V)((V)1L))))
            {
                return;
            }
            h_husick = p.Call("time");
            L_re: ;
            v_rand = p.Call("rand", (V)0L, (V)4L);
            if (V.T(V.B(V.T(((V)(v_rand) < (V)((V)1L))) || V.T(((V)(v_rand) > (V)((V)3L))))))
            {
                goto L_re;
            }
            p.Call("set_rest", v_myid, v_rand);
        }
    }
}
