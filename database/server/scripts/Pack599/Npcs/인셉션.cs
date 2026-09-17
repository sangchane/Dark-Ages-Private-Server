using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 인셉션 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_인셉션", "5.99표")]
    public class NpcC778C149C158 : PackNpc
    {
        public NpcC778C149C158(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_name2_s = 0;
            V v_name3_s = 0;

            v_myid = p.Call("get_myid");
            yield return Mes((V)1L, (V)"당신이 존재해야할 공간으로 보내드리겠습니다.");
            if (V.T(((V)(p["#realmap"]) == (V)((V)2L))))
            {
                v_name2_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2"));
                v_name3_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장3"));
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name2_s, (V)"튜토리얼의장2", (V)22L, (V)20L, (V)140L, (V)1L, (V)2L, (V)"db/maps/던전/lod002.map");
                p.Call("npc_spawn", (V)"초보자도우미2", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)5L, (V)4L, (V)2L);
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name3_s, (V)"튜토리얼의장3", (V)22L, (V)25L, (V)140L, (V)1L, (V)3L, (V)"db/maps/던전/lod004.map");
                p.Call("warp", v_name2_s, (V)3L, (V)5L);
                p.Call("set_hair", p["#hair"]);
                p.Call("set_sex", p["#sex"]);
                yield break;
            }
            if (V.T(((V)(p["#realmap"]) == (V)((V)3L))))
            {
                v_name3_s = ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장3"));
                p["$map_num"] = ((V)(p["$map_num"]) + (V)((V)1L));
                p.Call("map_create", p["$map_num"], v_name3_s, (V)"튜토리얼의장3", (V)22L, (V)25L, (V)140L, (V)1L, (V)3L, (V)"db/maps/던전/lod004.map");
                p.Call("warp", v_name3_s, (V)3L, (V)19L);
                p.Call("mob_spawn3", (V)"(튜토리얼)팜팻", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장3")), (V)1L, (V)2L, (V)100L, (V)9L);
                p.Call("mob_spawn3", (V)"(튜토리얼)좀비", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장3")), (V)1L, (V)2L, (V)500L, (V)1L);
                p.Call("set_hair", p["#hair"]);
                p.Call("set_sex", p["#sex"]);
                yield break;
            }
            p.Call("warp", (V)"노비스마을", (V)37L, (V)29L);
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
