using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 윈드블레이드 — 5.99 `전사(비전직).txt` 의 SKILL_윈드블레이드 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("윈드블레이드", "5.99표")]
    public class SkillC708B4DCBE14B808C774B4DC : SkillScript
    {
        public SkillC708B4DCBE14B808C774B4DC(Skill skill) : base(skill)
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
            V v_conp = 0;
            V v_dam = 0;
            V v_mob11 = 0;
            V v_mob12 = 0;
            V v_mob13 = 0;
            V v_mob21 = 0;
            V v_mob22 = 0;
            V v_mob23 = 0;
            V v_mob31 = 0;
            V v_mob32 = 0;
            V v_mob33 = 0;
            V v_mob41 = 0;
            V v_mob42 = 0;
            V v_mob43 = 0;
            V v_myid = 0;
            V v_side = 0;
            V v_type11 = 0;
            V v_type12 = 0;
            V v_type13 = 0;
            V v_type21 = 0;
            V v_type22 = 0;
            V v_type23 = 0;
            V v_type31 = 0;
            V v_type32 = 0;
            V v_type33 = 0;
            V v_type41 = 0;
            V v_type42 = 0;
            V v_type43 = 0;
            V v_x1 = 0;
            V v_y1 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)30L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 30이상]");
                return;
            }
            p.Call("manal_del", (V)"30");
            v_x1 = p.Call("get_xs");
            v_y1 = p.Call("get_ys");
            v_conp = ((V)(p.Call("get_con", v_myid)) * (V)((V)30L));
            v_dam = ((V)(((V)(p.Call("get_att_damage", v_myid)) / (V)((V)10L))) * (V)((V)50L));
            v_dam = ((V)(((V)(v_dam) + (V)((((V)(p.Call("get_att_damage", v_myid)) / (V)((V)2L)))))) + (V)(v_conp));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_dam = ((V)(v_dam) * (V)((V)2L));
            }
            v_side = p.Call("get_side", v_myid);
            if (V.T(((V)(v_side) == (V)((V)0L))))
            {
                v_mob11 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)1L)));
                v_mob12 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)2L)));
                v_mob13 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)3L)));
                v_type11 = p.Call("istype", v_mob11);
                v_type12 = p.Call("istype", v_mob12);
                v_type13 = p.Call("istype", v_mob13);
                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) == (V)((V)1L))) && V.T(((V)(v_type12) == (V)((V)1L)))))) && V.T(((V)(v_type13) == (V)((V)1L))))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("skill_delay", (V)4L);
                    p.Call("effect", v_mob11, (V)0L, (V)166L, (V)75L);
                    p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                    p.Call("effect", v_mob13, (V)0L, (V)166L, (V)75L);
                    p.Call("damaged", v_mob11, v_dam);
                    p.Call("damaged", v_mob12, v_dam);
                    p.Call("damaged", v_mob13, v_dam);
                }
                else
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) == (V)((V)1L))) && V.T(((V)(v_type12) != (V)((V)1L)))))) && V.T(((V)(v_type13) != (V)((V)1L))))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)4L);
                        p.Call("effect", v_mob11, (V)0L, (V)166L, (V)75L);
                        p.Call("damaged", v_mob11, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) != (V)((V)1L))) && V.T(((V)(v_type12) == (V)((V)1L)))))) && V.T(((V)(v_type13) == (V)((V)1L))))))
                        {
                            p.Call("motion", (V)129L, (V)40L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)4L);
                            p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                            p.Call("effect", v_mob13, (V)0L, (V)166L, (V)75L);
                            p.Call("damaged", v_mob12, v_dam);
                            p.Call("damaged", v_mob13, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) == (V)((V)1L))) && V.T(((V)(v_type12) != (V)((V)1L)))))) && V.T(((V)(v_type13) == (V)((V)1L))))))
                            {
                                p.Call("motion", (V)129L, (V)40L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)4L);
                                p.Call("effect", v_mob11, (V)0L, (V)166L, (V)75L);
                                p.Call("effect", v_mob13, (V)0L, (V)166L, (V)75L);
                                p.Call("damaged", v_mob11, v_dam);
                                p.Call("damaged", v_mob13, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) == (V)((V)1L))) && V.T(((V)(v_type12) == (V)((V)1L)))))) && V.T(((V)(v_type13) != (V)((V)1L))))))
                                {
                                    p.Call("motion", (V)129L, (V)40L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)4L);
                                    p.Call("effect", v_mob11, (V)0L, (V)166L, (V)75L);
                                    p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                                    p.Call("damaged", v_mob11, v_dam);
                                    p.Call("damaged", v_mob12, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) != (V)((V)1L))) && V.T(((V)(v_type12) != (V)((V)1L)))))) && V.T(((V)(v_type13) == (V)((V)1L))))))
                                    {
                                        p.Call("motion", (V)129L, (V)40L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)4L);
                                        p.Call("effect", v_mob13, (V)0L, (V)166L, (V)75L);
                                        p.Call("damaged", v_mob13, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) != (V)((V)1L))) && V.T(((V)(v_type12) == (V)((V)1L)))))) && V.T(((V)(v_type13) != (V)((V)1L))))))
                                        {
                                            p.Call("motion", (V)129L, (V)40L);
                                            p.Call("game_sound", (V)18L, (V)0L);
                                            p.Call("skill_delay", (V)4L);
                                            p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                                            p.Call("damaged", v_mob12, v_dam);
                                        }
                if (V.T(p.Call("get_map_pk")))
                {
                    v_mob11 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) - (V)((V)1L)));
                    v_mob12 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) - (V)((V)2L)));
                    v_mob13 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) - (V)((V)3L)));
                    v_type11 = p.Call("istype", v_mob11);
                    v_type12 = p.Call("istype", v_mob12);
                    v_type13 = p.Call("istype", v_mob13);
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) == (V)((V)3L))) && V.T(((V)(v_type12) == (V)((V)3L)))))) && V.T(((V)(v_type13) == (V)((V)3L))))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)4L);
                        p.Call("effect", v_mob11, (V)0L, (V)166L, (V)75L);
                        p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                        p.Call("effect", v_mob13, (V)0L, (V)166L, (V)75L);
                        p.Call("char_damaged", v_mob11, v_dam, v_dam);
                        p.Call("char_damaged", v_mob12, v_dam, v_dam);
                        p.Call("char_damaged", v_mob13, v_dam, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) == (V)((V)3L))) && V.T(((V)(v_type12) != (V)((V)3L)))))) && V.T(((V)(v_type13) != (V)((V)3L))))))
                        {
                            p.Call("motion", (V)129L, (V)40L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)4L);
                            p.Call("effect", v_mob11, (V)0L, (V)166L, (V)75L);
                            p.Call("char_damaged", v_mob11, v_dam, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) != (V)((V)3L))) && V.T(((V)(v_type12) == (V)((V)3L)))))) && V.T(((V)(v_type13) == (V)((V)3L))))))
                            {
                                p.Call("motion", (V)129L, (V)40L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)4L);
                                p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                                p.Call("effect", v_mob13, (V)0L, (V)166L, (V)75L);
                                p.Call("char_damaged", v_mob12, v_dam, v_dam);
                                p.Call("char_damaged", v_mob13, v_dam, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) == (V)((V)3L))) && V.T(((V)(v_type12) != (V)((V)3L)))))) && V.T(((V)(v_type13) == (V)((V)3L))))))
                                {
                                    p.Call("motion", (V)129L, (V)40L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)4L);
                                    p.Call("effect", v_mob11, (V)0L, (V)166L, (V)75L);
                                    p.Call("effect", v_mob13, (V)0L, (V)166L, (V)75L);
                                    p.Call("char_damaged", v_mob11, v_dam, v_dam);
                                    p.Call("char_damaged", v_mob13, v_dam, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) == (V)((V)3L))) && V.T(((V)(v_type12) == (V)((V)3L)))))) && V.T(((V)(v_type13) != (V)((V)3L))))))
                                    {
                                        p.Call("motion", (V)129L, (V)40L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)4L);
                                        p.Call("effect", v_mob11, (V)0L, (V)166L, (V)75L);
                                        p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                                        p.Call("char_damaged", v_mob11, v_dam, v_dam);
                                        p.Call("char_damaged", v_mob12, v_dam, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) != (V)((V)3L))) && V.T(((V)(v_type12) != (V)((V)3L)))))) && V.T(((V)(v_type13) == (V)((V)3L))))))
                                        {
                                            p.Call("motion", (V)129L, (V)40L);
                                            p.Call("game_sound", (V)18L, (V)0L);
                                            p.Call("skill_delay", (V)4L);
                                            p.Call("effect", v_mob13, (V)0L, (V)166L, (V)75L);
                                            p.Call("char_damaged", v_mob13, v_dam, v_dam);
                                        }
                                        else
                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type11) != (V)((V)3L))) && V.T(((V)(v_type12) == (V)((V)3L)))))) && V.T(((V)(v_type13) != (V)((V)3L))))))
                                            {
                                                p.Call("motion", (V)129L, (V)40L);
                                                p.Call("game_sound", (V)18L, (V)0L);
                                                p.Call("skill_delay", (V)4L);
                                                p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                                                p.Call("char_damaged", v_mob12, v_dam, v_dam);
                                            }
                }
            }
            if (V.T(((V)(v_side) == (V)((V)1L))))
            {
                v_mob41 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), v_y1);
                v_mob42 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)2L)), v_y1);
                v_mob43 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)3L)), v_y1);
                v_type41 = p.Call("istype", v_mob41);
                v_type42 = p.Call("istype", v_mob42);
                v_type43 = p.Call("istype", v_mob43);
                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) == (V)((V)1L))) && V.T(((V)(v_type42) == (V)((V)1L)))))) && V.T(((V)(v_type43) == (V)((V)1L))))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("skill_delay", (V)4L);
                    p.Call("effect", v_mob41, (V)0L, (V)166L, (V)75L);
                    p.Call("effect", v_mob42, (V)0L, (V)166L, (V)75L);
                    p.Call("effect", v_mob43, (V)0L, (V)166L, (V)75L);
                    p.Call("damaged", v_mob41, v_dam);
                    p.Call("damaged", v_mob42, v_dam);
                    p.Call("damaged", v_mob43, v_dam);
                }
                else
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) == (V)((V)1L))) && V.T(((V)(v_type42) != (V)((V)1L)))))) && V.T(((V)(v_type43) != (V)((V)1L))))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)4L);
                        p.Call("effect", v_mob41, (V)0L, (V)166L, (V)75L);
                        p.Call("damaged", v_mob41, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) != (V)((V)1L))) && V.T(((V)(v_type42) == (V)((V)1L)))))) && V.T(((V)(v_type43) == (V)((V)1L))))))
                        {
                            p.Call("motion", (V)129L, (V)40L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)4L);
                            p.Call("effect", v_mob42, (V)0L, (V)166L, (V)75L);
                            p.Call("effect", v_mob43, (V)0L, (V)166L, (V)75L);
                            p.Call("damaged", v_mob42, v_dam);
                            p.Call("damaged", v_mob43, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) == (V)((V)1L))) && V.T(((V)(v_type42) != (V)((V)1L)))))) && V.T(((V)(v_type43) == (V)((V)1L))))))
                            {
                                p.Call("motion", (V)129L, (V)40L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)4L);
                                p.Call("effect", v_mob41, (V)0L, (V)166L, (V)75L);
                                p.Call("effect", v_mob43, (V)0L, (V)166L, (V)75L);
                                p.Call("damaged", v_mob41, v_dam);
                                p.Call("damaged", v_mob43, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) == (V)((V)1L))) && V.T(((V)(v_type42) == (V)((V)1L)))))) && V.T(((V)(v_type43) != (V)((V)1L))))))
                                {
                                    p.Call("motion", (V)129L, (V)40L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)4L);
                                    p.Call("effect", v_mob41, (V)0L, (V)166L, (V)75L);
                                    p.Call("effect", v_mob42, (V)0L, (V)166L, (V)75L);
                                    p.Call("damaged", v_mob41, v_dam);
                                    p.Call("damaged", v_mob42, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) != (V)((V)1L))) && V.T(((V)(v_type42) != (V)((V)1L)))))) && V.T(((V)(v_type43) == (V)((V)1L))))))
                                    {
                                        p.Call("motion", (V)129L, (V)40L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)4L);
                                        p.Call("effect", v_mob43, (V)0L, (V)166L, (V)75L);
                                        p.Call("damaged", v_mob43, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) != (V)((V)1L))) && V.T(((V)(v_type42) == (V)((V)1L)))))) && V.T(((V)(v_type43) != (V)((V)1L))))))
                                        {
                                            p.Call("motion", (V)129L, (V)40L);
                                            p.Call("game_sound", (V)18L, (V)0L);
                                            p.Call("skill_delay", (V)4L);
                                            p.Call("effect", v_mob42, (V)0L, (V)166L, (V)75L);
                                            p.Call("damaged", v_mob42, v_dam);
                                        }
                if (V.T(p.Call("get_map_pk")))
                {
                    v_mob41 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), v_y1);
                    v_mob42 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)2L)), v_y1);
                    v_mob43 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)3L)), v_y1);
                    v_type41 = p.Call("istype", v_mob41);
                    v_type42 = p.Call("istype", v_mob42);
                    v_type43 = p.Call("istype", v_mob43);
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) == (V)((V)3L))) && V.T(((V)(v_type42) == (V)((V)3L)))))) && V.T(((V)(v_type43) == (V)((V)3L))))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)4L);
                        p.Call("effect", v_mob41, (V)0L, (V)166L, (V)75L);
                        p.Call("effect", v_mob42, (V)0L, (V)166L, (V)75L);
                        p.Call("effect", v_mob43, (V)0L, (V)166L, (V)75L);
                        p.Call("char_damaged", v_mob41, v_dam, v_dam);
                        p.Call("char_damaged", v_mob42, v_dam, v_dam);
                        p.Call("char_damaged", v_mob43, v_dam, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) == (V)((V)3L))) && V.T(((V)(v_type42) != (V)((V)3L)))))) && V.T(((V)(v_type43) != (V)((V)3L))))))
                        {
                            p.Call("motion", (V)129L, (V)40L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)4L);
                            p.Call("effect", v_mob41, (V)0L, (V)166L, (V)75L);
                            p.Call("char_damaged", v_mob41, v_dam, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) != (V)((V)3L))) && V.T(((V)(v_type42) == (V)((V)3L)))))) && V.T(((V)(v_type43) == (V)((V)3L))))))
                            {
                                p.Call("motion", (V)129L, (V)40L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)4L);
                                p.Call("effect", v_mob42, (V)0L, (V)166L, (V)75L);
                                p.Call("effect", v_mob43, (V)0L, (V)166L, (V)75L);
                                p.Call("char_damaged", v_mob42, v_dam, v_dam);
                                p.Call("char_damaged", v_mob43, v_dam, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) == (V)((V)3L))) && V.T(((V)(v_type42) != (V)((V)3L)))))) && V.T(((V)(v_type43) == (V)((V)3L))))))
                                {
                                    p.Call("motion", (V)129L, (V)40L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)4L);
                                    p.Call("effect", v_mob41, (V)0L, (V)166L, (V)75L);
                                    p.Call("effect", v_mob43, (V)0L, (V)166L, (V)75L);
                                    p.Call("char_damaged", v_mob41, v_dam, v_dam);
                                    p.Call("char_damaged", v_mob43, v_dam, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) == (V)((V)3L))) && V.T(((V)(v_type42) == (V)((V)3L)))))) && V.T(((V)(v_type43) != (V)((V)3L))))))
                                    {
                                        p.Call("motion", (V)129L, (V)40L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)4L);
                                        p.Call("effect", v_mob41, (V)0L, (V)166L, (V)75L);
                                        p.Call("effect", v_mob42, (V)0L, (V)166L, (V)75L);
                                        p.Call("char_damaged", v_mob41, v_dam, v_dam);
                                        p.Call("char_damaged", v_mob42, v_dam, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) != (V)((V)3L))) && V.T(((V)(v_type42) != (V)((V)3L)))))) && V.T(((V)(v_type43) == (V)((V)3L))))))
                                        {
                                            p.Call("motion", (V)129L, (V)40L);
                                            p.Call("game_sound", (V)18L, (V)0L);
                                            p.Call("skill_delay", (V)4L);
                                            p.Call("effect", v_mob43, (V)0L, (V)166L, (V)75L);
                                            p.Call("char_damaged", v_mob43, v_dam, v_dam);
                                        }
                                        else
                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type41) != (V)((V)3L))) && V.T(((V)(v_type42) == (V)((V)3L)))))) && V.T(((V)(v_type43) != (V)((V)3L))))))
                                            {
                                                p.Call("motion", (V)129L, (V)40L);
                                                p.Call("game_sound", (V)18L, (V)0L);
                                                p.Call("skill_delay", (V)4L);
                                                p.Call("effect", v_mob42, (V)0L, (V)166L, (V)75L);
                                                p.Call("char_damaged", v_mob42, v_dam, v_dam);
                                            }
                }
            }
            if (V.T(((V)(v_side) == (V)((V)2L))))
            {
                v_mob21 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)1L)));
                v_mob22 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)2L)));
                v_mob23 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)3L)));
                v_type21 = p.Call("istype", v_mob21);
                v_type22 = p.Call("istype", v_mob22);
                v_type23 = p.Call("istype", v_mob23);
                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) == (V)((V)1L))) && V.T(((V)(v_type22) == (V)((V)1L)))))) && V.T(((V)(v_type23) == (V)((V)1L))))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("skill_delay", (V)4L);
                    p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                    p.Call("effect", v_mob22, (V)0L, (V)166L, (V)75L);
                    p.Call("effect", v_mob23, (V)0L, (V)166L, (V)75L);
                    p.Call("damaged", v_mob21, v_dam);
                    p.Call("damaged", v_mob22, v_dam);
                    p.Call("damaged", v_mob23, v_dam);
                }
                else
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) == (V)((V)1L))) && V.T(((V)(v_type22) != (V)((V)1L)))))) && V.T(((V)(v_type23) != (V)((V)1L))))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)4L);
                        p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                        p.Call("damaged", v_mob21, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) != (V)((V)1L))) && V.T(((V)(v_type22) == (V)((V)1L)))))) && V.T(((V)(v_type23) == (V)((V)1L))))))
                        {
                            p.Call("motion", (V)129L, (V)40L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)4L);
                            p.Call("effect", v_mob22, (V)0L, (V)166L, (V)75L);
                            p.Call("effect", v_mob23, (V)0L, (V)166L, (V)75L);
                            p.Call("damaged", v_mob22, v_dam);
                            p.Call("damaged", v_mob23, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) == (V)((V)1L))) && V.T(((V)(v_type22) != (V)((V)1L)))))) && V.T(((V)(v_type23) == (V)((V)1L))))))
                            {
                                p.Call("motion", (V)129L, (V)40L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)4L);
                                p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                                p.Call("effect", v_mob23, (V)0L, (V)166L, (V)75L);
                                p.Call("damaged", v_mob21, v_dam);
                                p.Call("damaged", v_mob23, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) == (V)((V)1L))) && V.T(((V)(v_type22) == (V)((V)1L)))))) && V.T(((V)(v_type23) != (V)((V)1L))))))
                                {
                                    p.Call("motion", (V)129L, (V)40L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)4L);
                                    p.Call("effect", v_mob12, (V)0L, (V)166L, (V)75L);
                                    p.Call("effect", v_mob22, (V)0L, (V)166L, (V)75L);
                                    p.Call("damaged", v_mob21, v_dam);
                                    p.Call("damaged", v_mob22, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) != (V)((V)1L))) && V.T(((V)(v_type22) != (V)((V)1L)))))) && V.T(((V)(v_type23) == (V)((V)1L))))))
                                    {
                                        p.Call("motion", (V)129L, (V)40L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)4L);
                                        p.Call("effect", v_mob23, (V)0L, (V)166L, (V)75L);
                                        p.Call("damaged", v_mob23, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) != (V)((V)1L))) && V.T(((V)(v_type22) == (V)((V)1L)))))) && V.T(((V)(v_type23) != (V)((V)1L))))))
                                        {
                                            p.Call("motion", (V)129L, (V)40L);
                                            p.Call("game_sound", (V)18L, (V)0L);
                                            p.Call("skill_delay", (V)4L);
                                            p.Call("effect", v_mob22, (V)0L, (V)166L, (V)75L);
                                            p.Call("damaged", v_mob22, v_dam);
                                        }
                if (V.T(p.Call("get_map_pk")))
                {
                    v_mob21 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) + (V)((V)1L)));
                    v_mob22 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) + (V)((V)2L)));
                    v_mob23 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) + (V)((V)3L)));
                    v_type21 = p.Call("istype", v_mob21);
                    v_type22 = p.Call("istype", v_mob22);
                    v_type23 = p.Call("istype", v_mob23);
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) == (V)((V)3L))) && V.T(((V)(v_type22) == (V)((V)3L)))))) && V.T(((V)(v_type23) == (V)((V)3L))))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)4L);
                        p.Call("effect", v_mob21, (V)0L, (V)166L, (V)75L);
                        p.Call("effect", v_mob22, (V)0L, (V)166L, (V)75L);
                        p.Call("effect", v_mob23, (V)0L, (V)166L, (V)75L);
                        p.Call("char_damaged", v_mob21, v_dam, v_dam);
                        p.Call("char_damaged", v_mob22, v_dam, v_dam);
                        p.Call("char_damaged", v_mob23, v_dam, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) == (V)((V)3L))) && V.T(((V)(v_type22) != (V)((V)3L)))))) && V.T(((V)(v_type23) != (V)((V)3L))))))
                        {
                            p.Call("motion", (V)129L, (V)40L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)4L);
                            p.Call("effect", v_mob21, (V)0L, (V)166L, (V)75L);
                            p.Call("char_damaged", v_mob21, v_dam, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) != (V)((V)3L))) && V.T(((V)(v_type22) == (V)((V)3L)))))) && V.T(((V)(v_type23) == (V)((V)3L))))))
                            {
                                p.Call("motion", (V)129L, (V)40L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)4L);
                                p.Call("effect", v_mob22, (V)0L, (V)166L, (V)75L);
                                p.Call("effect", v_mob23, (V)0L, (V)166L, (V)75L);
                                p.Call("char_damaged", v_mob22, v_dam, v_dam);
                                p.Call("char_damaged", v_mob23, v_dam, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) == (V)((V)3L))) && V.T(((V)(v_type22) != (V)((V)3L)))))) && V.T(((V)(v_type23) == (V)((V)3L))))))
                                {
                                    p.Call("motion", (V)129L, (V)40L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)4L);
                                    p.Call("effect", v_mob21, (V)0L, (V)166L, (V)75L);
                                    p.Call("effect", v_mob23, (V)0L, (V)166L, (V)75L);
                                    p.Call("char_damaged", v_mob21, v_dam, v_dam);
                                    p.Call("char_damaged", v_mob23, v_dam, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) == (V)((V)3L))) && V.T(((V)(v_type22) == (V)((V)3L)))))) && V.T(((V)(v_type23) != (V)((V)3L))))))
                                    {
                                        p.Call("motion", (V)129L, (V)40L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)4L);
                                        p.Call("effect", v_mob21, (V)0L, (V)166L, (V)75L);
                                        p.Call("effect", v_mob22, (V)0L, (V)166L, (V)75L);
                                        p.Call("char_damaged", v_mob21, v_dam, v_dam);
                                        p.Call("char_damaged", v_mob22, v_dam, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) != (V)((V)3L))) && V.T(((V)(v_type22) != (V)((V)3L)))))) && V.T(((V)(v_type23) == (V)((V)3L))))))
                                        {
                                            p.Call("motion", (V)129L, (V)40L);
                                            p.Call("game_sound", (V)18L, (V)0L);
                                            p.Call("skill_delay", (V)4L);
                                            p.Call("effect", v_mob23, (V)0L, (V)166L, (V)75L);
                                            p.Call("char_damaged", v_mob23, v_dam, v_dam);
                                        }
                                        else
                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type21) != (V)((V)3L))) && V.T(((V)(v_type22) == (V)((V)3L)))))) && V.T(((V)(v_type23) != (V)((V)3L))))))
                                            {
                                                p.Call("motion", (V)129L, (V)40L);
                                                p.Call("game_sound", (V)18L, (V)0L);
                                                p.Call("skill_delay", (V)4L);
                                                p.Call("effect", v_mob22, (V)0L, (V)166L, (V)75L);
                                                p.Call("char_damaged", v_mob22, v_dam, v_dam);
                                            }
                }
            }
            if (V.T(((V)(v_side) == (V)((V)3L))))
            {
                v_mob31 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), v_y1);
                v_mob32 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)2L)), v_y1);
                v_mob33 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)3L)), v_y1);
                v_type31 = p.Call("istype", v_mob31);
                v_type32 = p.Call("istype", v_mob32);
                v_type33 = p.Call("istype", v_mob33);
                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) == (V)((V)1L))) && V.T(((V)(v_type32) == (V)((V)1L)))))) && V.T(((V)(v_type33) == (V)((V)1L))))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("skill_delay", (V)4L);
                    p.Call("effect", v_mob31, (V)0L, (V)166L, (V)75L);
                    p.Call("effect", v_mob32, (V)0L, (V)166L, (V)75L);
                    p.Call("effect", v_mob33, (V)0L, (V)166L, (V)75L);
                    p.Call("damaged", v_mob31, v_dam);
                    p.Call("damaged", v_mob32, v_dam);
                    p.Call("damaged", v_mob33, v_dam);
                }
                else
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) == (V)((V)1L))) && V.T(((V)(v_type32) != (V)((V)1L)))))) && V.T(((V)(v_type33) != (V)((V)1L))))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)4L);
                        p.Call("effect", v_mob31, (V)0L, (V)166L, (V)75L);
                        p.Call("damaged", v_mob31, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) != (V)((V)1L))) && V.T(((V)(v_type32) == (V)((V)1L)))))) && V.T(((V)(v_type33) == (V)((V)1L))))))
                        {
                            p.Call("motion", (V)129L, (V)40L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)4L);
                            p.Call("effect", v_mob32, (V)0L, (V)166L, (V)75L);
                            p.Call("effect", v_mob33, (V)0L, (V)166L, (V)75L);
                            p.Call("damaged", v_mob32, v_dam);
                            p.Call("damaged", v_mob33, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) == (V)((V)1L))) && V.T(((V)(v_type32) != (V)((V)1L)))))) && V.T(((V)(v_type33) == (V)((V)1L))))))
                            {
                                p.Call("motion", (V)129L, (V)40L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)4L);
                                p.Call("effect", v_mob31, (V)0L, (V)166L, (V)75L);
                                p.Call("effect", v_mob33, (V)0L, (V)166L, (V)75L);
                                p.Call("damaged", v_mob31, v_dam);
                                p.Call("damaged", v_mob33, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) == (V)((V)1L))) && V.T(((V)(v_type32) == (V)((V)1L)))))) && V.T(((V)(v_type33) != (V)((V)1L))))))
                                {
                                    p.Call("motion", (V)129L, (V)40L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)4L);
                                    p.Call("effect", v_mob31, (V)0L, (V)166L, (V)75L);
                                    p.Call("effect", v_mob32, (V)0L, (V)166L, (V)75L);
                                    p.Call("damaged", v_mob31, v_dam);
                                    p.Call("damaged", v_mob32, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) != (V)((V)1L))) && V.T(((V)(v_type32) != (V)((V)1L)))))) && V.T(((V)(v_type33) == (V)((V)1L))))))
                                    {
                                        p.Call("motion", (V)129L, (V)40L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)4L);
                                        p.Call("effect", v_mob33, (V)0L, (V)166L, (V)75L);
                                        p.Call("damaged", v_mob33, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) != (V)((V)1L))) && V.T(((V)(v_type32) == (V)((V)1L)))))) && V.T(((V)(v_type33) != (V)((V)1L))))))
                                        {
                                            p.Call("motion", (V)129L, (V)40L);
                                            p.Call("game_sound", (V)18L, (V)0L);
                                            p.Call("skill_delay", (V)4L);
                                            p.Call("effect", v_mob32, (V)0L, (V)166L, (V)75L);
                                            p.Call("damaged", v_mob32, v_dam);
                                        }
                if (V.T(p.Call("get_map_pk")))
                {
                    v_mob31 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) - (V)((V)1L)), v_y1);
                    v_mob32 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) - (V)((V)2L)), v_y1);
                    v_mob33 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) - (V)((V)3L)), v_y1);
                    v_type31 = p.Call("istype", v_mob31);
                    v_type32 = p.Call("istype", v_mob32);
                    v_type33 = p.Call("istype", v_mob33);
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) == (V)((V)3L))) && V.T(((V)(v_type32) == (V)((V)3L)))))) && V.T(((V)(v_type33) == (V)((V)3L))))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("skill_delay", (V)4L);
                        p.Call("effect", v_mob31, (V)0L, (V)166L, (V)75L);
                        p.Call("effect", v_mob32, (V)0L, (V)166L, (V)75L);
                        p.Call("effect", v_mob33, (V)0L, (V)166L, (V)75L);
                        p.Call("char_damaged", v_mob31, v_dam, v_dam);
                        p.Call("char_damaged", v_mob32, v_dam, v_dam);
                        p.Call("char_damaged", v_mob33, v_dam, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) == (V)((V)3L))) && V.T(((V)(v_type32) != (V)((V)3L)))))) && V.T(((V)(v_type33) != (V)((V)3L))))))
                        {
                            p.Call("motion", (V)129L, (V)40L);
                            p.Call("game_sound", (V)18L, (V)0L);
                            p.Call("skill_delay", (V)4L);
                            p.Call("effect", v_mob31, (V)0L, (V)166L, (V)75L);
                            p.Call("char_damaged", v_mob31, v_dam, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) != (V)((V)3L))) && V.T(((V)(v_type32) == (V)((V)3L)))))) && V.T(((V)(v_type33) == (V)((V)3L))))))
                            {
                                p.Call("motion", (V)129L, (V)40L);
                                p.Call("game_sound", (V)18L, (V)0L);
                                p.Call("skill_delay", (V)4L);
                                p.Call("effect", v_mob32, (V)0L, (V)166L, (V)75L);
                                p.Call("effect", v_mob33, (V)0L, (V)166L, (V)75L);
                                p.Call("char_damaged", v_mob32, v_dam, v_dam);
                                p.Call("char_damaged", v_mob33, v_dam, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) == (V)((V)3L))) && V.T(((V)(v_type32) != (V)((V)3L)))))) && V.T(((V)(v_type33) == (V)((V)3L))))))
                                {
                                    p.Call("motion", (V)129L, (V)40L);
                                    p.Call("game_sound", (V)18L, (V)0L);
                                    p.Call("skill_delay", (V)4L);
                                    p.Call("effect", v_mob31, (V)0L, (V)166L, (V)75L);
                                    p.Call("effect", v_mob33, (V)0L, (V)166L, (V)75L);
                                    p.Call("char_damaged", v_mob31, v_dam, v_dam);
                                    p.Call("char_damaged", v_mob33, v_dam, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) == (V)((V)3L))) && V.T(((V)(v_type32) == (V)((V)3L)))))) && V.T(((V)(v_type33) != (V)((V)3L))))))
                                    {
                                        p.Call("motion", (V)129L, (V)40L);
                                        p.Call("game_sound", (V)18L, (V)0L);
                                        p.Call("skill_delay", (V)4L);
                                        p.Call("effect", v_mob31, (V)0L, (V)166L, (V)75L);
                                        p.Call("effect", v_mob32, (V)0L, (V)166L, (V)75L);
                                        p.Call("char_damaged", v_mob31, v_dam, v_dam);
                                        p.Call("char_damaged", v_mob32, v_dam, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) != (V)((V)3L))) && V.T(((V)(v_type32) != (V)((V)3L)))))) && V.T(((V)(v_type33) == (V)((V)3L))))))
                                        {
                                            p.Call("motion", (V)129L, (V)40L);
                                            p.Call("game_sound", (V)18L, (V)0L);
                                            p.Call("skill_delay", (V)4L);
                                            p.Call("effect", v_mob33, (V)0L, (V)166L, (V)75L);
                                            p.Call("char_damaged", v_mob33, v_dam, v_dam);
                                        }
                                        else
                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type31) != (V)((V)3L))) && V.T(((V)(v_type32) == (V)((V)3L)))))) && V.T(((V)(v_type33) != (V)((V)3L))))))
                                            {
                                                p.Call("motion", (V)129L, (V)40L);
                                                p.Call("game_sound", (V)18L, (V)0L);
                                                p.Call("skill_delay", (V)4L);
                                                p.Call("effect", v_mob32, (V)0L, (V)166L, (V)75L);
                                                p.Call("char_damaged", v_mob32, v_dam, v_dam);
                                            }
                }
            }
        }
    }
}
