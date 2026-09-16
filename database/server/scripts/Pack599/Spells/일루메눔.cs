using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 일루메눔 — 5.99 `Jigja.txt` 의 SPELL_일루메눔 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("일루메눔", "5.99표")]
    public class SpellC77CB8E8BA54B214 : SpellScript
    {
        public SpellC77CB8E8BA54B214(Spell spell) : base(spell)
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
            p.Call("message", (V)3L, (V)"일루매눔를 외웠습니다.");
            p.Call("group_message", (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 일루매눔를 외워주셧습니다.")));
        }
    }
}
