using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 두번찌르기 — 노바 `도적(비전직).txt` 의 SKILL_두번찌르기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("두번찌르기", "5.99표")]
    public class SkillB450BC88CC0CB974AE30 : SkillScript
    {
        public SkillB450BC88CC0CB974AE30(Skill skill) : base(skill)
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
            v_damage = ((V)(((p.Call("get_dex", v_myid)))) + (V)((V)157L));
            if (V.T(p.Call("enare", v_myid, (V)1L)))
                v_damage = ((V)(v_damage) + (V)((V)24L));
            p.Call("skill_delay", (V)4L);
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
                return;
            }
            if (V.T(V.B(!V.T(v_target))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("istype", p.Call("get_front_char", v_myid))) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_target = p.Call("get_front_char", v_myid);
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                    p.Call("motion", (V)135L, (V)20L);
                    p.Call("effect", v_target, (V)0L, (V)120L, (V)85L);
                    p.Call("game_sound", (V)69L, (V)0L);
                }
            }
            else
            {
                p.Call("damaged", v_target, v_damage);
                p.Call("motion", (V)135L, (V)20L);
                p.Call("effect", v_target, (V)0L, (V)120L, (V)85L);
                p.Call("game_sound", (V)69L, (V)0L);
            }
        }
    }
}
