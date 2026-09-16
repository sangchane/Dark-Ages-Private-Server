using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 깃털날리기(Lev1) — 5.99 `Monk.txt` 의 SPELL_깃털날리기(Lev1) 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("깃털날리기(Lev1)", "5.99표")]
    public class SpellAE43D138B0A0B9ACAE300028004C0065007600310029 : SpellScript
    {
        public SpellAE43D138B0A0B9ACAE300028004C0065007600310029(Spell spell) : base(spell)
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
            if (V.T(V.B(V.T(((V)(v_type) != (V)((V)1L))) && V.T(((V)(p.Call("get_vita", v_myid)) < (V)((V)3000L))))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다.");
                return;
            }
            if (V.T(p.Call("get_map_pk", v_myid)))
            {
                p.Call("message", (V)3L, (V)"PK맵에서는 사용할수 없는기술 입니다.");
                return;
            }
            v_damage = ((((V)(p.Call("get_basevita", v_myid)) / (V)((V)3L))));
            p.Call("manal_del", (V)"3000");
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
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                }
            }
            p.Call("message", (V)3L, (V)"깃털날리기(Lev1)를 외웠습니다.");
            p.Call("effect", v_target, (V)0L, (V)137L, (V)100L);
            p.Call("game_sound", (V)78L, (V)0L);
        }
    }
}
