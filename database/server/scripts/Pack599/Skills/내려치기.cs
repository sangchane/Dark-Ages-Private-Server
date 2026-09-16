using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 내려치기 — 5.99 `전사(비전직).txt` 의 SKILL_내려치기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("내려치기", "5.99표")]
    public class SkillB0B4B824CE58AE30 : SkillScript
    {
        public SkillB0B4B824CE58AE30(Skill skill) : base(skill)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)75L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 75]");
                return;
            }
            p.Call("manal_del", (V)75L);
            v_damage = ((V)(p.Call("get_att_damage", v_myid)) * (V)((V)4L));
            v_damage = ((V)(v_damage) + (V)((p.Call("get_att_damage", v_myid))));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_damage = ((V)(v_damage) * (V)((V)2L));
            }
            p.Call("skill_delay", (V)5L);
            if (V.T(V.B(!V.T(v_target))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("istype", p.Call("get_front_char", v_myid))) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_target = p.Call("get_front_char", v_myid);
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                    p.Call("motion", (V)1L, (V)70L);
                    p.Call("effect", v_target, (V)0L, (V)69L, (V)100L);
                    p.Call("game_sound", (V)7L, (V)0L);
                }
            }
            else
            {
                p.Call("effect", v_target, (V)0L, (V)69L, (V)100L);
                p.Call("damaged", v_target, v_damage);
                p.Call("motion", (V)1L, (V)70L);
                p.Call("game_sound", (V)7L, (V)0L);
            }
        }
    }
}
