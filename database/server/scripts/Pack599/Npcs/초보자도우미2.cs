using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 초보자도우미2 — 5.99 `Npc_Quest.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_초보자도우미2", "5.99표")]
    public class NpcCD08BCF4C790B3C4C6B0BBF80032 : PackNpc
    {
        public NpcCD08BCF4C790B3C4C6B0BBF80032(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;

            v_myid = p.Call("get_myid");
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
            if (V.T(((V)(p["#realmap"]) == (V)((V)3L))))
            {
                p.Call("warp_create", (V)0L, p.Call("get_mapname", v_myid), (V)20L, (V)4L, ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장3")), (V)3L, (V)19L, (V)0L, (V)99L, (V)0L);
                yield return Mes((V)0L, (V)"다음 맵(20,4)으로 이동해주세요^^");
            }
            yield return Mes((V)1L, (V)"자 당신이 오기만을 기다렸어요!");
            yield return Mes((V)1L, (V)"제가 첫번째로 가르쳐 드릴것은 아이템 장착방법 입니다.");
            yield return Mes((V)1L, (V)"아이템장착하기 전에 소지품창을 여는방법을 설명해드리자면, 키보드A(ㅁ)을 눌르시면 됩니다.");
            yield return Mes((V)1L, (V)"그리고 사용을 원하는 아이템을 눌러주시면됩니다.");
            yield return Mes((V)1L, (V)"참고로 첫번째 슬롯부터 열번째 슬롯까지는 1~0키로 바로바로 사용이 가능합니다.");
            p["#real"] = (V)1L;
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))))
            {
                p.Call("item_add", (V)"에페", (V)1L);
            }
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))))
            {
                p.Call("item_add", (V)"에페", (V)1L);
            }
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))))
            {
                p.Call("item_add", (V)"에페", (V)1L);
            }
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))))
            {
                p.Call("item_add", (V)"에페", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
            {
                p.Call("item_add", (V)"레더튜닉", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
            {
                p.Call("item_add", (V)"튜닉", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
            {
                p.Call("item_add", (V)"스카웃튜닉", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
            {
                p.Call("item_add", (V)"꼬뜨", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
            {
                p.Call("item_add", (V)"후드로브", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
            {
                p.Call("item_add", (V)"매직스커트", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
            {
                p.Call("item_add", (V)"셍즈", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
            {
                p.Call("item_add", (V)"로브", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)1L))))))
            {
                p.Call("item_add", (V)"도복", (V)1L);
            }
            if (V.T(V.B(V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))) && V.T(((V)(p.Call("get_sex", v_myid)) == (V)((V)2L))))))
            {
                p.Call("item_add", (V)"연무복", (V)1L);
            }
            p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 무기와 옷을 장착하신후에 말을 걸어주세요!!");
            yield return Mes((V)0L, (V)"무기와 옷을 지급해드리겠습니다.(무도가는 무기가 지급되지 않습니다.) 장착하신후 다시 말을 걸어주세요^^");
            yield break;
            L_go1: ;
            if (V.T(V.B(V.T(((V)(p.Call("get_wearitem_weapon", v_myid)) == (V)((V)0L))) && V.T(((V)(p.Call("get_class", v_myid)) != (V)((V)5L))))))
            {
                if (V.T(((V)(p.Call("get_wearitem_armor", v_myid)) == (V)((V)0L))))
                {
                    yield return Mes((V)0L, (V)"무기와 옷을 장착하신후에 말을 걸어주세요^^");
                    yield break;
                }
                else
                {
                    yield return Mes((V)0L, (V)"무기를 장착하신후에 말을 걸어주세요^^");
                    yield break;
                }
            }
            else
            {
                if (V.T(((V)(p.Call("get_wearitem_armor", v_myid)) == (V)((V)0L))))
                {
                    yield return Mes((V)0L, (V)"옷을 장착하신후에 말을 걸어주세요^^");
                    yield break;
                }
            }
            yield return Mes((V)1L, (V)"무기를 장착해 오셧군요!! 잘하셧습니다.");
            yield return Mes((V)1L, (V)"제가 이번에 가르칠것은 기본공격과, 스킬&스펠 사용입니다!");
            yield return Mes((V)1L, (V)"기본공격은 스페이스바를 통해 할수있다는거 아시죠!?");
            yield return Mes((V)1L, (V)"자 스킬창을 여는 방법을 설명해 드리겠습니다.");
            yield return Mes((V)1L, (V)"키보드의 S(ㄴ)을 눌러 주시거나, 메뉴탭에서 두번째 주먹모양 탭을 눌러주시면 됩니다.");
            yield return Mes((V)1L, (V)"다음으로는 스펠창 여는 방법을 설명해 드리겠습니다.");
            yield return Mes((V)1L, (V)"키보드의 D(ㅇ)을 눌러 주시거나, 메뉴탭에서 세번째 불덩이모양 탭을 눌러주시면 됩니다.");
            yield return Mes((V)1L, (V)"스킬창과 스펠창의 첫번째 슬롯부터 열번째슬롯까지는 숫자키 1~0으로 사용이 가능합니다.");
            yield return Mes((V)1L, (V)"그 이외에는 직접 클릭을 통해 사용이 가능합니다.");
            yield return Mes((V)1L, (V)"(마찬가지로 아이템창은 A(ㅁ), 채팅창은 F(ㄹ), 내정보창은 G(ㅎ)를 통해 여실수 있습니다.");
            yield return Mes((V)1L, (V)"자! 그럼 팜팻 한마리를 소환해 드리겠습니다, 팜팻을 기본공격(스페이스바)를 통해 잡고 다시 대화를 걸어주세요.");
            p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 팜팻을 잡고 대화를 걸어주세요^^");
            p["#real"] = (V)2L;
            p.Call("mob_spawn", (V)"(튜토리얼)팜팻", (V)7L, (V)6L, (V)1L);
            yield break;
            L_go2: ;
            if (V.T(((V)(p.Call("map_objmob")) >= (V)((V)1L))))
            {
                p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 팜팻을 잡고 대화를 걸어주세요^^");
                yield return Mes((V)0L, (V)"팜팻을 처치하신뒤 말을 걸어주세요^^");
                yield break;
            }
            yield return Mes((V)1L, (V)"팜팻을 처치 하셧군요!! 이로써 당신은 기본공격에 도를 트셧습니다!! 축하드립니다.");
            yield return Mes((V)1L, (V)"다음은, 스킬&스펠을 사용하여 몬스터를 잡아봅시다!");
            yield return Mes((V)1L, (V)"각 직업에 맞게 기본스킬을 지급해드리겠습니다!! 그럼 팜팻 3마리를 처치하신뒤 다시 말을 걸어주세요!");
            p["#real"] = (V)3L;
            p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 팜팻을 다잡고 대화를 걸어주세요^^");
            p.Call("mob_spawn", (V)"(튜토리얼)팜팻", (V)7L, (V)6L, (V)1L);
            p.Call("mob_spawn", (V)"(튜토리얼)팜팻", (V)8L, (V)6L, (V)1L);
            p.Call("mob_spawn", (V)"(튜토리얼)팜팻", (V)7L, (V)7L, (V)1L);
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)1L))))
            {
                p.Call("skill_add", (V)"숏블레이드");
                yield return Mes((V)0L, (V)"당신에게 숏블레이드를 지급해 드렸습니다. S(ㄴ)탭에서 확인하세요!");
            }
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)2L))))
            {
                p.Call("skill_add", (V)"찌르기");
                yield return Mes((V)0L, (V)"당신에게 찌르기를 지급해 드렸습니다. S(ㄴ)탭에서 확인하세요!");
            }
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)3L))))
            {
                p.Call("spell_add", (V)"마레노");
                yield return Mes((V)0L, (V)"당신에게 마레노를 지급해 드렸습니다. D(ㅇ)탭에서 확인하세요!");
            }
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)4L))))
            {
                p.Call("spell_add", (V)"홀리볼트");
                p.Call("spell_add", (V)"쿠로");
                yield return Mes((V)0L, (V)"당신에게 홀리볼트, 쿠로를 지급해 드렸습니다. D(ㅇ)탭에서 확인하세요!");
            }
            if (V.T(((V)(p.Call("get_class", v_myid)) == (V)((V)5L))))
            {
                p.Call("skill_add", (V)"단각");
                yield return Mes((V)0L, (V)"당신에게 단각을 지급해 드렸습니다. S(ㄴ)탭에서 확인하세요!");
            }
            yield break;
            L_go3: ;
            if (V.T(((V)(p.Call("map_objmob")) >= (V)((V)1L))))
            {
                p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 팜팻을 잡고 대화를 걸어주세요^^");
                yield return Mes((V)0L, (V)"팜팻을 처치하신뒤 말을 걸어주세요^^");
                yield break;
            }
            yield return Mes((V)1L, (V)"모든 팜팻을 처치하셧군요! 스킬과 스펠을 통해 처치했을거라 믿습니다^^~\\n다음으로는 배고픔시스템에 대해 알아보도록 하겠습니다.");
            yield return Mes((V)1L, (V)"배고픔시스템은 배고픔수치에 따라 유저캐릭터에게 패널티를 부가합니다.");
            yield return Mes((V)1L, (V)"배고픔 수치를 확인하고 싶으시다면, 상태창(G)의 GP수치를 확인하시면 됩니다.\\n이수치가 50이하로 내려갈시 당신의 공격력은 1단계약해집니다.");
            yield return Mes((V)1L, (V)"배고픔 수치가 25이하로 내려가게 된다면 당신의 공격력은 2단계로 약해집니다.\\n마지막으로 당신의 배고픔수치가 0이된다면!!!");
            yield return Mes((V)1L, (V)"당신캐릭터의 체력,마력 틱이 차지 않고 120초마다 당신의 체력이 최대체력의 5%만큼 깍이게 됩니다.\\n(이때 배고픔으로 인해 깍인 체력은 1미만으로 내려가지 않습니다.)");
            yield return Mes((V)1L, (V)"자그럼, 배고픔수치에 주의하시고 다음맵으로가는 워프(20,4)를 생성하도록 하겠습니다.");
            p["#realmap"] = (V)3L;
            p.Call("mob_spawn3", (V)"(튜토리얼)팜팻", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장3")), (V)1L, (V)2L, (V)100L, (V)9L);
            p.Call("mob_spawn3", (V)"(튜토리얼)좀비", ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장3")), (V)1L, (V)2L, (V)500L, (V)1L);
            p.Call("warp_create", (V)0L, p.Call("get_mapname", v_myid), (V)20L, (V)4L, ((V)(((V)((V)"사냥터") + (V)(p.Call("get_name")))) + (V)((V)"튜토리얼의장3")), (V)3L, (V)19L, (V)0L, (V)99L, (V)0L);
            p.Call("npc_say", (V)0L, (V)0L, (V)"초보자도우미 : 안녕히 가십시오^^");
            yield return Mes((V)0L, (V)"워프(20,4)생성이 완료되었 습니다. \\n안녕히 가십시오!\\n다음맵은, 모든몬스터를 처치하시면 완료됩니다.");
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
