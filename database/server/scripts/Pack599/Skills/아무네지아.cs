using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 아무네지아 — 5.99 `도적(비전직).txt` 의 SKILL_아무네지아 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("아무네지아", "5.99표")]
    public class SkillC544BB34B124C9C0C544 : SkillScript
    {
        public SkillC544BB34B124C9C0C544(Skill skill) : base(skill)
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
            V v_mob1 = 0;
            V v_mob2 = 0;
            V v_mob3 = 0;
            V v_mob4 = 0;
            V v_myid = 0;
            V v_type1 = 0;
            V v_type2 = 0;
            V v_type3 = 0;
            V v_type4 = 0;
            V v_x1 = 0;
            V v_y1 = 0;

            v_myid = p.Call("get_myid");
            p.Call("skill_delay", (V)13L);
            v_x1 = p.Call("get_xs");
            v_y1 = p.Call("get_ys");
            v_mob1 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)1L)));
            v_mob2 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)1L)));
            v_mob3 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), v_y1);
            v_mob4 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), v_y1);
            if (V.T(v_mob1))
            {
                v_type1 = p.Call("istype", v_mob1);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    if (V.T(p.Call("magic_exist", (V)8L, v_mob1, (V)1L)))
                    {
                        goto L_go1;
                    }
                    p.Call("magic", (V)8L, v_mob1, (V)"어둠의각인", (V)4L, (V)58L, v_myid);
                    p.Call("effect", v_mob1, (V)0L, (V)57L, (V)100L);
                }
            }
            L_go1: ;
            if (V.T(v_mob2))
            {
                v_type2 = p.Call("istype", v_mob2);
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    if (V.T(p.Call("magic_exist", (V)8L, v_mob2, (V)1L)))
                    {
                        goto L_go2;
                    }
                    p.Call("magic", (V)8L, v_mob2, (V)"어둠의각인", (V)4L, (V)58L, v_myid);
                    p.Call("effect", v_mob2, (V)0L, (V)57L, (V)100L);
                }
            }
            L_go2: ;
            if (V.T(v_mob3))
            {
                v_type3 = p.Call("istype", v_mob3);
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    if (V.T(p.Call("magic_exist", (V)8L, v_mob3, (V)1L)))
                    {
                        goto L_go3;
                    }
                    p.Call("magic", (V)8L, v_mob3, (V)"어둠의각인", (V)4L, (V)58L, v_myid);
                    p.Call("effect", v_mob3, (V)0L, (V)57L, (V)100L);
                }
            }
            L_go3: ;
            if (V.T(v_mob4))
            {
                v_type4 = p.Call("istype", v_mob4);
                if (V.T(((V)(v_type4) == (V)((V)1L))))
                {
                    if (V.T(p.Call("magic_exist", (V)8L, v_mob4, (V)1L)))
                    {
                        goto L_go4;
                    }
                    p.Call("magic", (V)8L, v_mob4, (V)"어둠의각인", (V)4L, (V)58L, v_myid);
                    p.Call("effect", v_mob4, (V)0L, (V)57L, (V)100L);
                }
            }
            p.Call("game_sound", (V)18L, (V)0L);
            L_go4: ;
        }
    }
}
