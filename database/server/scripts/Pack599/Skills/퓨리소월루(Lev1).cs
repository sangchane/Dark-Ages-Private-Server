using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 퓨리소월루(Lev1) — 5.99 `Warrior.txt` 의 SKILL_퓨리소월루(Lev1) 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("퓨리소월루(Lev1)", "5.99표")]
    public class SkillD4E8B9ACC18CC6D4B8E80028004C0065007600310029 : SkillScript
    {
        public SkillD4E8B9ACC18CC6D4B8E80028004C0065007600310029(Skill skill) : base(skill)
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
            V v_dam = 0;
            V v_mob1 = 0;
            V v_mob2 = 0;
            V v_mob3 = 0;
            V v_mob4 = 0;
            V v_myid = 0;
            V v_target = 0;
            V v_type = 0;
            V v_type1 = 0;
            V v_type2 = 0;
            V v_type3 = 0;
            V v_type4 = 0;
            V v_x1 = 0;
            V v_y1 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mapname")) == (V)((V)"OX퀴즈장"))))
            {
                p.Call("message", (V)3L, (V)"이벤트공간에서는 사용이불가능합니다.");
            }
            else
            {
                v_target = p.Call("skill_target");
                v_x1 = p.Call("get_xs");
                v_y1 = p.Call("get_ys");
                v_dam = ((V)(((p.Call("get_basevita", v_myid)))) * (V)((V)6L));
                v_type = p.Call("istype", v_target);
                v_mob1 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)1L)));
                v_mob2 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)1L)));
                v_mob3 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), v_y1);
                v_mob4 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), v_y1);
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                v_type4 = p.Call("istype", v_mob4);
                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                {
                    p.Call("motion", (V)129L, (V)20L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("skill_delay", (V)10L);
                    p.Call("effect", v_mob1, (V)0L, (V)296L, (V)75L);
                    p.Call("effect", v_mob2, (V)0L, (V)296L, (V)75L);
                    p.Call("effect", v_mob3, (V)0L, (V)296L, (V)75L);
                    p.Call("effect", v_mob4, (V)0L, (V)296L, (V)75L);
                    p.Call("damaged", v_mob1, v_dam);
                    p.Call("damaged", v_mob2, v_dam);
                    p.Call("damaged", v_mob3, v_dam);
                    p.Call("damaged", v_mob4, v_dam);
                }
                else
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                    {
                        p.Call("motion", (V)129L, (V)20L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)10L);
                        p.Call("effect", v_mob2, (V)0L, (V)296L, (V)75L);
                        p.Call("effect", v_mob3, (V)0L, (V)296L, (V)75L);
                        p.Call("effect", v_mob4, (V)0L, (V)296L, (V)75L);
                        p.Call("damaged", v_mob2, v_dam);
                        p.Call("damaged", v_mob3, v_dam);
                        p.Call("damaged", v_mob4, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                        {
                            p.Call("motion", (V)129L, (V)20L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)10L);
                            p.Call("effect", v_mob1, (V)0L, (V)296L, (V)75L);
                            p.Call("effect", v_mob3, (V)0L, (V)296L, (V)75L);
                            p.Call("effect", v_mob4, (V)0L, (V)296L, (V)75L);
                            p.Call("damaged", v_mob1, v_dam);
                            p.Call("damaged", v_mob3, v_dam);
                            p.Call("damaged", v_mob4, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                            {
                                p.Call("motion", (V)129L, (V)20L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)10L);
                                p.Call("effect", v_mob1, (V)0L, (V)296L, (V)75L);
                                p.Call("effect", v_mob2, (V)0L, (V)296L, (V)75L);
                                p.Call("effect", v_mob4, (V)0L, (V)296L, (V)75L);
                                p.Call("damaged", v_mob1, v_dam);
                                p.Call("damaged", v_mob2, v_dam);
                                p.Call("damaged", v_mob4, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                {
                                    p.Call("motion", (V)129L, (V)20L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)10L);
                                    p.Call("effect", v_mob1, (V)0L, (V)296L, (V)75L);
                                    p.Call("effect", v_mob2, (V)0L, (V)296L, (V)75L);
                                    p.Call("effect", v_mob3, (V)0L, (V)296L, (V)75L);
                                    p.Call("damaged", v_mob1, v_dam);
                                    p.Call("damaged", v_mob2, v_dam);
                                    p.Call("damaged", v_mob3, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                                    {
                                        p.Call("motion", (V)129L, (V)20L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)10L);
                                        p.Call("effect", v_mob3, (V)0L, (V)296L, (V)75L);
                                        p.Call("effect", v_mob4, (V)0L, (V)296L, (V)75L);
                                        p.Call("damaged", v_mob3, v_dam);
                                        p.Call("damaged", v_mob4, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                                        {
                                            p.Call("motion", (V)129L, (V)20L);
                                            p.Call("game_sound", (V)15L, (V)0L);
                                            p.Call("skill_delay", (V)10L);
                                            p.Call("effect", v_mob2, (V)0L, (V)296L, (V)75L);
                                            p.Call("effect", v_mob4, (V)0L, (V)296L, (V)75L);
                                            p.Call("damaged", v_mob2, v_dam);
                                            p.Call("damaged", v_mob4, v_dam);
                                        }
                                        else
                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                            {
                                                p.Call("motion", (V)129L, (V)20L);
                                                p.Call("game_sound", (V)18L, (V)0L);
                                                p.Call("skill_delay", (V)10L);
                                                p.Call("effect", v_mob2, (V)0L, (V)296L, (V)75L);
                                                p.Call("effect", v_mob3, (V)0L, (V)296L, (V)75L);
                                                p.Call("damaged", v_mob2, v_dam);
                                                p.Call("damaged", v_mob3, v_dam);
                                            }
                                            else
                                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                                                {
                                                    p.Call("motion", (V)129L, (V)20L);
                                                    p.Call("game_sound", (V)18L, (V)0L);
                                                    p.Call("skill_delay", (V)10L);
                                                    p.Call("effect", v_mob1, (V)0L, (V)296L, (V)75L);
                                                    p.Call("effect", v_mob4, (V)0L, (V)296L, (V)75L);
                                                    p.Call("damaged", v_mob1, v_dam);
                                                    p.Call("damaged", v_mob4, v_dam);
                                                }
                                                else
                                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                    {
                                                        p.Call("motion", (V)129L, (V)20L);
                                                        p.Call("game_sound", (V)18L, (V)0L);
                                                        p.Call("skill_delay", (V)10L);
                                                        p.Call("effect", v_mob1, (V)0L, (V)296L, (V)75L);
                                                        p.Call("effect", v_mob3, (V)0L, (V)296L, (V)75L);
                                                        p.Call("damaged", v_mob1, v_dam);
                                                        p.Call("damaged", v_mob3, v_dam);
                                                    }
                                                    else
                                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                        {
                                                            p.Call("motion", (V)129L, (V)20L);
                                                            p.Call("game_sound", (V)18L, (V)0L);
                                                            p.Call("skill_delay", (V)10L);
                                                            p.Call("effect", v_mob1, (V)0L, (V)296L, (V)75L);
                                                            p.Call("effect", v_mob2, (V)0L, (V)296L, (V)75L);
                                                            p.Call("damaged", v_mob1, v_dam);
                                                            p.Call("damaged", v_mob2, v_dam);
                                                        }
                                                        else
                                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                                                            {
                                                                p.Call("motion", (V)129L, (V)20L);
                                                                p.Call("game_sound", (V)18L, (V)0L);
                                                                p.Call("skill_delay", (V)10L);
                                                                p.Call("effect", v_mob4, (V)0L, (V)296L, (V)75L);
                                                                p.Call("damaged", v_mob4, v_dam);
                                                            }
                                                            else
                                                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                                {
                                                                    p.Call("motion", (V)129L, (V)20L);
                                                                    p.Call("game_sound", (V)18L, (V)0L);
                                                                    p.Call("skill_delay", (V)10L);
                                                                    p.Call("effect", v_mob3, (V)0L, (V)296L, (V)75L);
                                                                    p.Call("damaged", v_mob3, v_dam);
                                                                }
                                                                else
                                                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                                    {
                                                                        p.Call("motion", (V)129L, (V)20L);
                                                                        p.Call("game_sound", (V)18L, (V)0L);
                                                                        p.Call("skill_delay", (V)10L);
                                                                        p.Call("effect", v_mob2, (V)0L, (V)296L, (V)75L);
                                                                        p.Call("damaged", v_mob2, v_dam);
                                                                    }
                                                                    else
                                                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                                        {
                                                                            p.Call("motion", (V)129L, (V)20L);
                                                                            p.Call("game_sound", (V)18L, (V)0L);
                                                                            p.Call("skill_delay", (V)10L);
                                                                            p.Call("effect", v_mob1, (V)0L, (V)296L, (V)75L);
                                                                            p.Call("damaged", v_mob1, v_dam);
                                                                        }
            }
            if (V.T(((V)(v_type) != (V)((V)1L))))
            {
                p.Call("skill_delay", (V)10L);
                return;
            }
        }
    }
}
