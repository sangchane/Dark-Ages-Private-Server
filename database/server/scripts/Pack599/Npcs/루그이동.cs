using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Pack599
{
    /// <summary>
    /// 루그이동 — 5.99 `Npc_Warp.txt` 의 NPC 스크립트를 그대로 옮긴 것.
    /// </summary>
    /// <remarks>
    /// 손으로 고치지 말 것. `scripts/build-pack-npcs.py` 가 다시 만든다.
    /// </remarks>
    [Script("NPC_루그이동", "5.99표")]
    public class NpcB8E8ADF8C774B3D9 : PackNpc
    {
        public NpcB8E8ADF8C774B3D9(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599 p, Reply reply)
        {
            V v_s1 = 0;
            V v_s2 = 0;
            V v_s3 = 0;

            yield return Menu((V)"오우.. 이런 누추한곳에는 어쩐일인가??", (V)"루그라는 사람을 아십니까?", (V)"아 그냥 인사차들렀습니다.");
            v_s1 = reply.Choice;
            if (V.T(((V)(v_s1) == (V)((V)2L))))
            {
                yield return Mes((V)0L, (V)"아하 .. 온김에 여기저기 둘러보시다 가시죠.");
                yield break;
            }
            yield return Mes((V)1L, (V)"헉..그분을 어떻게 알고계시죠? 아.. 그분은.. 저희마을의 장인으로써.. 일찍이 왕궁의 전속 대장장이로 일하시면서 훌륭한 무기와 아이템들을 만들어내셨지요.");
            yield return Mes((V)1L, (V)"하지만 그분의 작품을 헛되게 사용하는 사람들이 능러나면서, 그분은 은둔생활을 시작하셨습니다. 그리고 어떤 누구도 만나려고 하지 않으셨죠.");
            yield return Mes((V)1L, (V)"그분이 계시는 작업장에는 특수한 자물쇠가 걸려있어서 그것을 열기위해서는 신비한 힘을 가진 황금의만능열쇠가 필요하다고 합니다.");
            yield return Mes((V)1L, (V)"그것은 아마도 적룡굴에서 구할수 있다고 들었습니다만... 그분은 굉장히 까다로운분이라서..");
            yield return Menu((V)"그런데.. 무엇때문에 그러시죠?", (V)"루그가 만드는아이템에 관심이있어서요.. 직접만나고싶습니다.", (V)"아.. 아무것도아닙니다...");
            v_s2 = reply.Choice;
            if (V.T(((V)(v_s2) == (V)((V)2L))))
            {
                yield return Mes((V)0L, (V)"아. 그러십니까... 그러시군요..그럼 다음에도 뵙죠.");
                yield break;
            }
            yield return Menu((V)"루그님께 가고 싶으시다고요? 열쇠가 필요한것은 아시겠죠? 혹시, 열쇠를 가지고 오셨습니까?", (V)"네. 구해왔습니다.", (V)"아뇨.. 아직구하지못했습니다.");
            v_s3 = reply.Choice;
            if (V.T(((V)(v_s3) == (V)((V)1L))))
            {
                if (V.T(((V)(p.Call("pc_itemcheck", (V)"비밀의황금열쇠")) == (V)((V)0x01L))))
                {
                    yield return Mes((V)1L, (V)"가지고 오셨군요.. 자 그럼 저를 따라오세요.");
                    p.Call("item_del", (V)"비밀의황금열쇠", (V)1L);
                    p.Call("warp", (V)"루그방", (V)7L, (V)7L);
                    yield break;
                }
                else
                {
                    yield return Mes((V)0L, (V)"으음.. 열쇠가 있어야 자물쇠를 열수 있습니다. 열쇠를 구해오시면 언제든지 안내해드리겠습니다.");
                    yield break;
                }
            }
            // 말도 메뉴도 없는 스크립트(적룡의결계 …)도 이터레이터여야 한다.
            yield break;
        }
    }
}
