using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 결계법사 — 5.99 `Mob_Spell.txt` 의 Monster_결계법사 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("Monster_결계법사", "5.99표")]
    public class MonsterACB0ACC4BC95C0AC : SpellScript
    {
        public MonsterACB0ACC4BC95C0AC(Spell spell) : base(spell)
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

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("rand", (V)1L, (V)10L)) >= (V)((V)6L))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("get_last_object_xs", v_myid)) == (V)((V)1000L))) || V.T(((V)(p.Call("get_last_object_ys", v_myid)) == (V)((V)1000L))))))
                {
                    return;
                }
                v_mob = p.Call("get_mobxy", p.Call("get_last_object_xs", v_myid), p.Call("get_last_object_ys", v_myid));
                p.Call("mob_say2", v_mob, (V)0L, (V)0L, (V)"나르콜리");
                p.Call("game_sound", (V)10L, (V)0L);
                p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 나르콜리를 가합니다.")));
                p.Call("mobnar_delay", v_myid, (V)8L);
            }
            else
            {
                if (V.T(V.B(V.T(((V)(p.Call("get_last_object_xs", v_myid)) == (V)((V)1000L))) || V.T(((V)(p.Call("get_last_object_ys", v_myid)) == (V)((V)1000L))))))
                {
                    return;
                }
                v_mob = p.Call("get_mobxy", p.Call("get_last_object_xs", v_myid), p.Call("get_last_object_ys", v_myid));
                p.Call("mob_say2", v_mob, (V)0L, (V)0L, (V)"마레누스");
                p.Call("game_sound", (V)47L, (V)0L);
                p.Call("effect", v_myid, (V)235L, (V)0L, (V)75L);
                p.Call("char_damaged2", v_myid, (V)4000L, (V)4000L);
            }
        }
    }
}
