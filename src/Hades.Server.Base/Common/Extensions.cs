#region

using System;
using System.Collections.Generic;
using System.Text;

#endregion

namespace Darkages.Common
{
    public static class Extensions
    {
        private static readonly Encoding Encoding = Encoding.GetEncoding(949);

        // .NET 6 부터 Enumerable.DistinctBy 가 표준에 들어왔고 동작이 같다(키별 첫 항목 유지).
        // 같은 이름을 여기 두면 호출이 모호해져 컴파일되지 않으므로 표준 것을 쓴다.

        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;

            return value;
        }

        public static bool IsWithin(this int value, int minimum, int maximum)
        {
            return value >= minimum && value <= maximum;
        }

        public static byte[] ToByteArray(this string str)
        {
            return Encoding.GetBytes(str);
        }
    }
}