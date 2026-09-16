using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 이상한빛 — 5.99 `Mob_Spell.txt` 의 Monster_이상한빛 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("Monster_이상한빛", "5.99표")]
    public class MonsterC774C0C1D55CBE5B : SpellScript
    {
        public MonsterC774C0C1D55CBE5B(Spell spell) : base(spell)
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
            V v_rand = 0;

            v_myid = p.Call("get_myid");
            v_rand = p.Call("rand", (V)1L, (V)3L);
            if (V.T(((V)(v_rand) == (V)((V)1L))))
            {
                p.Call("reberato", v_myid);
                p.Call("game_sound", (V)10L, (V)0L);
                p.Call("effect", v_myid, (V)232L, (V)0L, (V)100L);
                p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 리베라토를 가합니다.")));
            }
            else
                if (V.T(((V)(v_rand) == (V)((V)2L))))
                {
                    v_myid = p.Call("get_myid");
                    p.Call("game_sound", (V)10L, (V)0L);
                    p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 나르콜리를 가합니다.")));
                    p.Call("mobnar_delay", v_myid, (V)8L);
                }
                else
                    if (V.T(((V)(v_rand) == (V)((V)3L))))
                    {
                        v_myid = p.Call("get_myid");
                        p.Call("effect", v_myid, (V)23L, (V)0L, (V)75L);
                        p.Call("game_sound", (V)10L, (V)0L);
                        p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 아듀로를 가합니다.")));
                        p.Call("char_damaged2", v_myid, (V)485L, (V)485L);
                    }
        }
    }
}
