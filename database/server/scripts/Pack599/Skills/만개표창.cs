using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 만개표창 — 5.99 `도적(비전직).txt` 의 SKILL_만개표창 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("만개표창", "5.99표")]
    public class SkillB9CCAC1CD45CCC3D : SkillScript
    {
        public SkillB9CCAC1CD45CCC3D(Skill skill) : base(skill)
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
            V v_mob5 = 0;
            V v_mob6 = 0;
            V v_mob7 = 0;
            V v_mob8 = 0;
            V v_myid = 0;
            V v_type1 = 0;
            V v_type2 = 0;
            V v_type3 = 0;
            V v_type4 = 0;
            V v_type5 = 0;
            V v_type6 = 0;
            V v_type7 = 0;
            V v_type8 = 0;
            V v_x1 = 0;
            V v_y1 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)130L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 130]");
                return;
            }
            p.Call("manal_del", (V)130L);
            v_dam = ((V)((((V)(((V)(p.Call("get_att_damage", v_myid)) / (V)((V)10L))) * (V)((V)20L)))) + (V)((((V)(p.Call("get_dex", v_myid)) * (V)((V)10L)))));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_dam = ((V)(v_dam) * (V)((V)2L));
            }
            p.Call("skill_delay", (V)12L);
            v_x1 = p.Call("get_xs");
            v_y1 = p.Call("get_ys");
            v_mob1 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)1L)));
            v_mob2 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
            v_mob3 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
            v_mob4 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), (v_y1));
            v_mob5 = p.Call("get_mobxy", ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
            v_mob6 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), (v_y1));
            v_mob7 = p.Call("get_mobxy", (v_x1), ((V)((v_y1)) + (V)((V)1L)));
            v_mob8 = p.Call("get_mobxy", ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
            v_type1 = p.Call("istype", v_mob1);
            v_type2 = p.Call("istype", v_mob2);
            v_type3 = p.Call("istype", v_mob3);
            v_type4 = p.Call("istype", v_mob4);
            v_type5 = p.Call("istype", v_mob5);
            v_type6 = p.Call("istype", v_mob6);
            v_type7 = p.Call("istype", v_mob7);
            v_type8 = p.Call("istype", v_mob8);
            if (V.T(((V)(v_type1) == (V)((V)1L))))
            {
                p.Call("motion", (V)1L, (V)40L);
                p.Call("game_sound", (V)18L, (V)0L);
                p.Call("effect", v_mob1, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_mob1, v_dam);
            }
            if (V.T(((V)(v_type2) == (V)((V)1L))))
            {
                p.Call("motion", (V)1L, (V)40L);
                p.Call("game_sound", (V)18L, (V)0L);
                p.Call("effect", v_mob2, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_mob2, v_dam);
            }
            if (V.T(((V)(v_type3) == (V)((V)1L))))
            {
                p.Call("motion", (V)1L, (V)40L);
                p.Call("game_sound", (V)18L, (V)0L);
                p.Call("effect", v_mob3, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_mob3, v_dam);
            }
            if (V.T(((V)(v_type4) == (V)((V)1L))))
            {
                p.Call("motion", (V)1L, (V)40L);
                p.Call("game_sound", (V)18L, (V)0L);
                p.Call("effect", v_mob4, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_mob4, v_dam);
            }
            if (V.T(((V)(v_type5) == (V)((V)1L))))
            {
                p.Call("motion", (V)1L, (V)40L);
                p.Call("game_sound", (V)18L, (V)0L);
                p.Call("effect", v_mob5, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_mob5, v_dam);
            }
            if (V.T(((V)(v_type6) == (V)((V)1L))))
            {
                p.Call("motion", (V)1L, (V)40L);
                p.Call("game_sound", (V)18L, (V)0L);
                p.Call("effect", v_mob6, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_mob6, v_dam);
            }
            if (V.T(((V)(v_type7) == (V)((V)1L))))
            {
                p.Call("motion", (V)1L, (V)40L);
                p.Call("game_sound", (V)18L, (V)0L);
                p.Call("effect", v_mob7, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_mob7, v_dam);
            }
            if (V.T(((V)(v_type8) == (V)((V)1L))))
            {
                p.Call("motion", (V)1L, (V)40L);
                p.Call("game_sound", (V)18L, (V)0L);
                p.Call("effect", v_mob8, (V)0L, (V)26L, (V)100L);
                p.Call("damaged", v_mob8, v_dam);
            }
            if (V.T(p.Call("get_map_pk")))
            {
                v_mob1 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) - (V)((V)1L)));
                v_mob2 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                v_mob3 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) - (V)((V)1L)));
                v_mob4 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) - (V)((V)1L)), (v_y1));
                v_mob5 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) - (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                v_mob6 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), (v_y1));
                v_mob7 = p.Call("get_char_serial", v_myid, (v_x1), ((V)((v_y1)) + (V)((V)1L)));
                v_mob8 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), ((V)((v_y1)) + (V)((V)1L)));
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                v_type4 = p.Call("istype", v_mob4);
                v_type5 = p.Call("istype", v_mob5);
                v_type6 = p.Call("istype", v_mob6);
                v_type7 = p.Call("istype", v_mob7);
                v_type8 = p.Call("istype", v_mob8);
                if (V.T(((V)(v_type1) == (V)((V)3L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob1, (V)0L, (V)26L, (V)100L);
                    p.Call("char_damaged", v_mob1, v_dam, v_dam);
                }
                if (V.T(((V)(v_type2) == (V)((V)3L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob2, (V)0L, (V)26L, (V)100L);
                    p.Call("char_damaged", v_mob2, v_dam, v_dam);
                }
                if (V.T(((V)(v_type3) == (V)((V)3L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob3, (V)0L, (V)26L, (V)100L);
                    p.Call("char_damaged", v_mob3, v_dam, v_dam);
                }
                if (V.T(((V)(v_type4) == (V)((V)3L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob4, (V)0L, (V)26L, (V)100L);
                    p.Call("char_damaged", v_mob4, v_dam, v_dam);
                }
                if (V.T(((V)(v_type5) == (V)((V)3L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob5, (V)0L, (V)26L, (V)100L);
                    p.Call("char_damaged", v_mob5, v_dam, v_dam);
                }
                if (V.T(((V)(v_type6) == (V)((V)3L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob6, (V)0L, (V)26L, (V)100L);
                    p.Call("char_damaged", v_mob6, v_dam, v_dam);
                }
                if (V.T(((V)(v_type7) == (V)((V)3L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob7, (V)0L, (V)26L, (V)100L);
                    p.Call("char_damaged", v_mob7, v_dam, v_dam);
                }
                if (V.T(((V)(v_type8) == (V)((V)3L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("game_sound", (V)18L, (V)0L);
                    p.Call("effect", v_mob8, (V)0L, (V)26L, (V)100L);
                    p.Call("char_damaged", v_mob8, v_dam, v_dam);
                }
            }
        }
    }
}
