using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 렌토 — 5.99 `법사(비전직).txt` 의 SPELL_렌토 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("렌토", "5.99표")]
    public class SpellB80CD1A0 : SpellScript
    {
        public SpellB80CD1A0(Spell spell) : base(spell)
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
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(V.B(V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)1L))))) && V.T(p.Call("get_mobdie")))))
            {
                return;
            }
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)30L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 30]");
                return;
            }
            p.Call("manal_del", (V)"30");
            if (V.T(((V)(v_type) != (V)((V)1L))))
            {
                return;
            }
            if (V.T(p.Call("magic_exist", (V)1L, v_target, (V)1L)))
            {
                return;
            }
            p.Call("magic", (V)1L, v_target, (V)"렌토", (V)120L, (V)15L, v_myid);
            p.Call("message", (V)3L, (V)"렌토를 외웠습니다.");
            p.Call("effect", v_target, (V)0L, (V)259L, (V)130L);
            p.Call("game_sound", (V)8L, (V)0L);
        }
    }
}
