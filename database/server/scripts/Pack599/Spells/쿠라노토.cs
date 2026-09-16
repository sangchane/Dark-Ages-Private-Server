using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 쿠라노토 — 5.99 `공통스킬.txt` 의 SPELL_쿠라노토 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("쿠라노토", "5.99표")]
    public class SpellCFE0B77CB178D1A0 : SpellScript
    {
        public SpellCFE0B77CB178D1A0(Spell spell) : base(spell)
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
            V v_hill = 0;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)(((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)3L))))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 3%]");
                return;
            }
            p.Call("manal_del", ((V)(((V)(p.Call("get_basemana", v_myid)) / (V)((V)100L))) * (V)((V)3L)));
            v_hill = ((V)(p.Call("get_wis", v_myid)) * (V)((V)15L));
            if (V.T(((V)(v_hill) > (V)((V)6000L))))
            {
                v_hill = (V)6000L;
            }
            v_type = p.Call("istype", v_myid);
            p.Call("motion", (V)136L, (V)75L);
            p.Call("effect", v_target, (V)4L, (V)0L, (V)75L);
            p.Call("game_sound", (V)8L, (V)0L);
            p.Call("set_vita", v_myid, ((V)(p.Call("get_vita", v_myid)) + (V)(v_hill)));
        }
    }
}
