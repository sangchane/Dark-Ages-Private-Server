using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 마레네라 — 노바 `법사(비전직).txt` 의 SPELL_마레네라 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("마레네라", "5.99표")]
    public class SpellB9C8B808B124B77C : SpellScript
    {
        public SpellB9C8B808B124B77C(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)320L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 320]");
                return;
            }
            p.Call("manal_del", (V)"320");
            v_damage = ((V)(((p.Call("get_int", v_myid)))) + (V)((V)116L));
            if (V.T(p.Call("enare", v_myid, (V)1L)))
                v_damage = ((V)(v_damage) + (V)((V)34L));
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                p.Call("damaged", v_target, v_damage);
            }
            else
            {
                if (V.T(V.B(V.T(((V)(v_type) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    p.Call("char_damaged2", v_target, v_damage, v_damage);
                }
            }
            p.Call("message", (V)3L, (V)"마레네라를 외웠습니다.");
            p.Call("effect", v_target, (V)0L, (V)104L, (V)75L);
            p.Call("motion", (V)136L, (V)75L);
            p.Call("game_sound", (V)47L, (V)0L);
        }
    }
}
