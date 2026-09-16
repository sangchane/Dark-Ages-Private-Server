using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 신의축복 — 5.99 `Jigja.txt` 의 SPELL_신의축복 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("신의축복", "5.99표")]
    public class SpellC2E0C758CD95BCF5 : SpellScript
    {
        public SpellC2E0C758CD95BCF5(Spell spell) : base(spell)
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

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)5L))))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 5%]");
                return;
            }
            p.Call("set_manal", ((V)(p.Call("get_mana", v_myid)) - (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)5L)))));
            p.Call("god_bless", (V)86L, (V)120L);
            p.Call("game_sound", (V)21L, (V)0L);
            p.Call("message", (V)3L, (V)"신의축복을 외웠습니다.");
            p.Call("group_message", (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 신의축복을 외워주셧습니다.")));
        }
    }
}
