using System;
using System.Collections.Generic;

namespace RA
{
    public static class StringExtensions
    {
        public static double ToHours(this string value)
        {
            if (TimeSpan.TryParse(value, out var tsval))
            {
                return tsval.TotalHours;
            }
            else
            {
                throw new ArgumentException($"時間の形式(hh:mm)にしてください: {value}");
            }
        }
        public static bool TryToHours(this string value, out double val)
        {
            if (TimeSpan.TryParse(value, out var tsval))
            {
                val = tsval.TotalHours;
                return true;
            }
            else
            {
                val = default(double);
                return false;
            }
        }
    }
}
