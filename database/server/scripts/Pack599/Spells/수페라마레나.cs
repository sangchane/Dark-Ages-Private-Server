using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 수페라마레나 — 5.99 `법사(비전직).txt` 의 SPELL_수페라마레나 을 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-abilities.py` 가 다시 만든다.
    /// </remarks>
    [Script("수페라마레나", "5.99표")]
    public class SpellC218D398B77CB9C8B808B098 : SpellScript
    {
        public SpellC218D398B77CB9C8B808B098(Spell spell) : base(spell)
        {
        }

        public override void OnFailed(Sprite sprite, Sprite target)
        {
        }

        public override void OnSuccess(Sprite sprite, Sprite target)
        {
        }

        public override void OnUse(Sprite sprite, Sprite target)
        {
            var p = new Pack599(sprite, target);
            if (!p.Ready)
                return;
            V v_dam = 0;
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
            if (V.T(((V)(p.Call("get_mana", v_myid)) < (V)((V)50L))))
            {
                p.Call("message", (V)3L, (V)"사용하기에 마력량이적습니다. [필요마나 : 50이상]");
                return;
            }
            p.Call("manal_del", (V)"50");
            v_dam = ((V)(((V)(p.Call("get_mgc_damage", v_myid)) / (V)((V)10L))) * (V)((V)19L));
            if (V.T(p.Call("get_critical", v_myid)))
            {
                v_dam = ((V)(v_dam) * (V)((V)2L));
            }
            v_x1 = p.Call("get_xs");
            v_y1 = p.Call("get_ys");
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
                p.Call("motion", (V)136L, (V)20L);
                p.Call("game_sound", (V)47L, (V)0L);
                p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                p.Call("damaged", v_mob1, v_dam);
                p.Call("damaged", v_mob2, v_dam);
                p.Call("damaged", v_mob3, v_dam);
                p.Call("damaged", v_mob4, v_dam);
            }
            else
                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                {
                    p.Call("motion", (V)136L, (V)20L);
                    p.Call("game_sound", (V)47L, (V)0L);
                    p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                    p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                    p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                    p.Call("damaged", v_mob2, v_dam);
                    p.Call("damaged", v_mob3, v_dam);
                    p.Call("damaged", v_mob4, v_dam);
                }
                else
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                    {
                        p.Call("motion", (V)136L, (V)20L);
                        p.Call("game_sound", (V)47L, (V)0L);
                        p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                        p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                        p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                        p.Call("damaged", v_mob1, v_dam);
                        p.Call("damaged", v_mob3, v_dam);
                        p.Call("damaged", v_mob4, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                        {
                            p.Call("motion", (V)136L, (V)20L);
                            p.Call("game_sound", (V)47L, (V)0L);
                            p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                            p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                            p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                            p.Call("damaged", v_mob1, v_dam);
                            p.Call("damaged", v_mob2, v_dam);
                            p.Call("damaged", v_mob4, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                            {
                                p.Call("motion", (V)136L, (V)20L);
                                p.Call("game_sound", (V)47L, (V)0L);
                                p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                p.Call("damaged", v_mob1, v_dam);
                                p.Call("damaged", v_mob2, v_dam);
                                p.Call("damaged", v_mob3, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                                {
                                    p.Call("motion", (V)136L, (V)20L);
                                    p.Call("game_sound", (V)47L, (V)0L);
                                    p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                    p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                    p.Call("damaged", v_mob3, v_dam);
                                    p.Call("damaged", v_mob4, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                                    {
                                        p.Call("motion", (V)136L, (V)20L);
                                        p.Call("game_sound", (V)47L, (V)0L);
                                        p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                        p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                        p.Call("damaged", v_mob2, v_dam);
                                        p.Call("damaged", v_mob4, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                        {
                                            p.Call("motion", (V)136L, (V)20L);
                                            p.Call("game_sound", (V)47L, (V)0L);
                                            p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                            p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                            p.Call("damaged", v_mob2, v_dam);
                                            p.Call("damaged", v_mob3, v_dam);
                                        }
                                        else
                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                                            {
                                                p.Call("motion", (V)136L, (V)20L);
                                                p.Call("game_sound", (V)47L, (V)0L);
                                                p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                p.Call("damaged", v_mob1, v_dam);
                                                p.Call("damaged", v_mob4, v_dam);
                                            }
                                            else
                                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                {
                                                    p.Call("motion", (V)136L, (V)20L);
                                                    p.Call("game_sound", (V)47L, (V)0L);
                                                    p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                    p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                    p.Call("damaged", v_mob1, v_dam);
                                                    p.Call("damaged", v_mob3, v_dam);
                                                }
                                                else
                                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                    {
                                                        p.Call("motion", (V)136L, (V)20L);
                                                        p.Call("game_sound", (V)47L, (V)0L);
                                                        p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                        p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                        p.Call("damaged", v_mob1, v_dam);
                                                        p.Call("damaged", v_mob2, v_dam);
                                                    }
                                                    else
                                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) == (V)((V)1L)))))))))
                                                        {
                                                            p.Call("motion", (V)136L, (V)20L);
                                                            p.Call("game_sound", (V)47L, (V)0L);
                                                            p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                            p.Call("damaged", v_mob4, v_dam);
                                                        }
                                                        else
                                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                            {
                                                                p.Call("motion", (V)136L, (V)20L);
                                                                p.Call("game_sound", (V)47L, (V)0L);
                                                                p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                                p.Call("damaged", v_mob3, v_dam);
                                                            }
                                                            else
                                                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)1L))) && V.T(((V)(v_type2) == (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                                {
                                                                    p.Call("motion", (V)136L, (V)20L);
                                                                    p.Call("game_sound", (V)47L, (V)0L);
                                                                    p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                                    p.Call("damaged", v_mob2, v_dam);
                                                                }
                                                                else
                                                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)1L))) && V.T(((V)(v_type2) != (V)((V)1L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)1L))) && V.T(((V)(v_type4) != (V)((V)1L)))))))))
                                                                    {
                                                                        p.Call("motion", (V)136L, (V)20L);
                                                                        p.Call("game_sound", (V)47L, (V)0L);
                                                                        p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                                        p.Call("damaged", v_mob1, v_dam);
                                                                    }
            if (V.T(p.Call("get_map_pk")))
            {
                v_mob1 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) - (V)((V)1L)));
                v_mob2 = p.Call("get_char_serial", v_myid, v_x1, ((V)((v_y1)) + (V)((V)1L)));
                v_mob3 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) - (V)((V)1L)), v_y1);
                v_mob4 = p.Call("get_char_serial", v_myid, ((V)((v_x1)) + (V)((V)1L)), v_y1);
                v_type1 = p.Call("istype", v_mob1);
                v_type2 = p.Call("istype", v_mob2);
                v_type3 = p.Call("istype", v_mob3);
                v_type4 = p.Call("istype", v_mob4);
                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)3L))) && V.T(((V)(v_type2) == (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)3L))) && V.T(((V)(v_type4) == (V)((V)3L)))))))))
                {
                    p.Call("motion", (V)136L, (V)20L);
                    p.Call("game_sound", (V)47L, (V)0L);
                    p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                    p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                    p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                    p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                    p.Call("char_damaged", v_mob1, v_dam, v_dam);
                    p.Call("char_damaged", v_mob2, v_dam, v_dam);
                    p.Call("char_damaged", v_mob3, v_dam, v_dam);
                    p.Call("char_damaged", v_mob4, v_dam, v_dam);
                }
                else
                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)3L))) && V.T(((V)(v_type2) == (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)3L))) && V.T(((V)(v_type4) == (V)((V)3L)))))))))
                    {
                        p.Call("motion", (V)136L, (V)20L);
                        p.Call("game_sound", (V)47L, (V)0L);
                        p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                        p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                        p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                        p.Call("char_damaged", v_mob2, v_dam, v_dam);
                        p.Call("char_damaged", v_mob3, v_dam, v_dam);
                        p.Call("char_damaged", v_mob4, v_dam, v_dam);
                    }
                    else
                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)3L))) && V.T(((V)(v_type2) != (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)3L))) && V.T(((V)(v_type4) == (V)((V)3L)))))))))
                        {
                            p.Call("motion", (V)136L, (V)20L);
                            p.Call("game_sound", (V)47L, (V)0L);
                            p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                            p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                            p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                            p.Call("char_damaged", v_mob1, v_dam, v_dam);
                            p.Call("char_damaged", v_mob3, v_dam, v_dam);
                            p.Call("char_damaged", v_mob4, v_dam, v_dam);
                        }
                        else
                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)3L))) && V.T(((V)(v_type2) == (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)3L))) && V.T(((V)(v_type4) == (V)((V)3L)))))))))
                            {
                                p.Call("motion", (V)136L, (V)20L);
                                p.Call("game_sound", (V)47L, (V)0L);
                                p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                p.Call("char_damaged", v_mob1, v_dam, v_dam);
                                p.Call("char_damaged", v_mob2, v_dam, v_dam);
                                p.Call("char_damaged", v_mob4, v_dam, v_dam);
                            }
                            else
                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)3L))) && V.T(((V)(v_type2) == (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)3L))) && V.T(((V)(v_type4) != (V)((V)3L)))))))))
                                {
                                    p.Call("motion", (V)136L, (V)20L);
                                    p.Call("game_sound", (V)47L, (V)0L);
                                    p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                    p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                    p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                    p.Call("char_damaged", v_mob1, v_dam, v_dam);
                                    p.Call("char_damaged", v_mob2, v_dam, v_dam);
                                    p.Call("char_damaged", v_mob3, v_dam, v_dam);
                                }
                                else
                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)3L))) && V.T(((V)(v_type2) != (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)3L))) && V.T(((V)(v_type4) == (V)((V)3L)))))))))
                                    {
                                        p.Call("motion", (V)136L, (V)20L);
                                        p.Call("game_sound", (V)47L, (V)0L);
                                        p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                        p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                        p.Call("char_damaged", v_mob3, v_dam, v_dam);
                                        p.Call("char_damaged", v_mob4, v_dam, v_dam);
                                    }
                                    else
                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)3L))) && V.T(((V)(v_type2) == (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)3L))) && V.T(((V)(v_type4) == (V)((V)3L)))))))))
                                        {
                                            p.Call("motion", (V)136L, (V)20L);
                                            p.Call("game_sound", (V)47L, (V)0L);
                                            p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                            p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                            p.Call("char_damaged", v_mob2, v_dam, v_dam);
                                            p.Call("char_damaged", v_mob4, v_dam, v_dam);
                                        }
                                        else
                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)3L))) && V.T(((V)(v_type2) == (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)3L))) && V.T(((V)(v_type4) != (V)((V)3L)))))))))
                                            {
                                                p.Call("motion", (V)136L, (V)20L);
                                                p.Call("game_sound", (V)47L, (V)0L);
                                                p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                p.Call("char_damaged", v_mob2, v_dam, v_dam);
                                                p.Call("char_damaged", v_mob3, v_dam, v_dam);
                                            }
                                            else
                                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)3L))) && V.T(((V)(v_type2) != (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)3L))) && V.T(((V)(v_type4) == (V)((V)3L)))))))))
                                                {
                                                    p.Call("motion", (V)136L, (V)20L);
                                                    p.Call("game_sound", (V)47L, (V)0L);
                                                    p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                    p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                    p.Call("char_damaged", v_mob1, v_dam, v_dam);
                                                    p.Call("char_damaged", v_mob4, v_dam, v_dam);
                                                }
                                                else
                                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)3L))) && V.T(((V)(v_type2) != (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)3L))) && V.T(((V)(v_type4) != (V)((V)3L)))))))))
                                                    {
                                                        p.Call("motion", (V)136L, (V)20L);
                                                        p.Call("game_sound", (V)47L, (V)0L);
                                                        p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                        p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                        p.Call("char_damaged", v_mob1, v_dam, v_dam);
                                                        p.Call("char_damaged", v_mob3, v_dam, v_dam);
                                                    }
                                                    else
                                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)3L))) && V.T(((V)(v_type2) == (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)3L))) && V.T(((V)(v_type4) != (V)((V)3L)))))))))
                                                        {
                                                            p.Call("motion", (V)136L, (V)20L);
                                                            p.Call("game_sound", (V)47L, (V)0L);
                                                            p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                            p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                            p.Call("char_damaged", v_mob1, v_dam, v_dam);
                                                            p.Call("char_damaged", v_mob2, v_dam, v_dam);
                                                        }
                                                        else
                                                            if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)3L))) && V.T(((V)(v_type2) != (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)3L))) && V.T(((V)(v_type4) == (V)((V)3L)))))))))
                                                            {
                                                                p.Call("motion", (V)136L, (V)20L);
                                                                p.Call("game_sound", (V)47L, (V)0L);
                                                                p.Call("effect", v_mob4, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                                p.Call("char_damaged", v_mob4, v_dam, v_dam);
                                                            }
                                                            else
                                                                if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)3L))) && V.T(((V)(v_type2) != (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) == (V)((V)3L))) && V.T(((V)(v_type4) != (V)((V)3L)))))))))
                                                                {
                                                                    p.Call("motion", (V)136L, (V)20L);
                                                                    p.Call("game_sound", (V)47L, (V)0L);
                                                                    p.Call("effect", v_mob3, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                                    p.Call("char_damaged", v_mob3, v_dam, v_dam);
                                                                }
                                                                else
                                                                    if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) != (V)((V)3L))) && V.T(((V)(v_type2) == (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)3L))) && V.T(((V)(v_type4) != (V)((V)3L)))))))))
                                                                    {
                                                                        p.Call("motion", (V)136L, (V)20L);
                                                                        p.Call("game_sound", (V)47L, (V)0L);
                                                                        p.Call("effect", v_mob2, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                                        p.Call("char_damaged", v_mob2, v_dam, v_dam);
                                                                    }
                                                                    else
                                                                        if (V.T(V.B(V.T((V.B(V.T(((V)(v_type1) == (V)((V)3L))) && V.T(((V)(v_type2) != (V)((V)3L)))))) && V.T((V.B(V.T(((V)(v_type3) != (V)((V)3L))) && V.T(((V)(v_type4) != (V)((V)3L)))))))))
                                                                        {
                                                                            p.Call("motion", (V)136L, (V)20L);
                                                                            p.Call("game_sound", (V)47L, (V)0L);
                                                                            p.Call("effect", v_mob1, (V)0L, (V)10L, (V)75L);  // 노바 이펙트(5.99: 0, 234)
                                                                            p.Call("char_damaged", v_mob1, v_dam, v_dam);
                                                                        }
            }
        }
    }
}
