using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 장풍 — 5.99 `무도가(비전직).txt` 의 SPELL_장풍 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("장풍", "5.99표")]
    public class SpellC7A5D48D : SpellScript
    {
        public SpellC7A5D48D(Spell spell) : base(spell)
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
            V v_damage = 0;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)150L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 150]");
                return;
            }
            p.Call("manal_del", (V)"150");
            v_damage = p.Call("get_att_damage", v_myid);
            v_damage = ((V)(v_damage) + (V)(((V)(((V)(p.Call("get_mgc_damage", v_myid)) / (V)((V)10L))) * (V)((V)25L))));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_damage = ((V)(v_damage) * (V)((V)2L));
            }
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                p.Call("effect", v_target, (V)0L, (V)158L, (V)75L);
                p.Call("damaged", v_target, v_damage);
            }
            else
            {
                if (V.T(V.B(V.T(((V)(v_type) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    p.Call("effect", v_target, (V)0L, (V)158L, (V)75L);
                    p.Call("char_damaged2", v_target, v_damage, v_damage);
                }
            }
            p.Call("message", (V)3L, (V)"장풍 외웠습니다.");
            p.Call("game_sound", (V)15L, (V)0L);
            p.Call("motion", (V)132L, (V)50L);
        }
    }
}
