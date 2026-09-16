using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 피닉스모드 — 5.99 `전사(비전직).txt` 의 SKILL_피닉스모드 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("피닉스모드", "5.99표")]
    public class SkillD53CB2C9C2A4BAA8B4DC : SkillScript
    {
        public SkillD53CB2C9C2A4BAA8B4DC(Skill skill) : base(skill)
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

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_vita", v_myid)) < (V)((V)100L))))
            {
                p.Call("message", (V)3L, (V)"체력이 100이상이여야 사용할수 있습니다.");
                return;
            }
            p.Call("set_vital", ((V)(p.Call("get_vita", v_myid)) - (V)(((V)(((V)(p.Call("get_vita", v_myid)) / (V)((V)100L))) * (V)((V)7L)))));
            p.Call("skill_delay", (V)20L);
            if (V.T(((V)(p.Call("phoenix", v_myid, (V)60L)) == (V)((V)1L))))
            {
                p.Call("game_sound", (V)81L, (V)0L);
                p.Call("message", (V)3L, (V)"피닉스모드를 외웠습니다.");
                p.Call("effect", v_myid, (V)291L, (V)34L, (V)100L);
                p.Call("motion", (V)130L, (V)60L);
            }
        }
    }
}
