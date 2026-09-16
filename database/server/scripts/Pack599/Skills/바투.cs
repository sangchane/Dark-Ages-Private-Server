using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 바투 — 5.99 `전사(비전직).txt` 의 SKILL_바투 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("바투", "5.99표")]
    public class SkillBC14D22C : SkillScript
    {
        public SkillBC14D22C(Skill skill) : base(skill)
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
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;

            v_myid = p.Call("get_myid");
            v_target = p.Call("skill_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(v_type) != (V)((V)1L))))
            {
                return;
            }
            if (V.T(p.Call("magic_exist", (V)6L, v_target, (V)1L)))
            {
                p.Call("message", (V)3L, (V)"이미 걸려있습니다.");
                return;
            }
            p.Call("magic", (V)6L, v_target, (V)0L, (V)15L, (V)0L, v_myid);
            p.Call("message", (V)3L, (V)"바투를 외웠습니다.");
            p.Call("motion", (V)129L, (V)20L);
            p.Call("game_sound", (V)8L, (V)0L);
            p.Call("skill_delay", (V)20L);
        }
    }
}
