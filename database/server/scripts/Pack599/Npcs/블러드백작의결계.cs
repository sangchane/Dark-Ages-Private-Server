using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 블러드백작의결계 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_블러드백작의결계", "5.99표")]
    public class NpcBE14B7ECB4DCBC31C791C758ACB0ACC4 : PackNpc
    {
        public NpcBE14B7ECB4DCBC31C791C758ACB0ACC4(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_mob = 0;
            V v_mob1 = 0;
            V v_mob2 = 0;
            V v_myid = 0;
            V v_name1_s = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            L_re: ;
            yield return Menu((V)"베크나탑의기운이 느껴진다.\\n입장 하겠습니까?\\n(그룹장만 입장신청이 가능합니다.)\\n(입장제한X)", (V)"입장한다.", (V)"입장하지 않는다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(V.B(!V.T(p.Call("group_master", v_myid)))))
                {
                    yield return Mes((V)0L, (V)"당신은 그룹이 없거나, 그룹장이 아닙니다.");
                    yield break;
                }
                v_name1_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"블러드백작의결계"));
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name1_s, (V)"블러드백작의결계", (V)30L, (V)31L, (V)142L, (V)6L, (V)1L, (V)"db/maps/드라큐라백작의성/maps/lod117.map");
                p.Call("group_val", (V)"#bloodearl", (V)1L, (V)1L);
                p.Call("item_clear", v_name1_s);
                p.Call("mob_clear", v_name1_s);
                p.Call("group_warp", v_name1_s, (V)13L, (V)29L);
                p.Call("group_message", (V)3L, (V)"{=c※던전※ 베크나탑에 입장하셧습니다.");
                p.Call("message", (V)3L, (V)"{=c※던전※ 베크나탑에 입장하셧습니다.");
                p.Call("sleep", (V)1000L);
                p.Call("mob_spawn2", (V)"유령하녀1", (V)11L, (V)23L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)9L)));
                p.Call("mob_spawn2", (V)"유령하녀2", (V)14L, (V)25L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)9L)));
                p.Call("mob_spawn2", (V)"유령하녀3", (V)17L, (V)23L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)9L)));
                p.Call("mob_spawn2", (V)"로밍체어3", (V)19L, (V)20L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)4L)));
                p.Call("mob_spawn2", (V)"로밍레더3", (V)5L, (V)12L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)4L)));
                p.Call("mob_spawn2", (V)"부3", (V)7L, (V)15L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)4L)));
                p.Call("mob_spawn2", (V)"로밍스테츄3", (V)23L, (V)28L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)4L)));
                p.Call("mob_spawn2", (V)"미러테이블3", (V)7L, (V)25L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)4L)));
                p.Call("mob_spawn2", (V)"레아로3", (V)16L, (V)16L, (V)1L, ((V)(p.Call("group_bighp")) * (V)((V)4L)));
                v_mob = p.Call("get_mobxy", (V)11L, (V)23L);
                v_mob1 = p.Call("get_mobxy", (V)14L, (V)25L);
                v_mob2 = p.Call("get_mobxy", (V)17L, (V)23L);
                if (V.T(v_mob))
                {
                    p.Call("effect", v_mob, (V)0L, (V)307L, (V)100L);
                    p.Call("mob_say2", v_mob, (V)0L, (V)0L, (V)"나가 주세요..");
                }
                if (V.T(v_mob1))
                {
                    p.Call("effect", v_mob1, (V)0L, (V)307L, (V)100L);
                    p.Call("mob_say2", v_mob1, (V)0L, (V)0L, (V)"나가 주세요..");
                }
                if (V.T(v_mob2))
                {
                    p.Call("effect", v_mob2, (V)0L, (V)307L, (V)100L);
                    p.Call("mob_say2", v_mob2, (V)0L, (V)0L, (V)"나가 주세요..");
                }
                yield break;
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)0L, (V)"입장을 하지 않습니다.");
                    yield break;
                }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
