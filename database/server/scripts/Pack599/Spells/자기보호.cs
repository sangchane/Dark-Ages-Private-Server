using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 자기보호 — 5.99 `Monk.txt` 의 SPELL_자기보호 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("자기보호", "5.99표")]
    public class SpellC790AE30BCF4D638 : SpellScript
    {
        public SpellC790AE30BCF4D638(Spell spell) : base(spell)
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

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)150L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 150]");
                return;
            }
            p.Call("manal_del", (V)"150");
            if (V.T(((V)(p.Call("horrama", v_myid, (V)60L)) == (V)((V)1L))))
            {
                p.Call("game_sound", (V)8L, (V)0L);
                p.Call("effect", v_myid, (V)262L, (V)0L, (V)100L);
                p.Call("message1", v_myid, (V)3L, (V)"자기보호를 외웠습니다.");
            }
        }
    }
}
