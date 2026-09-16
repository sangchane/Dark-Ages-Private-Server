using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 디소루미아 — 5.99 `Jigja.txt` 의 SPELL_디소루미아 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("디소루미아", "5.99표")]
    public class SpellB514C18CB8E8BBF8C544 : SpellScript
    {
        public SpellB514C18CB8E8BBF8C544(Spell spell) : base(spell)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)540L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 540]");
                return;
            }
            p.Call("manal_del", (V)"540");
            p.Call("group_mobsor_end", (V)282L);
            p.Call("game_sound", (V)39L, (V)0L);
            p.Call("message", (V)3L, (V)"디소루미아를 외웠습니다.");
            p.Call("group_message", (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 디소루미아를 외워주셧습니다.")));
        }
    }
}
