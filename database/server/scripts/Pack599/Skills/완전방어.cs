using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 완전방어 — 5.99 `전사(비전직).txt` 의 SKILL_완전방어 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("완전방어", "5.99표")]
    public class SkillC644C804BC29C5B4 : SkillScript
    {
        public SkillC644C804BC29C5B4(Skill skill) : base(skill)
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
            V v_rand = 0;

            v_myid = p.Call("get_myid");
            L_re: ;
            v_rand = p.Call("rand", (V)1L, (V)10L);
            if (V.T(V.B(V.T(((V)(v_rand) < (V)((V)1L))) || V.T(((V)(v_rand) > (V)((V)10L))))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_rand) > (V)((V)8L))))
            {
                p.Call("message", (V)3L, (V)"실패 했습니다.");
                p.Call("skill_delay", (V)16L);
                return;
            }
            p.Call("defens", v_myid, (V)10L);
            p.Call("effect", v_myid, (V)64L, (V)0L, (V)80L);  // 노바 이펙트(5.99: 301, 0)
            p.Call("motion", (V)130L, (V)60L);
            p.Call("game_sound", (V)8L, (V)0L);
            p.Call("skill_delay", (V)18L);
            p.Call("message", (V)3L, (V)"완전방어를 외웠습니다.");
        }
    }
}
