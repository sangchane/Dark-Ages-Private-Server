using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 초보자도우미1 — 5.99 `Npc_Quest.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_초보자도우미1", "5.99표")]
    public class NpcCD08BCF4C790B3C4C6B0BBF80031 : PackNpc
    {
        public NpcCD08BCF4C790B3C4C6B0BBF80031(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;
            V v_select2 = 0;

            v_myid = p.Call("get_myid");
            if (V.T(p.Call("get_class", v_myid)))
            {
                p.Call("warp_create", (V)0L, p.Call("get_mapname", v_myid), (V)20L, (V)12L, ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)3L, (V)5L, (V)0L, (V)99L, (V)0L);
                yield return Mes((V)0L, (V)"어서 다음맵으로 가보세요^^워프 좌표는(20,12)입니다.");
            }
            if (V.T(((V)(p["#real"]) == (V)((V)1L))))
            {
                goto L_go1;
            }
            if (V.T(((V)(p["#real"]) == (V)((V)2L))))
            {
                goto L_go2;
            }
            if (V.T(((V)(p["#real"]) == (V)((V)3L))))
            {
                goto L_go3;
            }
            yield return Mes((V)1L, (V)"신온라인에 오신것을 환영합니다!저는 당신의 첫시작을 도와드릴겁니다.");
            yield return Mes((V)1L, (V)"이동및 공격키는 대부분 아실테지만, 모르시는분들을 위해 설명해주자면 방향키와, 스페이스바입니다.");
            yield return Mes((V)1L, (V)"자 그럼 임무를 드리겠습니다! 제 앞쪽으로 오셔서 저에게 다시 말을 걸어주세요!");
            p.Call("message", (V)3L, (V)"{=c임무 : 도우미NPC 앞(9,12)으로 이동하여 말을 걸어보자!");
            p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 제 앞쪽으로 와주세요^^~");
            p["#real"] = (V)1L;
            yield break;
            L_go1: ;
            if (V.T(((V)(p.Call("get_xs", v_myid)) != (V)((V)10L))))
            {
                yield return Mes((V)1L, (V)"아직 제 앞으로 오시지 않았네요^^");
                yield break;
            }
            if (V.T(((V)(p.Call("get_ys", v_myid)) != (V)((V)11L))))
            {
                yield return Mes((V)1L, (V)"아직 제 앞으로 오시지 않았네요^^");
                yield break;
            }
            yield return Mes((V)1L, (V)"제 앞으로 와주셧군요! 잘하셧습니다.");
            yield return Mes((V)1L, (V)"이제는 당신의 직업을 선택할 차례입니다.직업은 총 5개가 있습니다.");
            p["#real"] = (V)2L;
            p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 저에게 다시 말을 걸어주세요^^");
            yield return Mes((V)0L, (V)"저에게 다시 말을 걸어주세요^^");
            yield break;
            L_go2: ;
            v_select = (V)0L;
            yield return Menu((V)"자 당신에게 각직업에 대해 설명을 해드릴게요^^", (V)"전사", (V)"도적", (V)"마법사", (V)"성직자", (V)"무도가", (V)"직업을 선택한다.");
            v_select = reply.Choice;
            if (V.T(V.B(V.T(V.B(!V.T(v_select))) || V.T(((V)(v_select) == (V)((V)0L))))))
            {
                goto L_go2;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"전사에 대해 설명해드리겠습니다.");
                yield return Mes((V)1L, (V)"전사는 강력한 근거리대미지를 소유하고, 높은 체력을 가지고있지만, 낮은 마력과 회복능력이 떨어집니다.");
                goto L_go2;
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"도적에 대해 설명해드리겠습니다.");
                yield return Mes((V)1L, (V)"도적은 치명적인 대미지와(크리티컬) 적절한 체력과마력, 뛰어난 회복력이 있습니다.");
                goto L_go2;
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"마법사에 대해 설명해드리겠습니다.");
                yield return Mes((V)1L, (V)"마법사는 강력한 원거리대미지를 소유하고, 높은마력을 가지고있지만, 체력이 낮습니다.");
                goto L_go2;
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"성직자에 대해 설명해드리겠습니다.");
                yield return Mes((V)1L, (V)"성직자는 뛰어난 자기&타인 회복실력과 적절한 대미지, 체력마력이 특징입니다.");
                goto L_go2;
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"무도가에 대해 설명해드리겠습니다.");
                yield return Mes((V)1L, (V)"무도가는 적절한 대미지의 원거리, 근거리 스펠, 기술과 적절한 자기회복실력이 특징입니다.");
                goto L_go2;
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                p["#real"] = (V)3L;
                p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 저에게 다시 말을 걸어주세요^^");
                yield return Mes((V)0L, (V)"자 직업을 가지실 생각이 생기셧군요! 저에게 다시 말을 걸어주시면 직업을 드리겠습니다.");
                yield break;
            }
            L_go3: ;
            v_select = (V)0L;
            v_select2 = (V)0L;
            yield return Menu((V)"어느 직업을 선택 하겠습니까?", (V)"전사", (V)"도적", (V)"마법사", (V)"성직자", (V)"무도가", (V)"직업설명을 다시 듣는다.");
            v_select = reply.Choice;
            if (V.T(V.B(V.T(V.B(!V.T(v_select))) || V.T(((V)(v_select) == (V)((V)0L))))))
            {
                goto L_go3;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Menu((V)"전사를 하시겠습니까?", (V)"전사를 선택한다.", (V)"다시 생각한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    p.Call("set_class", (V)1L);
                    yield return Mes((V)1L, (V)"당신은 전사가 되셧습니다!축하드립니다. 다음 장소로 갈수 있는 워프가 생성되었습니다!안녕히 가세요^^워프 좌표는(20,12)입니다.");
                    p.Call("legend_add", (V)0L, (V)176L, (V)"어둠의전설, 전사의 길을 걷다.");
                    p["#realmap"] = (V)2L;
                    p["#real"] = (V)0L;
                    p.Call("effect", v_myid, (V)78L, (V)0L, (V)75L);
                    p.Call("game_sound", (V)32L, (V)0L);
                    p.Call("warp_create", (V)0L, p.Call("get_mapname", v_myid), (V)20L, (V)12L, ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)3L, (V)5L, (V)0L, (V)99L, (V)0L);
                    p.Call("message", (V)3L, (V)"{=c임무 : 다음 장소(20,12)로 움직이자!");
                    p.Call("npc_spawn", (V)"초보자도우미2", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)5L, (V)4L, (V)2L);
                    p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 안녕히 가십시오^^");
                    yield break;
                }
                else
                {
                    goto L_go3;
                }
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Menu((V)"도적을 하시겠습니까?", (V)"도적을 선택한다.", (V)"다시 생각한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    p.Call("set_class", (V)2L);
                    yield return Mes((V)1L, (V)"당신은 도적이 되셧습니다!축하드립니다. 다음 장소로 갈수 있는 워프가 생성되었습니다!안녕히 가세요^^워프 좌표는(20,12)입니다.");
                    p.Call("legend_add", (V)0L, (V)176L, (V)"어둠의전설, 도적의 길을 걷다.");
                    p["#realmap"] = (V)2L;
                    p["#real"] = (V)0L;
                    p.Call("effect", v_myid, (V)78L, (V)0L, (V)75L);
                    p.Call("game_sound", (V)32L, (V)0L);
                    p.Call("warp_create", (V)0L, p.Call("get_mapname", v_myid), (V)20L, (V)12L, ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)3L, (V)5L, (V)0L, (V)99L, (V)0L);
                    p.Call("npc_spawn", (V)"초보자도우미2", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)5L, (V)4L, (V)2L);
                    p.Call("message", (V)3L, (V)"{=c임무 : 다음 장소(20,12)로 움직이자!");
                    p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 안녕히 가십시오^^");
                    yield break;
                }
                else
                {
                    goto L_go3;
                }
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Menu((V)"마법사를 하시겠습니까?", (V)"마법사를 선택한다.", (V)"다시 생각한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    p.Call("set_class", (V)3L);
                    yield return Mes((V)1L, (V)"당신은 마법사가 되셧습니다!축하드립니다. 다음 장소로 갈수 있는 워프가 생성되었습니다!안녕히 가세요^^워프 좌표는(20,12)입니다.");
                    p.Call("legend_add", (V)0L, (V)176L, (V)"어둠의전설, 마법사의 길을 걷다.");
                    p["#realmap"] = (V)2L;
                    p["#real"] = (V)0L;
                    p.Call("effect", v_myid, (V)78L, (V)0L, (V)75L);
                    p.Call("game_sound", (V)32L, (V)0L);
                    p.Call("warp_create", (V)0L, p.Call("get_mapname", v_myid), (V)20L, (V)12L, ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)3L, (V)5L, (V)0L, (V)99L, (V)0L);
                    p.Call("npc_spawn", (V)"초보자도우미2", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)5L, (V)4L, (V)2L);
                    p.Call("message", (V)3L, (V)"{=c임무 : 다음 장소(20,12)로 움직이자!");
                    p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 안녕히 가십시오^^");
                    yield break;
                }
                else
                {
                    goto L_go3;
                }
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Menu((V)"성직자를 하시겠습니까?", (V)"성직자를 선택한다.", (V)"다시 생각한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    p.Call("set_class", (V)4L);
                    yield return Mes((V)1L, (V)"당신은 성직자가 되셧습니다!축하드립니다. 다음 장소로 갈수 있는 워프가 생성되었습니다!안녕히 가세요^^워프 좌표는(20,12)입니다.");
                    p.Call("legend_add", (V)0L, (V)176L, (V)"어둠의전설, 성직자의 길을 걷다.");
                    p["#realmap"] = (V)2L;
                    p["#real"] = (V)0L;
                    p.Call("effect", v_myid, (V)78L, (V)0L, (V)75L);
                    p.Call("game_sound", (V)32L, (V)0L);
                    p.Call("warp_create", (V)0L, p.Call("get_mapname", v_myid), (V)20L, (V)12L, ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)3L, (V)5L, (V)0L, (V)99L, (V)0L);
                    p.Call("npc_spawn", (V)"초보자도우미2", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)5L, (V)4L, (V)2L);
                    p.Call("message", (V)3L, (V)"{=c임무 : 다음 장소(20,12)로 움직이자!");
                    p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 안녕히 가십시오^^");
                    yield break;
                }
                else
                {
                    goto L_go3;
                }
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Menu((V)"무도가를 하시겠습니까?", (V)"무도가를 선택한다.", (V)"다시 생각한다.");
                v_select2 = reply.Choice;
                if (V.T(((V)(v_select2) == (V)((V)1L))))
                {
                    p.Call("set_class", (V)5L);
                    yield return Mes((V)1L, (V)"당신은 무도가가 되셧습니다!축하드립니다. 다음 장소로 갈수 있는 워프가 생성되었습니다!안녕히 가세요^^워프 좌표는(20,12)입니다.");
                    p.Call("legend_add", (V)0L, (V)176L, (V)"어둠의전설, 무도가의 길을 걷다.");
                    p["#realmap"] = (V)2L;
                    p["#real"] = (V)0L;
                    p.Call("effect", v_myid, (V)78L, (V)0L, (V)75L);
                    p.Call("game_sound", (V)32L, (V)0L);
                    p.Call("warp_create", (V)0L, p.Call("get_mapname", v_myid), (V)20L, (V)12L, ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)3L, (V)5L, (V)0L, (V)99L, (V)0L);
                    p.Call("npc_spawn", (V)"초보자도우미2", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장2")), (V)5L, (V)4L, (V)2L);
                    p.Call("message", (V)3L, (V)"{=c임무 : 다음 장소(20,12)로 움직이자!");
                    p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 안녕히 가십시오^^");
                    yield break;
                }
                else
                {
                    goto L_go3;
                }
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                p["#real"] = (V)2L;
                p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 저에게 다시 말을 걸어주세요^^");
                yield return Mes((V)0L, (V)"자 다시 설명을 보실려면 저에게 다시 말을 걸어주세요^^");
                yield break;
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
