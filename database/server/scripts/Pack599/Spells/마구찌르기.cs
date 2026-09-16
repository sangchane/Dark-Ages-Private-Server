using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 마구찌르기 — 5.99 `도적(비전직).txt` 의 SPELL_마구찌르기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("마구찌르기", "5.99표")]
    public class SpellB9C8AD6CCC0CB974AE30 : SpellScript
    {
        public SpellB9C8AD6CCC0CB974AE30(Spell spell) : base(spell)
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
        }
    }
}
