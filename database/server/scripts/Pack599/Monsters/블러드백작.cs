using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 블러드백작 — 5.99 `Mob_Spell.txt` 의 Monster_블러드백작 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("Monster_블러드백작", "5.99표")]
    public class MonsterBE14B7ECB4DCBC31C791 : SpellScript
    {
        public MonsterBE14B7ECB4DCBC31C791(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("rand", (V)1L, (V)10L)) < (V)((V)3L))))
            {
                p.Call("reberato", v_myid);
                p.Call("game_sound", (V)10L, (V)0L);
                p.Call("effect", v_myid, (V)232L, (V)0L, (V)100L);
                p.Call("mob_say2", p.Call("get_object_id", v_myid), (V)0L, (V)0L, (V)"리베라토.");
                p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 리베라토를 가합니다.")));
            }
            else
                if (V.T(((V)(p.Call("rand", (V)1L, (V)10L)) < (V)((V)6L))))
                {
                    p.Call("group_hill", (V)0L, (V)72L);
                    p.Call("effect", v_myid, (V)72L, (V)0L, (V)100L);
                    p.Call("group_damaged2", v_myid, ((V)(p.Call("group_bighp")) / (V)((V)3L)));
                    p.Call("game_sound", (V)63L, (V)0L);
                    p.Call("mob_say2", p.Call("get_object_id", v_myid), (V)0L, (V)0L, (V)"메테오.");
                    p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 메테오를 가합니다.")));
                }
                else
                {
                    p.Call("game_sound", (V)10L, (V)0L);
                    p.Call("mob_say2", p.Call("get_object_id", v_myid), (V)0L, (V)0L, (V)"소루마.");
                    p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 소루마를 가합니다.")));
                    p.Call("mobsor_delay", v_myid, (V)8L);
                }
        }
    }
}
