#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Darkages.Common;
using Darkages.Network.Game;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 보석 — 가방의 장비를 분해해 얻고(앱 0xF1 10), 99레벨 사냥터 괴물도 중급 보석을 떨군다. 사용자 2026-10-08 「분해로 나올 보석은
    /// 아이템 레벨이 99이상인 경우에 고가 4종도 낮은 확률로 … 중급은 아이템 레벨에 따라 확률이 상승 … 중급보석은 현재 99레벨 던전에서만」.
    /// 보석으로 만드는 것은 제작 NPC 메린(<c>scripts/Mundanes/GemCrafter.cs</c>). 설계 <c>autopilot/gems/SPEC.md</c>.
    /// </summary>
    public static class Gems
    {
        /// <summary>중급 — 크리스탈목걸이 재료(노바 메린).</summary>
        public static readonly string[] Middle = { "루비", "사파이어", "에메랄드", "진주" };

        /// <summary>고가 — 홀디트링 재료(혼든·노바 유메). 사냥터에서는 아직 안 나온다(승급 던전 때).</summary>
        public static readonly string[] Precious = { "페리도트", "레드자스퍼", "젤리오팔", "꿈의바다" };

        /// <summary>이 레벨 이상 장비를 분해하면 고가도 <see cref="PreciousChance" /> 로 나온다.</summary>
        public const int PreciousLevel = 99;

        public const double PreciousChance = 0.05;

        /// <summary>중급 확률 = 장비 레벨 × 이 값 — 11 5.5% · 41 20.5% · 71 35.5% · 99 49.5%.</summary>
        public const double MiddlePerLevel = 0.005;

        /// <summary>99레벨 사냥터에서 한 마리가 중급 보석 하나를 떨굴 확률.</summary>
        public const double GroundChance = 0.005;

        // 생성기 scripts/gen/items/build-gem-grounds.py 가 만든 99레벨 사냥터 — 서버를 켠 뒤 처음 물을 때 한 번 읽는다.
        private static readonly Lazy<HashSet<int>> Grounds =
            new Lazy<HashSet<int>>(() => Read(Path.Combine(ServerContext.StoragePath, "static", "gem-grounds.tsv")));

        // 상점이 파는 물건 — 사서 분해하면 금화로 보석을 사는 셈이다(리뷰 2026-10-08: 99레벨 금장갑이 4,000골드). 분해는 드랍한 장비만.
        private static readonly Lazy<HashSet<string>> Sold = new Lazy<HashSet<string>>(() =>
            ServerContext.GlobalMundaneTemplateCache.Values.Where(npc => npc?.DefaultMerchantStock != null)
                .SelectMany(npc => npc.DefaultMerchantStock).ToHashSet());

        /// <summary>
        /// 분해해서 나올 보석 이름, 없으면 null. <paramref name="roll" /> 은 [0,1) 한 번 — 고가 몫과 중급 몫을 겹치지 않게 잇는다.
        /// <paramref name="pick" /> 은 넷 중 어느 것(0~3).
        /// </summary>
        public static string Roll(int level, double roll, int pick)
        {
            level = Math.Clamp(level, 0, PreciousLevel);
            var precious = level >= PreciousLevel ? PreciousChance : 0;

            if (roll < precious)
                return Precious[pick];

            return roll < precious + level * MiddlePerLevel ? Middle[pick] : null;
        }

        /// <summary>99레벨 사냥터(<paramref name="map" />)에서 잡은 괴물이 떨굴 중급 보석, 없으면 null.</summary>
        public static string FromGround(int map, double roll, int pick) =>
            roll < GroundChance && Grounds.Value.Contains(map) ? Middle[pick] : null;

        /// <summary>「맵번호&lt;탭&gt;이름」 줄들. 파일이 없으면 빈 목록 — 사냥터 보석만 멈추고 서버는 뜬다. 숫자가 아닌 줄은 건너뛴다.</summary>
        public static HashSet<int> Read(string path) =>
            File.Exists(path)
                ? File.ReadLines(path).Select(line => int.TryParse(line.Split('\t')[0], out var map) ? map : 0)
                    .Where(map => map > 0).ToHashSet()
                : new HashSet<int>();

        /// <summary>
        /// 가방 <paramref name="slot" /> 의 장비를 없애고 <see cref="Roll" /> 대로 보석을 준다. 장비만(퀘스트 물건·상점이 파는 것은 빼고) —
        /// 앱도 장비만 단추를 보이지만 서버가 다시 본다. 장비는 겹치지 않아 빈 칸이 하나 생기므로 보석은 늘 들어간다.
        /// </summary>
        public static void Disassemble(Aisling aisling, byte slot)
        {
            using var mutation = ActivitySession.BeginMutation(aisling?.Client?.Activity, "Gems.Disassemble");
            var item = aisling?.Inventory?.FindInSlot(slot);
            if (item?.Template == null)
                return;

            if (!item.Template.Flags.HasFlag(ItemFlags.Equipable) || item.Template.Flags.HasFlag(ItemFlags.QuestRelated))
            {
                aisling.Client.SendMessage(0x02, "장비만 분해할 수 있습니다.");
                return;
            }

            if (Sold.Value.Contains(item.Template.Name))
            {
                aisling.Client.SendMessage(0x02, "상점에서 파는 물건은 분해할 수 없습니다.");
                return;
            }

            var gem = Roll(item.Template.LevelRequired, Generator.Random.NextDouble(), Generator.Random.Next(Middle.Length));
            aisling.Inventory.RemoveRange(aisling.Client, item, 1);

            if (gem != null && ServerContext.GlobalItemTemplateCache.TryGetValue(gem, out var template))
            {
                aisling.Client.SendMessage(0x02, $"{item.DisplayName}을(를) 분해했습니다.");
                Item.Create(aisling, template).GiveTo(aisling, false);
            }
            else
            {
                aisling.Client.SendMessage(0x02, $"{item.DisplayName}을(를) 분해했지만 보석은 나오지 않았습니다.");
            }
        }
    }
}
