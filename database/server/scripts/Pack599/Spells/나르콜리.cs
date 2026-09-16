using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 나르콜리 — 5.99 `법사(비전직).txt` 의 SPELL_나르콜리 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("나르콜리", "5.99표")]
    public class SpellB098B974CF5CB9AC : SpellScript
    {
        public SpellB098B974CF5CB9AC(Spell spell) : base(spell)
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
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)50L))))
            {
                p.Call("message", (V)3L, (V)"마력이 부족합니다. [필요마나 : 50]");
                return;
            }
            p.Call("manal_del", (V)"50");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(V.B(V.T(V.B(V.T(v_target) && V.T(((V)(v_type) == (V)((V)1L))))) && V.T(p.Call("get_mobdie")))))
            {
                return;
            }
            if (V.T(((V)(p.Call("rand", (V)1L, (V)10L)) > (V)((V)8L))))
            {
                p.Call("effect", v_target, (V)0L, (V)33L, (V)130L);
                p.Call("message", (V)3L, (V)"대상이 마법을 피했다!");
                return;
            }
            if (V.T(((V)(v_type) == (V)((V)1L))))
            {
                if (V.T(p.Call("magic_exist", (V)2L, v_target, (V)1L)))
                {
                    p.Call("message", (V)3L, (V)"이미 걸려있습니다.");
                    return;
                }
                p.Call("magic", (V)2L, v_target, (V)0L, (V)20L, (V)0L, v_myid);
            }
            else
            {
                if (V.T(V.B(V.T(((V)(v_type) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    p.Call("mobnar_delay", v_target, (V)15L);
                    p.Call("message1", v_target, (V)3L, ((V)(p.Call("get_name")) + (V)((V)"님께서 나르콜리를 시전하셧습니다.")));
                }
                else
                {
                    return;
                }
            }
            p.Call("message", (V)3L, (V)"나르콜리를 외웠습니다.");
            p.Call("motion", (V)136L, (V)75L);
            p.Call("game_sound", (V)47L, (V)0L);
        }
    }
}
