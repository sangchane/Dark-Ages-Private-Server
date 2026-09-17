using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 승급이아 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_승급이아", "5.99표")]
    public class NpcC2B9AE09C774C544 : PackNpc
    {
        public NpcC2B9AE09C774C544(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            yield return Mes((V)1L, (V)"더욱더 강해지기를 원하는 자여, 모든 역경과 고난을 깨뚫고 오신 자여, 그대에게 더욱더 전진할수 있는 기회를 주겠어요.");
            L_re: ;
            yield return Menu((V)"승급을 원하십니까?", (V)"승급을 원합니다.");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("get_class_sub", v_myid)) != (V)((V)0L))))
                {
                    yield return Mes((V)1L, (V)"당신은 이미 승급을 하셧습니다.");
                    yield break;
                }
                p.Call("set_class_sub", (V)1L);
                p.Call("legend_add", (V)0L, (V)176L, (V)"블랙팜의서, 승급을 하다.");
                p.Call("item_del", (V)"잉크병", (V)99L);
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)1L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)1L))))))
                {
                    p.Call("item_add", (V)"마스터아머1", (V)1L);
                    p.Call("item_add", (V)"아스카론", (V)1L);
                    p.Call("skill_add", (V)"M포효");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)1L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)2L))))))
                {
                    p.Call("item_add", (V)"마스터아머2", (V)1L);
                    p.Call("item_add", (V)"아스카론", (V)1L);
                    p.Call("skill_add", (V)"M포효");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)2L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)1L))))))
                {
                    p.Call("item_add", (V)"스카우트아머", (V)1L);
                    p.Call("item_add", (V)"아조스", (V)1L);
                    p.Call("skill_add", (V)"기습");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)2L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)2L))))))
                {
                    p.Call("item_add", (V)"레인저아머", (V)1L);
                    p.Call("item_add", (V)"아조스", (V)1L);
                    p.Call("skill_add", (V)"기습");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)3L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)1L))))))
                {
                    p.Call("item_add", (V)"다크후드", (V)1L);
                    p.Call("item_add", (V)"로오의은총", (V)1L);
                    p.Call("spell_add", (V)"메테오");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)3L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)2L))))))
                {
                    p.Call("item_add", (V)"다크로브", (V)1L);
                    p.Call("item_add", (V)"로오의은총", (V)1L);
                    p.Call("spell_add", (V)"메테오");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)4L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)1L))))))
                {
                    p.Call("item_add", (V)"세인트후드", (V)1L);
                    p.Call("item_add", (V)"이아의은총", (V)1L);
                    p.Call("spell_add", (V)"홀리쿠라노");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)4L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)2L))))))
                {
                    p.Call("item_add", (V)"세인트로브", (V)1L);
                    p.Call("item_add", (V)"이아의은총", (V)1L);
                    p.Call("spell_add", (V)"홀리쿠라노");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)5L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)1L))))))
                {
                    p.Call("item_add", (V)"무상도복", (V)1L);
                    p.Call("item_add", (V)"하이브레이질배틀핸드", (V)1L);
                    p.Call("skill_add", (V)"허공답보");
                }
                if (V.T(V.B(V.T(((V)(p.Call("get_class")) == (V)((V)5L))) && V.T(((V)(p.Call("get_sex")) == (V)((V)2L))))))
                {
                    p.Call("item_add", (V)"월영도복", (V)1L);
                    p.Call("item_add", (V)"하이브레이질배틀핸드", (V)1L);
                    p.Call("skill_add", (V)"허공답보");
                }
                p.Call("warp", (V)"신들의세계", (V)15L, (V)15L);
                p.Call("broadcast", (V)5L, ((V)(((V)((V)"[") + (V)(p.Call("get_name")))) + (V)((V)"]님이 승급하셨습니다. 다같이 축하해줍시다!")));
                p.Call("effect", v_myid, (V)90L, (V)90L, (V)100L);
                p.Call("game_sound", (V)47L, (V)0L);
                yield return Mes((V)0L, (V)"승급이 완료됬습니다. 추가로, 승급옷과, 승급무기를 지급해드렸습니다.");
                yield break;
            }
            yield break;
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
