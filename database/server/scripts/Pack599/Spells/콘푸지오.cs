using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 콘푸지오 — 5.99 `법사(비전직).txt` 의 SPELL_콘푸지오 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("콘푸지오", "5.99표")]
    public class SpellCF58D478C9C0C624 : SpellScript
    {
        public SpellCF58D478C9C0C624(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)2L))))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 2%]");
                return;
            }
            p.Call("manal_del", ((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)2L)));
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(V.B(V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)1L))))) && V.T(p.Call("get_mobdie")))))
            {
                return;
            }
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                p.Call("effect", v_target, (V)0L, (V)118L, (V)80L);
                p.Call("motion", (V)136L, (V)75L);
                p.Call("message", (V)3L, (V)"콘푸지오를 외웠습니다.");
                p.Call("magic", (V)9L, v_target, (V)0L, (V)2L, (V)0L, v_myid);
                p.Call("game_sound", (V)47L, (V)0L);
                return;
            }
            p.Call("message", (V)3L, (V)"사용이 불가능한 대상입니다.");
        }
    }
}
