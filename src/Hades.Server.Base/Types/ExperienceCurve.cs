using System;

namespace Darkages.Types
{
    /// <summary>
    /// 한 레벨을 올리는 데 드는 경험치. 원작이 쓰던 곡선이다.
    /// </summary>
    /// <remarks>
    /// <para>
    /// 원작과 서버팩들은 이것을 표로 싣는다 — `db/server/experience.txt`, 0~98 레벨. 5.99 와
    /// NovaOnline 의 표가 **완전히 같고** 혼든만 다르므로, 둘이 일치하는 그 표를 원작으로 본다.
    /// </para>
    /// <para>
    /// 표를 그대로 싣지 않고 식으로 두는 이유는 서버가 이미 식을 쓰고 있어서다 — 파일을 싣고 읽는
    /// 장치를 새로 만드는 것보다 몸통을 갈아 끼우는 쪽이 작다. 차분을 내 보면 표가 거의 규칙적이라
    /// 그렇게 할 수 있다.
    /// </para>
    /// <para>
    /// <b>8~49 레벨은 등차수열이고 42 개가 오차 없이 맞는다.</b> 50 레벨부터는 등비인데 한가운데가
    /// 한 번 끊긴다 — 68→69 에서 값이 1.32 배로 튄다. 사람이 손으로 놓은 「69 레벨 벽」이라 식에서는
    /// 나오지 않고, 여기서는 그 자리에서 조각을 나누는 것으로 담는다.
    /// </para>
    /// <para>
    /// 표와의 오차: 8~49 는 0, 50~68 은 평균 1.36 % · 최대 2.94 %, 69~98 은 평균 0.35 % · 최대 0.68 %.
    /// </para>
    /// <para>
    /// 하데스가 쓰던 식(<c>레벨 × (레벨 × 0.1 + 0.5) × 5000</c>)은 이 곡선의 약 다섯 배였다. 그런데
    /// <c>Aisling.Create</c> 가 새 캐릭터에게 주는 <c>ExpNext = 600</c> 은 **원작 표의 1→2 값**이라,
    /// 1 레벨 구간만 원작이고 그 뒤가 전부 어긋나 있었다. 이 곡선으로 그 둘이 다시 맞는다.
    /// </para>
    /// </remarks>
    public static class ExperienceCurve
    {
        /// <summary>
        /// 2~7 레벨. 여섯 개뿐이고 규칙이 없어 적어 둔다. 자리 0·1 은 쓰지 않는다.
        /// </summary>
        private static readonly uint[] Early = { 0, 0, 600, 1_800, 3_000, 4_200, 5_850, 6_798 };

        /// <summary>표에서 능력치 점수 칸이 전 구간 이 값이다 — <c>StatsPerLevel</c> 이 맞는지의 근거.</summary>
        public const int PointsPerLevel = 2;

        /// <summary>
        /// <paramref name="level" /> 이 되는 데 드는 경험치. 누적이 아니라 **그 한 레벨** 의 값이다.
        /// </summary>
        public static uint ToReach(int level)
        {
            if (level <= 1)
                return 0;

            if (level < Early.Length)
                return Early[level];

            // 8~49 — 등차. 표 42 개와 오차 없이 맞는다.
            if (level <= 49)
                return (uint) (1236 * level - 1854);

            // 50~68 — 레벨마다 2.75 % 씩.
            if (level <= 68)
                return (uint) (61_000 * Math.Pow(1.02747, level - 50));

            // 69~ — 69 레벨 벽을 넘은 뒤. 레벨마다 2.48 % 씩.
            return (uint) (130_872 * Math.Pow(1.02482, level - 69));
        }
    }
}
