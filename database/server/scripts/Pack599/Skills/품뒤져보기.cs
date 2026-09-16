using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 품뒤져보기 — 5.99 `공통스킬.txt` 의 SKILL_품뒤져보기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("품뒤져보기", "5.99표")]
    public class SkillD488B4A4C838BCF4AE30 : SkillScript
    {
        public SkillD488B4A4C838BCF4AE30(Skill skill) : base(skill)
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
            p.Call("sense_item");
        }
    }
}
