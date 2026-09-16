using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 이모탈 — 5.99 `성직자(비전직).txt` 의 SPELL_이모탈 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("이모탈", "5.99표")]
    public class SpellC774BAA8D0C8 : SpellScript
    {
        public SpellC774BAA8D0C8(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)50L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 50]");
                return;
            }
            p.Call("manal_del", (V)"50");
            v_target = p.Call("spell_target");
            L_RE: ;
            v_rnd = p.Call("rand", (V)1L, (V)10L);
            if (V.T(((V)(v_rnd) <= (V)((V)0L))))
            {
                goto L_RE;
            }
            if (V.T(((V)(v_rnd) <= (V)((V)8L))))
            {
                if (V.T(((V)(p.Call("immortal", v_myid, (V)10L)) == (V)((V)1L))))
                {
                    p.Call("effect", v_myid, (V)6L, (V)0L, (V)100L);
                    p.Call("game_sound", (V)8L, (V)0L);
                    p.Call("message", (V)3L, (V)"이모탈을 외웠습니다.");
                }
                return;
            }
            p.Call("message", (V)3L, (V)"실패했습니다.");
        }
    }
}
