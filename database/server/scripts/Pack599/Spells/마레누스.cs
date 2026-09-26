using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 마레누스 — 5.99 `법사(비전직).txt` 의 SPELL_마레누스 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("마레누스", "5.99표")]
    public class SpellB9C8B808B204C2A4 : SpellScript
    {
        public SpellB9C8B808B204C2A4(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)44L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 44이상]");
                return;
            }
            p.Call("manal_del", (V)"44");
            v_damage = ((V)(((V)(p.Call("get_mgc_damage", v_myid)) / (V)((V)10L))) * (V)((V)22L));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_damage = ((V)(v_damage) * (V)((V)2L));
            }
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(V.B(V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)1L))))) && V.T(p.Call("get_mobdie")))))
            {
                return;
            }
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                p.Call("effect", v_target, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 235)
                p.Call("damaged", v_target, v_damage);
            }
            else
            {
                if (V.T(V.B(V.T(((V)(v_type) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    p.Call("char_damaged2", v_target, v_damage, v_damage);
                    if (V.T(((V)(v_target) != (V)(v_myid))))
                    {
                        p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 마레누스를 가합니다.")));
                    }
                }
            }
            p.Call("effect", v_target, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 235)
            p.Call("message", (V)3L, (V)"마레누스를 외웠습니다.");
            p.Call("motion", (V)136L, (V)75L);
            p.Call("game_sound", (V)47L, (V)0L);
        }
    }
}
