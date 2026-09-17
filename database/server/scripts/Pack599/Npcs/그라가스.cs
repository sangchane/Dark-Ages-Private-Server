using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 그라가스 — 5.99 `Npc_Quest.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_그라가스", "5.99표")]
    public class NpcADF8B77CAC00C2A4 : PackNpc
    {
        public NpcADF8B77CAC00C2A4(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_myid = 0;
            V v_select = 0;

            v_myid = p.Call("get_myid");
            v_select = (V)0L;
            if (V.T(((V)(p["#gragas"]) == (V)((V)1L))))
            {
                goto L_go;
            }
            if (V.T(((V)(p["#gragas"]) == (V)((V)2L))))
            {
                yield return Mes((V)0L, (V)"어쩐일이야?? 참고로, 내비법은 비밀이다!");
                yield break;
            }
            yield return Mes((V)1L, (V)"아.. 새로운 술 제조법이없을까!?");
            yield return Mes((V)1L, (V)"움.... 그래! 동굴지네의 껍질과 동굴쥐의 꼬리로 담근 술! 좋을듯..ㅋㅋ");
            yield return Mes((V)1L, (V)"앗! 너는 뭐냐 왜 남의집에 들어와서 내비법을 엿듣는거지!?");
            yield return Mes((V)1L, (V)"어쩔수 없다.. 살인멸구를.... 아, 그보다 더 좋은방법이 있지!");
            yield return Mes((V)1L, (V)"너는 내비법을 엿들었으니깐 이미 나랑 공모자야! 그니깐 니가 재료를 구해와야한다는거지");
            L_re: ;
            yield return Menu((V)"자 가서 동굴지네의 껍질 3개와 동굴쥐의 꼬리 3개를 가져와.", (V)"그래. 다녀올게", (V)"꺼져 돼지야ㅗㅗ");
            v_select = reply.Choice;
            if (V.T(((V)(v_select) == (V)((V)0L))))
            {
                goto L_re;
            }
            if (V.T(((V)(v_select) == (V)((V)1L))))
            {
                yield return Mes((V)1L, (V)"그래. 현명한 생각이야, 만약 니가 제대로 구해만 준다면 이 쓸모없는 귀걸이를 줄게.");
                p["#gragas"] = (V)1L;
                yield return Mes((V)0L, (V)"이름은 별의귀걸이라는대 나는 쓸모가 없어.. 누가그러던대 이거 끼면고 사냥하면 경험치 더준대 쩌네..\\n여튼 가따와.");
                yield break;
            }
            else
                if (V.T(((V)(v_select) == (V)((V)2L))))
                {
                    yield return Mes((V)0L, (V)"ㅡㅡ 도둑넘 당장 저리가!");
                    yield break;
                }
            L_go: ;
            yield return Mes((V)1L, (V)"어때 제대로 구해왔어?? 보여줘봐");
            if (V.T(V.B(V.T(((V)(p.Call("item_exist", v_myid, (V)"동굴지네의껍질")) < (V)((V)3L))) || V.T(((V)(p.Call("item_exist", v_myid, (V)"동굴쥐의꼬리")) < (V)((V)3L))))))
            {
                yield return Mes((V)0L, (V)"아직 동굴지네의껍질, 동굴쥐의 꼬리 3개씩 안가져 왔구나? 언능 가져와 현기증날것같단 말야.");
                yield break;
            }
            yield return Mes((V)1L, (V)"오! 그래 가져와줬구나. 자 이 별의귀걸이랑 교환하자..");
            p.Call("item_add", (V)"별의귀걸이", (V)1L);
            p.Call("item_del", (V)"동굴지네의껍질", (V)3L);
            p.Call("item_del", (V)"동굴쥐의꼬리", (V)3L);
            p["#gragas"] = (V)2L;
            yield return Mes((V)1L, (V)"자 교환이 완료됬어. 가져가!");
            p.Call("legend_add", (V)2L, (V)249L, (V)"블랙팜의서, 그라가스와의 비밀 동업을 성공적으로 완수 하였다.");
            yield return Mes((V)0L, (V)"참고로 내가 세상에서 제일로 잘생겼다? 몰랐찌?ㅋㅋ");
            yield break;
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
