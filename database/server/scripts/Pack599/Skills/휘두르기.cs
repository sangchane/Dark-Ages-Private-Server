using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 휘두르기 — 5.99 `전사(비전직).txt` 의 SKILL_휘두르기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("휘두르기", "5.99표")]
    public class SkillD718B450B974AE30 : SkillScript
    {
        public SkillD718B450B974AE30(Skill skill) : base(skill)
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
            V v_myid = 0;
            V v_side = 0;
            V v_type1 = 0;
            V v_type2 = 0;
            V v_type3 = 0;
            V v_x1 = 0;
            V v_y1 = 0;

            v_myid = p.Call("get_myid");
            v_x1 = p.Call("get_xs");
            v_y1 = p.Call("get_ys");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)40L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 40이상]");
                return;
            }
            p.Call("manal_del", (V)"40");
            v_dam = ((V)(p.Call("get_att_damage", v_myid)) * (V)((V)3L));
            v_dam = ((V)(((V)(v_dam) + (V)(((V)(p.Call("get_str", v_myid)) * (V)((V)20L))))) + (V)((((V)(p.Call("get_att_damage", v_myid)) / (V)((V)2L)))));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_dam = ((V)(v_dam) * (V)((V)2L));
            }
            p.Call("skill_delay", (V)6L);
            v_side = p.Call("get_side", v_myid);
            if (V.T(((V)(v_side) == (V)((V)0L))))
            {
                v_mob1 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)1L)));
                v_mob2 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                v_mob3 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob1, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob1, v_dam);
                }
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob2, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob2, v_dam);
                }
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob3, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob3, v_dam);
                }
                if (V.T(p.Call("get_map_pk")))
                {
                    v_mob1 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) - (V)((V)1L)));
                    v_mob2 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                    v_mob3 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                    v_type1 = p.Call("istype", v_mob1);
                    v_type2 = p.Call("istype", v_mob2);
                    v_type3 = p.Call("istype", v_mob3);
                    if (V.T(((V)(v_type1) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob1, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob1, v_dam, v_dam);
                    }
                    if (V.T(((V)(v_type2) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob2, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob2, v_dam, v_dam);
                    }
                    if (V.T(((V)(v_type3) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob3, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob3, v_dam, v_dam);
                    }
                }
            }
            if (V.T(((V)(v_side) == (V)((V)1L))))
            {
                v_mob1 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), (v_y1));
                v_mob2 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                v_mob3 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob1, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob1, v_dam);
                }
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob2, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob2, v_dam);
                }
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob3, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob3, v_dam);
                }
                if (V.T(p.Call("get_map_pk")))
                {
                    v_mob1 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), (v_y1));
                    v_mob2 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                    v_mob3 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                    v_type1 = p.Call("istype", v_mob1);
                    v_type2 = p.Call("istype", v_mob2);
                    v_type3 = p.Call("istype", v_mob3);
                    if (V.T(((V)(v_type1) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob1, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob1, v_dam, v_dam);
                    }
                    if (V.T(((V)(v_type2) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob2, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob2, v_dam, v_dam);
                    }
                    if (V.T(((V)(v_type3) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob3, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob3, v_dam, v_dam);
                    }
                }
            }
            if (V.T(((V)(v_side) == (V)((V)2L))))
            {
                v_mob1 = p.Call("get_mobxy", (v_x1), ((V)((v_y1)) + (V)((V)1L)));
                v_mob2 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                v_mob3 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob1, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob1, v_dam);
                }
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob2, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob2, v_dam);
                }
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob3, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob3, v_dam);
                }
                if (V.T(p.Call("get_map_pk")))
                {
                    v_mob1 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), (v_y1));
                    v_mob2 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                    v_mob3 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                    v_type1 = p.Call("istype", v_mob1);
                    v_type2 = p.Call("istype", v_mob2);
                    v_type3 = p.Call("istype", v_mob3);
                    if (V.T(((V)(v_type1) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob1, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob1, v_dam, v_dam);
                    }
                    if (V.T(((V)(v_type2) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob2, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob2, v_dam, v_dam);
                    }
                    if (V.T(((V)(v_type3) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob3, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob3, v_dam, v_dam);
                    }
                }
            }
            if (V.T(((V)(v_side) == (V)((V)3L))))
            {
                v_mob1 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), (v_y1));
                v_mob2 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                v_mob3 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob1, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob1, v_dam);
                }
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob2, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob2, v_dam);
                }
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    p.Call("motion", (V)129L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob3, (V)0L, (V)100L, (V)100L);
                    p.Call("damaged", v_mob3, v_dam);
                }
                if (V.T(p.Call("get_map_pk")))
                {
                    v_mob1 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), (v_y1));
                    v_mob2 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                    v_mob3 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                    v_type1 = p.Call("istype", v_mob1);
                    v_type2 = p.Call("istype", v_mob2);
                    v_type3 = p.Call("istype", v_mob3);
                    if (V.T(((V)(v_type1) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob1, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob1, v_dam, v_dam);
                    }
                    if (V.T(((V)(v_type2) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob2, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob2, v_dam, v_dam);
                    }
                    if (V.T(((V)(v_type3) == (V)((V)3L))))
                    {
                        p.Call("motion", (V)129L, (V)40L);
                        p.Call("game_sound", (V)18L, (V)0L);
                        p.Call("effect", v_mob3, (V)0L, (V)100L, (V)100L);
                        p.Call("char_damaged", v_mob3, v_dam, v_dam);
                    }
                }
            }
        }
    }
}
