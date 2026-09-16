using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 유령하녀 — 5.99 `Mob_Spell.txt` 의 Monster_유령하녀 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("Monster_유령하녀", "5.99표")]
    public class MonsterC720B839D558B140 : SpellScript
    {
        public MonsterC720B839D558B140(Spell spell) : base(spell)
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
            V v_myid = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("rand", (V)1L, (V)8L)) > (V)((V)6L))))
            {
                if (V.T(p.Call("set_strabismus", v_myid, (V)12L)))
                {
                    return;
                }
                else
                {
                    p.Call("game_sound", (V)8L, (V)0L);
                    p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 딜루메니를 가합니다.")));
                    p.Call("effect", v_myid, (V)276L, (V)0L, (V)80L);
                }
            }
            else
            {
                p.Call("reberato", v_myid);
                p.Call("game_sound", (V)10L, (V)0L);
                p.Call("effect", v_myid, (V)232L, (V)0L, (V)100L);
                p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 리베라토를 가합니다.")));
            }
        }
    }
}
