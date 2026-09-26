using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 찔러휘비기 — 5.99 `도적(비전직).txt` 의 SKILL_찔러휘비기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("찔러휘비기", "5.99표")]
    public class SkillCC14B7ECD718BE44AE30 : SkillScript
    {
        public SkillCC14B7ECD718BE44AE30(Skill skill) : base(skill)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)40L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 40이상]");
                return;
            }
            p.Call("manal_del", (V)"40");
            v_damage = (((V)(((V)(p.Call("get_att_damage", v_myid)) / (V)((V)10L))) * (V)((V)22L)));
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
                    p.Call("motion", (V)134L, (V)30L);
                    p.Call("effect", v_target, (V)0L, (V)119L, (V)50L);  // 노바 이펙트(5.99: 0, 26, 속도 100)
                    p.Call("game_sound", (V)7L, (V)0L);
                }
            }
            else
            {
                p.Call("effect", v_target, (V)0L, (V)119L, (V)50L);  // 노바 이펙트(5.99: 0, 26, 속도 100)
                p.Call("damaged", v_target, v_damage);
                p.Call("motion", (V)134L, (V)30L);
                p.Call("game_sound", (V)7L, (V)0L);
            }
        }
    }
}
