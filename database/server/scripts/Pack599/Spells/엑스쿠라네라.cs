using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 엑스쿠라네라 — 5.99 `성직자(비전직).txt` 의 SPELL_엑스쿠라네라 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("엑스쿠라네라", "5.99표")]
    public class SpellC5D1C2A4CFE0B77CB124B77C : SpellScript
    {
        public SpellC5D1C2A4CFE0B77CB124B77C(Spell spell) : base(spell)
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

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)430L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 430]");
                return;
            }
            p.Call("manal_del", (V)"430");
            v_hill = ((V)(p.Call("get_wis", v_myid)) * (V)((V)70L));
            p.Call("group_hill", v_hill, (V)267L);
            p.Call("message", (V)3L, (V)"엑스쿠라네라를 외웠습니다.");
            p.Call("group_message", (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 엑스쿠라네라를 외워주셧습니다.")));
            p.Call("motion", (V)128L, (V)55L);
            p.Call("game_sound", (V)38L, (V)0L);
        }
    }
}
