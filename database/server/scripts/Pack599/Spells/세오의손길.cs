using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 세오의손길 — 5.99 `Wizard.txt` 의 SPELL_세오의손길 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("세오의손길", "5.99표")]
    public class SpellC138C624C758C190AE38 : SpellScript
    {
        public SpellC138C624C758C190AE38(Spell spell) : base(spell)
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
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)3000L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 3000이상]");
                return;
            }
            p.Call("manal_del", (V)"3000");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            v_rnd = p.Call("rand", (V)1L, (V)10L);
            if (V.T(((V)(v_rnd) <= (V)((V)8L))))
            {
                if (V.T(((V)(v_type) != (V)((V)1L))))
                {
                    return;
                }
                if (V.T(p.Call("magic_exist", (V)5L, v_target, (V)1L)))
                {
                    return;
                }
                p.Call("magic", (V)5L, v_target, (V)"칸의눈(Lev1)", (V)30L, (V)5L, v_myid);
                p.Call("message", (V)3L, (V)"세오의손길을 외웠습니다.");
                p.Call("effect", v_target, (V)0L, (V)383L, (V)130L);
                p.Call("game_sound", (V)8L, (V)0L);
            }
            else
            {
                p.Call("effect", v_target, (V)0L, (V)33L, (V)130L);
                p.Call("message", (V)3L, (V)"몬스터가 마법을 피했다!");
            }
        }
    }
}
