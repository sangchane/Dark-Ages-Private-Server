#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

#endregion

namespace Darkages.Types
{
    /// <summary>
    /// 퀘스트 재료 — 괴물 드랍 목록과 따로 굴린다(목록에 넣으면 칸수로 나뉘어 원래 물건 확률이 옅어진다). 첫 쓰임은 에리얼 퀘스트의
    /// 블루하콘무기·레드하콘다리(사용자 2026-10-11, <c>autopilot/aerial-quest/SPEC.md</c>). 표는 생성기
    /// <c>scripts/gen/world/build-aerial-quest.py</c>·<c>build-king-arcfus-quest.py</c> 가 나눠 쓰는 <c>static/quest-drops.tsv</c>
    /// (괴물 이름 · 아이템 · 확률 · 있으면 잡은 사람이 가져야 할 물건 — 킹아크퍼스 퀘스트의 문어다리를 든 사람만 문어의원한).
    /// </summary>
    public static class QuestDrops
    {
        private static readonly Lazy<Dictionary<string, (string Item, double Chance, string Needs)>> Table =
            new Lazy<Dictionary<string, (string, double, string)>>(() => Read(Path.Combine(ServerContext.StoragePath, "static", "quest-drops.tsv")));

        /// <summary>이 괴물이 이번에 떨굴 재료 — 없으면 null. <paramref name="holds" /> 는 잡은 사람이 그 물건을 가졌는지.</summary>
        public static string Roll(string monster, double roll, Func<string, bool> holds) =>
            monster != null && Table.Value.TryGetValue(monster, out var drop) && roll < drop.Chance
            && (drop.Needs == null || holds(drop.Needs)) ? drop.Item : null;

        /// <summary>「괴물&lt;탭&gt;아이템&lt;탭&gt;확률[&lt;탭&gt;필요한 물건]」 줄들. 파일이 없으면 빈 표 — 재료만 멈추고 서버는 뜬다.</summary>
        public static Dictionary<string, (string Item, double Chance, string Needs)> Read(string path)
        {
            var table = new Dictionary<string, (string, double, string)>();
            if (!File.Exists(path))
                return table;
            foreach (var cells in File.ReadLines(path).Select(line => line.Split('\t')))
                if (cells.Length is 3 or 4 && double.TryParse(cells[2], NumberStyles.Float, CultureInfo.InvariantCulture, out var chance))
                    table[cells[0]] = (cells[1], chance, cells.Length == 4 ? cells[3] : null);
            return table;
        }
    }
}
