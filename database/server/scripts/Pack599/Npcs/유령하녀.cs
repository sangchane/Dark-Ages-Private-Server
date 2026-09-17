using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 유령하녀 — 5.99 `Npc_Script.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_유령하녀", "5.99표")]
    public class NpcC720B839D558B140 : PackNpc
    {
        public NpcC720B839D558B140(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;
            V v_select2 = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            L_re: ;
            yield return Menu((V)"어서오세요, 손님. 이곳은 블러드백작의성입니다.", (V)"드라큐라백작의성 설명", (V)"블러드백작", (V)"흑요석아이템 파괴");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"드라큐라백작의성은, 동쪽, 서쪽 구간이 있습니다.");
                yield return Mes((V)1L, (V)"동, 서쪽 구간에는 각각 16개의 방이 존재하며, 가운대 다시 만나는 방이 존재합니다.(응접실2)");
                yield return Mes((V)1L, (V)"동쪽 구간은 비승급자전용 사냥터로써, 5층까지는 저층으로 성에 처음으로 오신분들에게 적절합니다.");
                yield return Mes((V)1L, (V)"6층부터 10층까지는 사냥에 숙련된 분들에게 적절한 중층입니다.");
                yield return Mes((V)1L, (V)"그리고 11층부터 16층까지는 지존(99)에게 어울리는 사냥터입니다.");
                yield return Mes((V)1L, (V)"서쪽도 마찬가지로 5, 10, 16으로 저, 중, 고 층으로 분류되며 이곳은 승급자이상분들이 이용하시기에 적절합니다.");
                yield return Mes((V)1L, (V)"이곳 백작의성에 존재하는 몬스터들은 장비 아이템을 드랍합니다. 동쪽 몬스터들은 흑요석아이템을 떨구며, 교환이 불가능하고, 서쪽 몬스터들은 별아이템을 떨구며 교환이 가능합니다.");
                yield return Mes((V)1L, (V)"이로써 블러드백작의성 설명을 끝내겠습니다.");
                goto L_re;
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)1L, (V)"블러드 백작님께서는 조용하신것을 좋아하시기에 저를 제외한 하녀 3명분하고만 성의 제일 깊은 방에서 머무십니다.");
                    yield return Mes((V)1L, (V)"블러드 백작님은 자신의 상태를 적의 상태에 비례하여 조절하는 능력이 있으십니다. 강한적이 올수록 더욱더 강해지는 셈이죠.");
                    yield return Mes((V)1L, (V)"참고로 블러드 백작님의 방에는, 승급자, 비승급자가 같이 입장할수 있으며, 그룹상태여야 합니다.");
                    yield return Mes((V)1L, (V)"더이상 블러드백작님에 대해서 해줄말이 없네요.");
                    goto L_re;
                }
                else
                    if (V.T(((V)(v_select) == (V)((V)3L))))
                    {
                        yield return Mes((V)1L, (V)"파괴하실 흑요석 아이템을 첫번째 아이템 칸에 올려놔 주세요. 참고로 파괴비는 10만골드 입니다.");
                        v_select2 = (V)0L;
                        L_re2: ;
                        yield return Menu((V)"첫번째 칸에있는 흑요석 아이템을 파괴하시겠습니까?", (V)"파괴한다.", (V)"파괴하지 않는다.");
                        v_select2 = reply.Choice;
                        if (V.T(((V)(v_select2) == (V)((V)0L))))
                        {
                            goto L_re2;
                        }
                        if (V.T(((V)(v_select2) == (V)((V)1L))))
                        {
                            if (V.T(((V)(p.Call("get_money", v_myid)) < (V)((V)100000L))))
                            {
                                yield return Mes((V)0L, (V)"골드가 부족하여 흑요석 아이템을 파괴해드릴수 없습니다.");
                                yield break;
                            }
                            if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석워리어귀걸이"))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석로그귀걸이"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석소서러귀걸이"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석프리스트귀걸이"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석몽크귀걸이"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석워리어벨트"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석로그벨트"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석소서러벨트"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석프리스트벨트"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석몽크벨트"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석워리어장갑"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석로그장갑"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석소서러장갑"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석프리스트장갑"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석몽크팔찌"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석워리어반지"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석로그반지"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석소서러반지"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석프리스트반지"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석몽크반지"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석워리어목걸이"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석로그목걸이"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석소서러목걸이"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석프리스트목걸이"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석몽크목걸이"))))))
                            {
                                if (V.T(V.B(V.T(V.B(V.T(V.B(V.T(V.B(V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석워리어각반"))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석로그각반"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석소서러각반"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석프리스트각반"))))) && V.T(((V)(p.Call("item_one_check", v_myid)) != (V)((V)"흑요석몽크각반"))))))
                                {
                                    yield return Mes((V)0L, (V)"첫번째 칸에 있는 아이템이 흑요석 아이템이 아니라서 파괴가 불가능합니다.");
                                    yield break;
                                }
                            }
                            if (V.T(((V)(p.Call("item_one_check", v_myid)) == (V)((V)0L))))
                            {
                                yield return Mes((V)0L, (V)"첫번째 칸에 있는 아이템이 흑요석 아이템이 아니라서 파괴가 불가능합니다.");
                                yield break;
                            }
                            yield return Mes((V)1L, (V)"첫번째 칸에있는 아이템이 흑요석 아이템이 맞습니다. 다음을 누르시면 파괴합니다.");
                            p.Call("item_del", p.Call("item_one_check", v_myid), (V)1L);
                            p.Call("money_del", (V)100000L);
                            yield return Mes((V)0L, (V)"성공적으로 파괴가 완료되었습니다.");
                            yield break;
                        }
                        else
                            if (V.T(((V)(v_select2) == (V)((V)2L))))
                            {
                                yield return Mes((V)0L, (V)"흑요석 아이템을 파괴하지 않습니다.");
                                yield break;
                            }
                    }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
