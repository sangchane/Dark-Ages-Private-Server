using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 신의대장장이1 — 5.99 `Npc_Making.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_신의대장장이1", "5.99표")]
    public class NpcC2E0C758B300C7A5C7A5C7740031 : PackNpc
    {
        public NpcC2E0C758B300C7A5C7A5C7740031(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_select = 0;

            yield return Mes((V)1L, (V)"안녕하세요? 저는 승급옷 업그레이드 담당 NPC 블랙팜 이에요~>ㅁ<");
            yield return Mes((V)1L, (V)"우선 승급옷을 업글하실려면 생셋 or 암셋은 필수!");
            yield return Menu((V)"무엇을 만드시겠습니까?", (V)"남전사(빛)", (V)"남도적(빛)", (V)"남도가(빛)", (V)"남법사(빛)", (V)"남사제(빛)", (V)"남전사(암)", (V)"남도적(암)", (V)"남도가(암)", (V)"남법사(암)", (V)"남사제(암)", (V)"여전사(빛)", (V)"여도적(빛)", (V)"여도가(빛)", (V)"여법사(빛)", (V)"여사제(빛)", (V)"여전사(암)", (V)"여도적(암)", (V)"여도가(암)", (V)"여법사(암)", (V)"여사제(암)");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"남 전사 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"레오파드2", (V)1L);
                    p.Call("item_add", (V)"헬름2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)2L))))
            {
                yield return Mes((V)1L, (V)"남 도적 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"아비스2", (V)1L);
                    p.Call("item_add", (V)"루크2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)3L))))
            {
                yield return Mes((V)1L, (V)"남 도가 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"위타천2", (V)1L);
                    p.Call("item_add", (V)"투신의머리띠2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)4L))))
            {
                yield return Mes((V)1L, (V)"남 법사 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"마르두크2", (V)1L);
                    p.Call("item_add", (V)"솔라레스2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)5L))))
            {
                yield return Mes((V)1L, (V)"남 직자 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이 와 생명의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"데메테르2", (V)1L);
                    p.Call("item_add", (V)"아가트2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)6L))))
            {
                yield return Mes((V)1L, (V)"남 남전사 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"헬름1", (V)1L);
                    p.Call("item_add", (V)"레오파드1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)7L))))
            {
                yield return Mes((V)1L, (V)"남 도적 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"루크1", (V)1L);
                    p.Call("item_add", (V)"아비스1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)8L))))
            {
                yield return Mes((V)1L, (V)"남 도가 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"위타천1", (V)1L);
                    p.Call("item_add", (V)"투신의머리띠1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)9L))))
            {
                yield return Mes((V)1L, (V)"남 법사 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"솔라레스1", (V)1L);
                    p.Call("item_add", (V)"마르두크1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)10L))))
            {
                yield return Mes((V)1L, (V)"남 직자 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"데메테르1", (V)1L);
                    p.Call("item_add", (V)"아가트1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)11L))))
            {
                yield return Mes((V)1L, (V)"여 전사 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"살렛2", (V)1L);
                    p.Call("item_add", (V)"아레스2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)12L))))
            {
                yield return Mes((V)1L, (V)"여 도적 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"레일라2", (V)1L);
                    p.Call("item_add", (V)"라멜2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)13L))))
            {
                yield return Mes((V)1L, (V)"여  도가승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"화영잠2", (V)1L);
                    p.Call("item_add", (V)"가릉비가2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)14L))))
            {
                yield return Mes((V)1L, (V)"여 법사승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"마아트2", (V)1L);
                    p.Call("item_add", (V)"세르네티2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)15L))))
            {
                yield return Mes((V)1L, (V)"여 직자 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이 와 생명의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"생명의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"생명의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"생명의목걸이", (V)1L);
                    p.Call("item_del", (V)"생명의벨트", (V)1L);
                    p.Call("item_add", (V)"큐이레스2", (V)1L);
                    p.Call("item_add", (V)"라피스라즐리2", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)16L))))
            {
                yield return Mes((V)1L, (V)"여 여전사 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"살렛1", (V)1L);
                    p.Call("item_add", (V)"아레스1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)17L))))
            {
                yield return Mes((V)1L, (V)"여 도적 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"레일라1", (V)1L);
                    p.Call("item_add", (V)"라멜1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)18L))))
            {
                yield return Mes((V)1L, (V)"여 도가 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"가릉비가1", (V)1L);
                    p.Call("item_add", (V)"화영잠1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)19L))))
            {
                yield return Mes((V)1L, (V)"여  법사승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 생명의목걸이.생명의벨트입니다.");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_add", (V)"마아트1", (V)1L);
                    p.Call("item_add", (V)"세르네티1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            if (V.T(((V)(v_select) == (V)((V)20L))))
            {
                yield return Mes((V)1L, (V)"여 직자 승급옷 업그레이드입니다.");
                yield return Mes((V)1L, (V)"재료는 암흑의목걸이 와 암흑의벨트 입니다..");
                yield return Mes((V)1L, (V)"진행합니다.");
                if (V.T(V.B(V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의목걸이")) == (V)((V)0x01L))) && V.T(((V)(p.Call("pc_itemcheck", (V)"암흑의벨트")) == (V)((V)0x01L))))))
                {
                    yield return Mes((V)1L, (V)"제작중입니다.");
                    p.Call("item_del", (V)"암흑의목걸이", (V)1L);
                    p.Call("item_del", (V)"암흑의벨트", (V)1L);
                    p.Call("item_add", (V)"라피스라즐리1", (V)1L);
                    p.Call("item_add", (V)"큐이레스1", (V)1L);
                    yield return Mes((V)1L, (V)"##### 옷업그래이드 성공!! #####");
                }
                else
                {
                    yield return Mes((V)1L, (V)"재료가 부족하거나 제물이 아이템칸 3번째칸에 있습니다.");
                }
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
