using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 호러캐슬 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_호러캐슬", "5.99표")]
    public class NpcD638B7ECCE90C2AC : PackNpc
    {
        public NpcD638B7ECCE90C2AC(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_i = 0;
            V v_myid = 0;
            V v_name1_s = 0;
            V v_rand1 = 0;
            V v_rand2 = 0;
            V v_rand3 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(V.B(!V.T(p.Call("group_exist", v_myid)))))
            {
                p.Call("message", (V)3L, (V)"당신은 그룹이 없습니다.");
                p.Call("warp", (V)"호러캐슬메인홀", (V)23L, (V)15L);
                yield break;
            }
            if (V.T(((V)(p.Call("get_class_sub", v_myid)) != (V)((V)0L))))
            {
                p.Call("message", (V)3L, (V)"승급자는 입장불가");
                p.Call("warp", (V)"호러캐슬메인홀", (V)23L, (V)15L);
                yield break;
            }
            if (V.T(p.Call("group_class_sub")))
            {
                p.Call("message", (V)3L, (V)"그룹원중에 승급자가 존재하여 대기실로 나가집니다.");
                p.Call("group_warp", (V)"호러캐슬메인홀", (V)23L, (V)15L);
                yield break;
            }
            if (V.T(V.B(V.T(((V)(p.Call("group_bighp", v_myid)) > (V)((V)50000L))) || V.T(((V)(p.Call("group_bigmp", v_myid)) > (V)((V)30000L))))))
            {
                p.Call("message", (V)3L, (V)"강한 힘을 가진 그룹원이 존재하여, 몬스터가 강해집니다.");
            }
            v_name1_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"호러캐슬"));
            p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
            p.Call("map_create", p["$map_num"], v_name1_s, (V)"호러캐슬", (V)15L, (V)15L, (V)154L, (V)0L, (V)0L, (V)"db/maps/호러캐슬/maps/lod0001.map");
            p.Call("warp_create", (V)3L, v_name1_s, (V)9L, (V)0L, (V)"호러캐슬다음방", (V)0L, (V)0L, (V)99L, (V)99L, (V)0L);
            p.Call("item_clear", v_name1_s);
            p.Call("mob_clear", v_name1_s);
            p.Call("group_warp", v_name1_s, (V)9L, (V)2L);
            if (V.T(((V)(p.Call("group_bighp", v_myid)) < (V)((V)50000L))))
            {
                p.Call("sleep", (V)300L);
                if (V.T(((V)(p.Call("map_objmob")) >= (V)((V)1L))))
                {
                    yield break;
                }
                for (v_i = (V)1L; V.T(((V)(v_i) <= (V)((V)17L))); v_i = ((V)(v_i) + (V)((V)1L)))
                {
                    L_AA: ;
                    v_rand1 = p.Call("rand", (V)1L, ((V)(p.Call("get_mapxs", v_myid)) - (V)((V)8L)));
                    if (V.T(((V)(v_rand1) <= (V)((V)0L))))
                    {
                        goto L_AA;
                    }
                    L_BB: ;
                    v_rand2 = p.Call("rand", (V)1L, ((V)(p.Call("get_mapys", v_myid)) - (V)((V)1L)));
                    if (V.T(((V)(v_rand2) <= (V)((V)0L))))
                    {
                        goto L_BB;
                    }
                    L_CC: ;
                    v_rand3 = p.Call("rand", (V)0L, (V)9L);
                    if (V.T(V.B(V.T(((V)(v_rand3) <= (V)((V)0L))) || V.T(((V)(v_rand3) > (V)((V)7L))))))
                    {
                        goto L_CC;
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)1L))))
                    {
                        p.Call("mob_spawn", (V)"바탈리우스1", v_rand1, v_rand2, (V)1L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)2L))))
                    {
                        p.Call("mob_spawn", (V)"헤라시우스1", v_rand1, v_rand2, (V)2L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)3L))))
                    {
                        p.Call("mob_spawn", (V)"헤라시우스1", v_rand1, v_rand2, (V)3L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)4L))))
                    {
                        p.Call("mob_spawn", (V)"헤브렐리우스1", v_rand1, v_rand2, (V)1L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)5L))))
                    {
                        p.Call("mob_spawn", (V)"바클라시우스1", v_rand1, v_rand2, (V)2L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)6L))))
                    {
                        p.Call("mob_spawn", (V)"슈크래시우스1", v_rand1, v_rand2, (V)3L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)7L))))
                    {
                        p.Call("mob_spawn", (V)"타클라시우스1", v_rand1, v_rand2, (V)1L);
                    }
                }
            }
            else
            {
                p.Call("sleep", (V)300L);
                if (V.T(((V)(p.Call("map_objmob")) >= (V)((V)1L))))
                {
                    yield break;
                }
                for (v_i = (V)1L; V.T(((V)(v_i) <= (V)((V)17L))); v_i = ((V)(v_i) + (V)((V)1L)))
                {
                    L_DD: ;
                    v_rand1 = p.Call("rand", (V)1L, ((V)(p.Call("get_mapxs", v_myid)) - (V)((V)8L)));
                    if (V.T(((V)(v_rand1) <= (V)((V)0L))))
                    {
                        goto L_DD;
                    }
                    L_EE: ;
                    v_rand2 = p.Call("rand", (V)1L, ((V)(p.Call("get_mapys", v_myid)) - (V)((V)1L)));
                    if (V.T(((V)(v_rand2) <= (V)((V)0L))))
                    {
                        goto L_EE;
                    }
                    L_FF: ;
                    v_rand3 = p.Call("rand", (V)0L, (V)9L);
                    if (V.T(V.B(V.T(((V)(v_rand3) <= (V)((V)0L))) || V.T(((V)(v_rand3) > (V)((V)7L))))))
                    {
                        goto L_FF;
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)1L))))
                    {
                        p.Call("mob_spawn", (V)"바탈리우스2", v_rand1, v_rand2, (V)1L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)2L))))
                    {
                        p.Call("mob_spawn", (V)"헤라시우스2", v_rand1, v_rand2, (V)2L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)3L))))
                    {
                        p.Call("mob_spawn", (V)"헤라시우스2", v_rand1, v_rand2, (V)3L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)4L))))
                    {
                        p.Call("mob_spawn", (V)"헤브렐리우스2", v_rand1, v_rand2, (V)1L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)5L))))
                    {
                        p.Call("mob_spawn", (V)"바클라시우스2", v_rand1, v_rand2, (V)2L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)6L))))
                    {
                        p.Call("mob_spawn", (V)"슈크래시우스2", v_rand1, v_rand2, (V)3L);
                    }
                    if (V.T(((V)(v_rand3) == (V)((V)7L))))
                    {
                        p.Call("mob_spawn", (V)"타클라시우스2", v_rand1, v_rand2, (V)1L);
                    }
                }
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
