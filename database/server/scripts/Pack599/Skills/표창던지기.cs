using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 표창던지기 — 5.99 `도적(비전직).txt` 의 SKILL_표창던지기 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("표창던지기", "5.99표")]
    public class SkillD45CCC3DB358C9C0AE30 : SkillScript
    {
        public SkillD45CCC3DB358C9C0AE30(Skill skill) : base(skill)
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
            V v_myid = 0;
            V v_side = 0;
            V v_type1 = 0;
            V v_type2 = 0;
            V v_type3 = 0;
            V v_type4 = 0;
            V v_type5 = 0;
            V v_x1 = 0;
            V v_y1 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(p.Call("get_map_pk")))
            {
                p.Call("message", (V)3L, (V)"PK장에서는 사용이 불가능합니다.");
                return;
            }
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)35L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 35이상]");
                return;
            }
            p.Call("manal_del", (V)"35");
            if (V.T(((V)(p.Call("item_exist", v_myid, (V)"투척용표창")) == (V)((V)0L))))
            {
                p.Call("message", (V)3L, (V)"던질수 있는 표창을 소지하고 있지 않습니다.");
                return;
            }
            p.Call("item_del", (V)"투척용표창", (V)1L);
            v_dam = (((V)(((V)(p.Call("get_att_damage", v_myid)) / (V)((V)10L))) * (V)((V)18L)));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_dam = ((V)(v_dam) * (V)((V)3L));
            }
            p.Call("skill_delay", (V)3L);
            v_x1 = p.Call("get_xs");
            v_y1 = p.Call("get_ys");
            v_side = p.Call("get_side", v_myid);
            if (V.T(((V)(v_side) == (V)((V)0L))))
            {
                v_mob1 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)1L)));
                v_mob2 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)2L)));
                v_mob3 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)3L)));
                v_mob4 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)4L)));
                v_mob5 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) - (V)((V)5L)));
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                v_type4 = p.Call("istype", v_mob4);
                v_type5 = p.Call("istype", v_mob5);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob1, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob1, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob2, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob2, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob3, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob3, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type4) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob4, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob4, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type5) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob5, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob5, v_dam);
                    goto L_go;
                }
            }
            if (V.T(((V)(v_side) == (V)((V)1L))))
            {
                v_mob1 = p.Call("get_mobxy", ((V)(v_x1) + (V)((V)1L)), v_y1);
                v_mob2 = p.Call("get_mobxy", ((V)(v_x1) + (V)((V)2L)), v_y1);
                v_mob3 = p.Call("get_mobxy", ((V)(v_x1) + (V)((V)3L)), v_y1);
                v_mob4 = p.Call("get_mobxy", ((V)(v_x1) + (V)((V)4L)), v_y1);
                v_mob5 = p.Call("get_mobxy", ((V)(v_x1) + (V)((V)5L)), v_y1);
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                v_type4 = p.Call("istype", v_mob4);
                v_type5 = p.Call("istype", v_mob5);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob1, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob1, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob2, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob2, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob3, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob3, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type4) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob4, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob4, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type5) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob5, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob5, v_dam);
                    goto L_go;
                }
            }
            if (V.T(((V)(v_side) == (V)((V)2L))))
            {
                v_mob1 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)1L)));
                v_mob2 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)2L)));
                v_mob3 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)3L)));
                v_mob4 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)4L)));
                v_mob5 = p.Call("get_mobxy", v_x1, ((V)((v_y1)) + (V)((V)5L)));
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                v_type4 = p.Call("istype", v_mob4);
                v_type5 = p.Call("istype", v_mob5);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob1, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob1, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob2, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob2, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob3, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob3, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type4) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob4, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob4, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type5) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob5, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob5, v_dam);
                    goto L_go;
                }
            }
            if (V.T(((V)(v_side) == (V)((V)3L))))
            {
                v_mob1 = p.Call("get_mobxy", ((V)(v_x1) - (V)((V)1L)), v_y1);
                v_mob2 = p.Call("get_mobxy", ((V)(v_x1) - (V)((V)2L)), v_y1);
                v_mob3 = p.Call("get_mobxy", ((V)(v_x1) - (V)((V)3L)), v_y1);
                v_mob4 = p.Call("get_mobxy", ((V)(v_x1) - (V)((V)4L)), v_y1);
                v_mob5 = p.Call("get_mobxy", ((V)(v_x1) - (V)((V)5L)), v_y1);
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                v_type4 = p.Call("istype", v_mob4);
                v_type5 = p.Call("istype", v_mob5);
                if (V.T(((V)(v_type1) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob1, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob1, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type2) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob2, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob2, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type3) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob3, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob3, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type4) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob4, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob4, v_dam);
                    goto L_go;
                }
                if (V.T(((V)(v_type5) == (V)((V)1L))))
                {
                    p.Call("motion", (V)1L, (V)40L);
                    p.Call("effect", v_mob5, (V)0L, (V)26L, (V)75L);
                    p.Call("game_sound", (V)1L, (V)0L);
                    p.Call("damaged", v_mob5, v_dam);
                    goto L_go;
                }
            }
            L_go: ;
        }
    }
}
