using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 비스트어택(Lev1) — 5.99 `Monk.txt` 의 SKILL_비스트어택(Lev1) 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("비스트어택(Lev1)", "5.99표")]
    public class SkillBE44C2A4D2B8C5B4D0DD0028004C0065007600310029 : SkillScript
    {
        public SkillBE44C2A4D2B8C5B4D0DD0028004C0065007600310029(Skill skill) : base(skill)
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
            v_damage = ((V)(p.Call("get_basevita", v_myid)) / (V)((V)3L));
            if (V.T(p.Call("enare", v_myid, (V)1L)))
                v_damage = ((V)(v_damage) + (V)((V)30000L));
            p.Call("skill_delay", (V)1L);
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
                return;
            }
            if (V.T(p.Call("get_map_pk", v_myid)))
            {
                p.Call("message", (V)3L, (V)"PK맵에서는 사용할수 없는기술 입니다.");
                return;
            }
            if (V.T(V.B(!V.T(v_target))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("istype", p.Call("get_front_char", v_myid))) == (V)((V)3L))) && V.T(p.Call("get_map_pk")))))
                {
                    v_target = p.Call("get_front_char", v_myid);
                    p.Call("char_damaged", v_target, v_damage, v_damage);
                    p.Call("motion", (V)131L, (V)20L);
                    p.Call("effect", v_target, (V)0L, (V)188L, (V)75L);
                    p.Call("game_sound", (V)80L, (V)0L);
                }
            }
            else
            {
                p.Call("damaged", v_target, v_damage);
                p.Call("motion", (V)131L, (V)20L);
                p.Call("effect", v_target, (V)0L, (V)188L, (V)75L);
                p.Call("game_sound", (V)80L, (V)0L);
            }
        }
    }
}
