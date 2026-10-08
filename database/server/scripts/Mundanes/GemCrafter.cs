#region

using System.Collections.Generic;
using System.Linq;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Storage.locales.Scripts.Pack599;
using Darkages.Types;

#endregion

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    /// <summary>
    /// 메린 — 보석으로 속성 목걸이·홀디트링을 만든다(사용자 2026-10-08 「업그레이드도 제작 npc 통해서 진행하자」, 설계 <c>autopilot/gems/SPEC.md</c>).
    /// </summary>
    /// <remarks>
    /// 레시피는 서버팩 둘에서 왔다 — 5.99 에는 없다. 크리스탈목걸이 + 보석 1개는 노바 <c>db/script/npc_script.txt</c> 메린,
    /// 홀디트링 + 보석 5개는 혼든 <c>db/script/엔피씨/엔피씨002.txt</c>(노바 유메 안내문도 5개, 노바 대본은 1개만 지운다). 가공비는 둘 다 없다.
    /// 보석은 장비 분해·99레벨 사냥터에서 얻는다(<see cref="Gems" />).
    /// </remarks>
    [Script("gem_crafter")]
    public class GemCrafter : PackNpc
    {
        private static readonly (string Base, string Gem, int Count, string Made)[] Recipes =
        {
            ("크리스탈목걸이", "루비", 1, "화염의크리스탈목걸이"),
            ("크리스탈목걸이", "진주", 1, "대지의크리스탈목걸이"),
            ("크리스탈목걸이", "에메랄드", 1, "바람의크리스탈목걸이"),
            ("크리스탈목걸이", "사파이어", 1, "바다의크리스탈목걸이"),
            ("홀디트링", "레드자스퍼", 5, "화염의홀디트링"),
            ("홀디트링", "젤리오팔", 5, "대지의홀디트링"),
            ("홀디트링", "페리도트", 5, "바람의홀디트링"),
            ("홀디트링", "꿈의바다", 5, "바다의홀디트링"),
        };

        public GemCrafter(GameServer server, Mundane mundane) : base(server, mundane)
        {
        }

        protected override IEnumerable<Prompt> Talk(Pack599.Pack599 p, Reply reply)
        {
            V me = p.Call("get_myid");

            yield return Menu("보석을 박아 속성을 입혀 드려요. 무엇을 만들까요?", Recipes.Select(r => (V) r.Made).ToArray());
            if (reply.Choice.Num < 1 || reply.Choice.Num > Recipes.Length)
                yield break;

            var recipe = Recipes[reply.Choice.Num - 1];
            yield return Menu($"{recipe.Made}에는 {recipe.Base} 1개와 {recipe.Gem} {recipe.Count}개가 들어요.", "만든다.");
            if (reply.Choice.Num != 1)
                yield break;

            if (p.Call("item_exist", me, recipe.Base).Num < 1 || p.Call("item_exist", me, recipe.Gem).Num < recipe.Count)
            {
                yield return Mes(0L, $"재료가 모자라요 — {recipe.Base} 1개와 {recipe.Gem} {recipe.Count}개가 있어야 해요.");
                yield break;
            }

            p.Call("item_del", recipe.Gem, recipe.Count);
            p.Call("item_del", recipe.Base, 1);
            p.Call("item_add", recipe.Made, 1);
            yield return Mes(0L, $"{recipe.Made}을(를) 만들었어요.");
        }
    }
}
