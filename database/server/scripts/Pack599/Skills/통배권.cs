using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 통배권 — 노바 `무도가(비전직).txt` 의 SKILL_통배권 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("통배권", "5.99표")]
    public class SkillD1B5BC30AD8C : SkillScript
    {
        public SkillD1B5BC30AD8C(Skill skill) : base(skill)
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
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
                return;
            }
            v_target = p.Call("skill_target");
            v_damage = ((V)(((p.Call("get_str", v_myid)))) + (V)((V)36L));
            if (V.T(p.Call("enare", v_myid, (V)1L)))
                v_damage = ((V)(v_damage) + (V)((V)9L));
            if (V.T(V.B(!V.T(v_target))))
                if (V.T(V.B(!V.T(v_target))))
                {
                    p.Call("skill_delay", (V)3L);
                    return;
                }
            p.Call("damaged", v_target, v_damage);
            p.Call("motion", (V)134L, (V)20L);
            p.Call("effect", v_target, (V)0L, (V)69L, (V)50L);
            p.Call("game_sound", (V)16L, (V)0L);
            p.Call("skill_delay", (V)2L);
            return;
        }
    }
}
