using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 홀리드래곤 — 5.99 `Mob_Spell.txt` 의 Monster_홀리드래곤 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("Monster_홀리드래곤", "5.99표")]
    public class MonsterD640B9ACB4DCB798ACE4 : SpellScript
    {
        public MonsterD640B9ACB4DCB798ACE4(Spell spell) : base(spell)
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
            p.Call("group_hill", (V)0L, (V)89L);
            p.Call("effect", v_myid, (V)89L, (V)0L, (V)100L);
            p.Call("group_damaged2", v_myid, ((V)(p.Call("group_bighp")) / (V)((V)4L)));
            p.Call("game_sound", (V)63L, (V)0L);
            p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 홀리드래곤을 가합니다.")));
        }
    }
}
