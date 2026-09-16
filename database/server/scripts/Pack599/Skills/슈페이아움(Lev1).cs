using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 슈페이아움(Lev1) — 5.99 `Jigja.txt` 의 SKILL_슈페이아움(Lev1) 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("슈페이아움(Lev1)", "5.99표")]
    public class SkillC288D398C774C544C6C00028004C0065007600310029 : SkillScript
    {
        public SkillC288D398C774C544C6C00028004C0065007600310029(Skill skill) : base(skill)
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)3000L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 3000이상]");
                return;
            }
            p.Call("manal_del", (V)"3000");
            v_target = p.Call("spell_target");
            v_type = p.Call("istype", v_target);
            if (V.T(((V)(p.Call("group_exist")) == (V)((V)0L))))
            {
                p.Call("message", (V)3L, (V)"그룹이 없습니다.");
                return;
            }
            p.Call("iaum_delay", (V)2L, (V)180L);
            p.Call("message", (V)3L, (V)"슈페이아움(Lev1)을 외었다!");
            p.Call("skill_delay", (V)10L);
        }
    }
}
