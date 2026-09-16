using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 소매치기 — 5.99 `도적(비전직).txt` 의 SKILL_소매치기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("소매치기", "5.99표")]
    public class SkillC18CB9E4CE58AE30 : SkillScript
    {
        public SkillC18CB9E4CE58AE30(Skill skill) : base(skill)
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
            V v_gold = 0;
            V v_myid = 0;
            V v_rand = 0;
            V v_target = 0;

            v_myid = p.Call("get_myid");
            v_target = p.Call("skill_target");
            if (V.T(V.B(V.T(p.Call("get_map_pk")) || V.T(((V)(p.Call("get_mapname", v_myid)) == (V)((V)"연습장"))))))
            {
                p.Call("message", (V)3L, (V)"현재 위치에서는 사용이 불가합니다.");
                return;
            }
            L_re: ;
            v_rand = p.Call("rand", (V)1L, (V)10L);
            if (V.T(V.B(V.T(((V)(v_rand) < (V)((V)1L))) || V.T(((V)(v_rand) > (V)((V)10L))))))
            {
                goto L_re;
            }
            p.Call("skill_delay", (V)30L);
            if (V.T(V.B(!V.T(v_target))))
            {
                p.Call("message", (V)3L, (V)"소매치기할 몬스터가 존재하지 않습니다.");
                return;
            }
            if (V.T(((V)(v_rand) < (V)((V)8L))))
            {
                v_gold = ((V)(p.Call("get_level", v_myid)) * (V)((V)5L));
                v_gold = ((V)(v_gold) - (V)(p.Call("rand", (V)30L, (V)100L)));
                if (V.T(((V)(v_gold) < (V)((V)0L))))
                {
                    p.Call("message", (V)3L, (V)"몬스터가 눈치를 채서 소매치기에 실패하였다.");
                    return;
                }
                p.Call("message", (V)3L, ((V)(((V)((V)"소매치기를 성공하여 [") + (V)(v_gold))) + (V)((V)"]G 를 얻었다!")));
                p.Call("money_add", v_gold);
            }
            else
            {
                p.Call("message", (V)3L, (V)"몬스터가 눈치를 채서 소매치기에 실패하였다.");
                return;
            }
        }
    }
}
