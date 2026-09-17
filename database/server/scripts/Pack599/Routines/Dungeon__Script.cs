using System.Collections.Generic;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// Dungeon__Script — 5.99 `script/Dungeon.txt` 를 그대로 옮긴 것. 개인 던전 사본 안의 사람마다 1초에 한 번 돈다(PackRoutine).
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("PACK_Dungeon__Script", "5.99표")]
    public class Routine00440075006E00670065006F006E005F005F005300630072006900700074 : PackRoutine
    {
        public Routine00440075006E00670065006F006E005F005F005300630072006900700074(Area area) : base(area)
        {
        }

        protected override IEnumerable<int> Run(Pack599 p)
        {
            V v_mob = 0;
            V v_myid = 0;
            V v_point = 0;

            v_myid = p.Call("get_myid");
            if (V.T(V.B(V.T(((V)(p.Call("get_map_stage", v_myid)) == (V)((V)0L))) || V.T(V.B(!V.T(p.Call("get_map_stage", v_myid)))))))
            {
                p["#stage2"] = (V)0L;
            }
            if (V.T(((V)(p.Call("get_map_stage", v_myid)) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)3L))))
                {
                    if (V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))))
                    {
                        if (V.T(((V)(p["#end_time"]) > (V)((V)0L))))
                        {
                            p["#end_time"] = ((V)(p["#end_time"]) - (V)((V)1L));
                            p.Call("message", (V)3L, ((V)(((V)((V)"{=c안내 : ") + (V)(p["#end_time"]))) + (V)((V)"초뒤 던전에서 퇴장합니다.")));
                            yield break;
                        }
                        if (V.T(((V)(p.Call("get_level", v_myid)) < (V)((V)5L))))
                        {
                            v_point = ((V)((((V)((V)5L) - (V)(p.Call("get_level", v_myid))))) * (V)((V)2L));
                            p.Call("set_point", ((V)(p.Call("get_point", v_myid)) + (V)(v_point)));
                            p.Call("set_level", (V)5L);
                        }
                        p["#realmap"] = (V)4L;
                        p.Call("set_nation", v_myid, (V)1L);
                        p.Call("message", (V)8L, ((V)(((V)(((V)(((V)((V)"{=u\\n던전클리어\\n\\n던전이름 : 초보자튜토리얼\\n\\n클리어타임(초) : ") + (V)(p.Call("get_clear_time", v_myid)))) + (V)((V)"\\n\\n잡은 몬스터수 : "))) + (V)(p.Call("get_kill_mob", v_myid)))) + (V)((V)"\\n\\n클리어경험치 : 레벨5\\n\\n마이소시아 국적을 따셧습니다!")));
                        p.Call("legend_add", (V)1L, (V)75L, (V)"어둠의전설, 초보자 튜토리얼을 완료 하였다.");
                        p.Call("warp", (V)"노비스마을", (V)38L, (V)31L);
                        p.Call("effect", v_myid, (V)79L, (V)0L, (V)100L);
                    }
                    else
                    {
                        p["#end_time"] = (V)5L;
                    }
                }
            }
            if (V.T(((V)(p.Call("get_map_stage", v_myid)) == (V)((V)2L))))
            {
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)3L))))
                {
                    if (V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))))
                    {
                        if (V.T(((V)(p["#end_time"]) > (V)((V)0L))))
                        {
                            p["#end_time"] = ((V)(p["#end_time"]) - (V)((V)1L));
                            p.Call("message", (V)3L, ((V)(((V)((V)"{=c안내 : ") + (V)(p["#end_time"]))) + (V)((V)"초뒤 던전에서 퇴장합니다.")));
                            yield break;
                        }
                        p.Call("exp_add", (V)50000L);
                        p.Call("message", (V)8L, ((V)(((V)(((V)(((V)((V)"{=u\\n던전클리어\\n\\n던전이름 : 노비스지하동굴\\n\\n클리어타임(초) : ") + (V)(p.Call("get_clear_time", v_myid)))) + (V)((V)"\\n\\n잡은 몬스터수 : "))) + (V)(p.Call("get_kill_mob", v_myid)))) + (V)((V)"\\n\\n클리어경험치 : 50000")));
                        p.Call("warp", (V)"노비스지하던전B2", (V)16L, (V)16L);
                    }
                    else
                    {
                        p["#end_time"] = (V)5L;
                    }
                }
            }
            if (V.T(((V)(p.Call("get_map_stage", v_myid)) == (V)((V)3L))))
            {
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)1L))))
                {
                    if (V.T(V.B(V.T(((V)(p.Call("get_xs", v_myid)) == (V)(p["#fota_x"]))) && V.T(((V)(p.Call("get_ys", v_myid)) == (V)(p["#fota_y"]))))))
                    {
                        yield break;
                    }
                    if (V.T(((V)(p.Call("rand", (V)1L, (V)5L)) == (V)((V)3L))))
                    {
                        if (V.T(p.Call("item_exist", v_myid, (V)"엔트자이언트의날개")))
                        {
                            p.Call("message", (V)3L, (V)"엔트자이언트의 날개를 소모하였다.");
                            p.Call("item_del", (V)"엔트자이언트의날개", (V)1L);
                            yield break;
                        }
                        p.Call("set_coma", v_myid, (V)1L);
                        p.Call("set_state", (V)1L, (V)1L);
                        p.Call("coma_delay", v_myid, (V)12L);
                        yield break;
                    }
                    p["#fota_x"] = p.Call("get_xs", v_myid);
                    p["#fota_y"] = p.Call("get_ys", v_myid);
                }
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)3L))))
                {
                    if (V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))))
                    {
                        if (V.T(((V)(p["#end_time"]) > (V)((V)0L))))
                        {
                            p["#end_time"] = ((V)(p["#end_time"]) - (V)((V)1L));
                            p.Call("message", (V)3L, ((V)(((V)((V)"{=c안내 : ") + (V)(p["#end_time"]))) + (V)((V)"초뒤 던전에서 퇴장합니다.")));
                            yield break;
                        }
                        p.Call("exp_add", (V)200000L);
                        p.Call("message", (V)8L, ((V)(((V)(((V)(((V)((V)"{=u\\n던전클리어\\n\\n던전이름 : 포테의숲오솔길\\n\\n클리어타임(초) : ") + (V)(p.Call("get_clear_time", v_myid)))) + (V)((V)"\\n\\n잡은 몬스터수 : "))) + (V)(p.Call("get_kill_mob", v_myid)))) + (V)((V)"\\n\\n클리어경험치 : 200000")));
                        p.Call("warp", (V)"포테의숲5존", (V)16L, (V)16L);
                    }
                    else
                    {
                        p["#end_time"] = (V)5L;
                    }
                }
            }
            if (V.T(((V)(p.Call("get_map_stage", v_myid)) == (V)((V)4L))))
            {
                if (V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))))
                {
                    if (V.T(((V)(p["#end_time"]) > (V)((V)0L))))
                    {
                        p["#end_time"] = ((V)(p["#end_time"]) - (V)((V)1L));
                        p.Call("message", (V)3L, ((V)(((V)((V)"{=c안내 : ") + (V)(p["#end_time"]))) + (V)((V)"초뒤 던전에서 퇴장합니다.")));
                        yield break;
                    }
                    p.Call("message", (V)8L, ((V)(((V)(((V)((V)"{=u\\n던전클리어\\n\\n던전이름 : 박물관옥탑\\n\\n클리어타임(초) : ") + (V)(p.Call("get_clear_time", v_myid)))) + (V)((V)"\\n\\n잡은 몬스터수 : "))) + (V)(p.Call("get_kill_mob", v_myid))));
                    p.Call("warp", (V)"박물관3층", (V)12L, (V)18L);
                }
                else
                {
                    p["#end_time"] = (V)5L;
                }
            }
            if (V.T(((V)(p.Call("get_map_stage", v_myid)) == (V)((V)6L))))
            {
                if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#bloodearl"]) == (V)((V)2L))))))
                {
                    p.Call("group_val", (V)"#bloodearl", (V)3L, (V)1L);
                    p.Call("sleep", (V)1300L);
                    if (V.T(p.Call("map_objmob", v_myid)))
                    {
                        yield break;
                    }
                    p.Call("mob_spawn2", (V)"블러드백작", (V)14L, (V)18L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)23L)));
                    p.Call("group_message", (V)3L, (V)"{=c안내 : 블러드백작이 잠에서 깨어 났습니다!");
                    p.Call("message", (V)3L, (V)"{=c안내 : 블러드백작이 잠에서 깨어 났습니다!");
                    v_mob = p.Call("get_mobxy", (V)14L, (V)18L);
                    p.Call("effect", v_mob, (V)0L, (V)306L, (V)200L);
                    p.Call("mob_say2", v_mob, (V)0L, (V)0L, (V)"누가 감히 나의 잠을 깨우느냐!!");
                }
                if (V.T(((V)(p["#bloodearl"]) == (V)((V)4L))))
                {
                    p["#end_time"] = (V)10L;
                    p["#bloodearl"] = (V)5L;
                }
                if (V.T(((V)(p["#bloodearl"]) == (V)((V)5L))))
                {
                    if (V.T(((V)(p["#end_time"]) > (V)((V)0L))))
                    {
                        p["#end_time"] = ((V)(p["#end_time"]) - (V)((V)1L));
                        p.Call("message", (V)3L, ((V)(((V)((V)"{=c안내 : ") + (V)(p["#end_time"]))) + (V)((V)"초뒤 던전에서 퇴장합니다.")));
                        yield break;
                    }
                    p.Call("exp_add", (V)5000000L);
                    p.Call("message", (V)8L, ((V)(((V)(((V)(((V)((V)"{=u\\n던전클리어\\n\\n던전이름 : 블러드백작의결계\\n\\n클리어타임(초) : ") + (V)(p.Call("get_clear_time", v_myid)))) + (V)((V)"\\n\\n잡은 몬스터수 : "))) + (V)(p.Call("get_kill_mob", v_myid)))) + (V)((V)"\\n\\n경험치 : 5백만")));
                    p.Call("warp", (V)"베크나탑미로대기실", (V)11L, (V)6L);
                }
            }
            if (V.T(((V)(p.Call("get_map_stage", v_myid)) == (V)((V)9L))))
            {
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)2L))))
                {
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest1"]) == (V)((V)0L))))))
                    {
                        p.Call("sleep", p.Call("rand", (V)100L, (V)800L));
                        if (V.T(((V)(p["#jobquest1"]) != (V)((V)0L))))
                        {
                            yield break;
                        }
                        p.Call("group_val", (V)"#jobquest1", (V)1L, (V)1L);
                        p.Call("mob_spawn2", (V)"결계남전사", (V)1L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계여도적", (V)3L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남법사", (V)4L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계전사", (V)4L, (V)1L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)13L)));
                        p.Call("mob_spawn2", (V)"결계여직자", (V)5L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남도가", (V)7L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                    }
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest1"]) == (V)((V)1L))))))
                    {
                        p.Call("group_val", (V)"#jobquest1", (V)2L, (V)1L);
                    }
                }
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)3L))))
                {
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest2"]) == (V)((V)0L))))))
                    {
                        p.Call("sleep", p.Call("rand", (V)100L, (V)800L));
                        if (V.T(((V)(p["#jobquest2"]) != (V)((V)0L))))
                        {
                            yield break;
                        }
                        p.Call("group_val", (V)"#jobquest2", (V)1L, (V)1L);
                        p.Call("mob_spawn2", (V)"결계남전사", (V)1L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계여도적", (V)3L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남법사", (V)4L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계도적", (V)4L, (V)1L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)13L)));
                        p.Call("mob_spawn2", (V)"결계여직자", (V)5L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남도가", (V)7L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                    }
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest2"]) == (V)((V)1L))))))
                    {
                        p.Call("group_val", (V)"#jobquest2", (V)2L, (V)1L);
                    }
                }
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)4L))))
                {
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest3"]) == (V)((V)0L))))))
                    {
                        p.Call("sleep", p.Call("rand", (V)100L, (V)800L));
                        if (V.T(((V)(p["#jobquest3"]) != (V)((V)0L))))
                        {
                            yield break;
                        }
                        p.Call("group_val", (V)"#jobquest3", (V)1L, (V)1L);
                        p.Call("mob_spawn2", (V)"결계남전사", (V)1L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계여도적", (V)3L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남법사", (V)4L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계도가", (V)4L, (V)1L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)13L)));
                        p.Call("mob_spawn2", (V)"결계여직자", (V)5L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남도가", (V)7L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                    }
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest3"]) == (V)((V)1L))))))
                    {
                        p.Call("group_val", (V)"#jobquest3", (V)2L, (V)1L);
                    }
                }
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)5L))))
                {
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest4"]) == (V)((V)0L))))))
                    {
                        p.Call("sleep", p.Call("rand", (V)100L, (V)800L));
                        if (V.T(((V)(p["#jobquest4"]) != (V)((V)0L))))
                        {
                            yield break;
                        }
                        p.Call("group_val", (V)"#jobquest4", (V)1L, (V)1L);
                        p.Call("mob_spawn2", (V)"결계남전사", (V)4L, (V)1L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계여도적", (V)4L, (V)3L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남법사", (V)4L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계법사", (V)1L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)13L)));
                        p.Call("mob_spawn2", (V)"결계여직자", (V)4L, (V)5L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남도가", (V)4L, (V)7L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                    }
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest4"]) == (V)((V)1L))))))
                    {
                        p.Call("group_val", (V)"#jobquest4", (V)2L, (V)1L);
                    }
                }
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)6L))))
                {
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest5"]) == (V)((V)0L))))))
                    {
                        p.Call("sleep", p.Call("rand", (V)100L, (V)800L));
                        if (V.T(((V)(p["#jobquest5"]) != (V)((V)0L))))
                        {
                            yield break;
                        }
                        p.Call("group_val", (V)"#jobquest5", (V)1L, (V)1L);
                        p.Call("mob_spawn2", (V)"결계남전사", (V)4L, (V)1L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계여도적", (V)4L, (V)3L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남법사", (V)4L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계직자", (V)1L, (V)4L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)13L)));
                        p.Call("mob_spawn2", (V)"결계여직자", (V)4L, (V)5L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                        p.Call("mob_spawn2", (V)"결계남도가", (V)4L, (V)7L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)8L)));
                    }
                    if (V.T(V.B(V.T(((V)(p.Call("map_objmob", v_myid)) <= (V)((V)0L))) && V.T(((V)(p["#jobquest5"]) == (V)((V)1L))))))
                    {
                        p.Call("group_val", (V)"#jobquest5", (V)2L, (V)1L);
                    }
                }
                if (V.T(((V)(p.Call("get_map_sub_stage", v_myid)) == (V)((V)8L))))
                {
                    if (V.T(((V)(p["#jobquest6"]) == (V)((V)2L))))
                    {
                        p["#end_time"] = (V)6L;
                        p["#jobquest6"] = (V)3L;
                    }
                    else
                        if (V.T(((V)(p["#jobquest6"]) == (V)((V)3L))))
                        {
                            if (V.T(((V)(p["#end_time"]) > (V)((V)0L))))
                            {
                                p["#end_time"] = ((V)(p["#end_time"]) - (V)((V)1L));
                                p.Call("message", (V)3L, ((V)(((V)((V)"{=c안내 : ") + (V)(p["#end_time"]))) + (V)((V)"초뒤 던전에서 퇴장합니다.")));
                                yield break;
                            }
                            p.Call("exp_add", (V)450000000L);
                            p.Call("message", (V)8L, ((V)(((V)(((V)(((V)((V)"{=u\\n던전클리어\\n\\n던전이름 : 직업퀘스트\\n\\n클리어타임(초) : ") + (V)(p.Call("get_clear_time", v_myid)))) + (V)((V)"\\n\\n잡은 몬스터수 : "))) + (V)(p.Call("get_kill_mob", v_myid)))) + (V)((V)"\\n\\n경험치 : 450000000")));
                            p.Call("warp", (V)"죽음의마을입구5", (V)10L, (V)10L);
                        }
                }
            }
            yield break;
        }
    }
}
