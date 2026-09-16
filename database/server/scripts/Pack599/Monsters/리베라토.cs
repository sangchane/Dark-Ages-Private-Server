using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 리베라토 — 5.99 `Mob_Spell.txt` 의 Monster_리베라토 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("Monster_리베라토", "5.99표")]
    public class MonsterB9ACBCA0B77CD1A0 : SpellScript
    {
        public MonsterB9ACBCA0B77CD1A0(Spell spell) : base(spell)
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
            p.Call("reberato", v_myid);
            p.Call("game_sound", (V)10L, (V)0L);
            p.Call("effect", v_myid, (V)232L, (V)0L, (V)100L);
            p.Call("message", (V)3L, ((V)(p.Call("object_name")) + (V)((V)"가(이) 리베라토를 가합니다.")));
        }
    }
}
