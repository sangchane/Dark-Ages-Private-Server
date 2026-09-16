using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 찌르기 — 5.99 `도적(비전직).txt` 의 SKILL_찌르기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("찌르기", "5.99표")]
    public class SkillCC0CB974AE30 : SkillScript
    {
        public SkillCC0CB974AE30(Skill skill) : base(skill)
        {
        }

        public override void OnFailed(Sprite sprite)
        {
        }

        public override void OnSuccess(Sprite sprite)
        {
        }

        public override void OnUse(Sprite sprite)
        {
            if (!Skill.Ready)
                return;

            var p = new Pack599(sprite, null);
            if (!p.Ready)
                return;

            p.Train(Skill);
            V v_damage = 0;
            V v_myid = 0;
            V v_target = 0;

            v_myid = p.Call("get_myid");
            v_target = p.Call("skill_target");
            if (V.T(((V)(p.Call("spell_exist", (V)"마구찌르기")) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)25L))))
                {
                    p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 25이상]");
                    return;
                }
                p.Call("manal_del", (V)"20");
                v_damage = (((V)(((V)(p.Call("get_att_damage", v_myid)) / (V)((V)10L))) * (V)((V)19L)));
            }
            else
            {
                if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)15L))))
                {
                    p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 15이상]");
                    return;
                }
                p.Call("manal_del", (V)"15");
                v_damage = (((V)(((V)(p.Call("get_att_damage", v_myid)) / (V)((V)10L))) * (V)((V)17L)));
            }
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_damage = ((V)(v_damage) * (V)((V)2L));
            }
            p.Call("skill_delay", (V)2L);
            if (V.T(V.B(!V.T(v_target))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("istype", p.Call("get_front_char", v_myid))) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_target = p.Call("get_front_char", v_myid);
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                    p.Call("motion", (V)134L, (V)30L);
                    p.Call("effect", v_target, (V)0L, (V)26L, (V)100L);
                    p.Call("game_sound", (V)7L, (V)0L);
                }
            }
            else
            {
                p.Call("effect", v_target, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_target, v_damage);
                p.Call("motion", (V)134L, (V)30L);
                p.Call("game_sound", (V)7L, (V)0L);
            }
        }
    }
}
