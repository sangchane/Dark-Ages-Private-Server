using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 결계도적 — 5.99 `Mob_Spell.txt` 의 Monster_결계도적 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("Monster_결계도적", "5.99표")]
    public class MonsterACB0ACC4B3C4C801 : SpellScript
    {
        public MonsterACB0ACC4B3C4C801(Spell spell) : base(spell)
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
            var p = Pack599.ForMonster(sprite, target);
            if (!p.Ready)
                return;
            V v_mob = 0;
            V v_myid = 0;

            if (V.T(V.B(V.T(((V)(p.Call("get_last_object_xs", v_myid)) == (V)((V)1000L))) || V.T(((V)(p.Call("get_last_object_ys", v_myid)) == (V)((V)1000L))))))
            {
                return;
            }
            v_mob = p.Call("get_mobxy", p.Call("get_last_object_xs", v_myid), p.Call("get_last_object_ys", v_myid));
            p.Call("mob_say2", v_mob, (V)0L, (V)0L, (V)"동면!");
            p.Call("game_sound", (V)10L, (V)0L);
            p.Call("mobsor_delay", v_myid, (V)8L);
        }
    }
}
