using System;

namespace RA
{
    public static class DateTimeExtensions
    {
        public static string FY(this DateTime dateTime)
        {
            return "FY" + (dateTime.Month < 4 ? dateTime.AddYears(-1).ToString("yy") : dateTime.ToString("yy"));
        }

        public static string ToISO8601(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
    }
    public partial class Math
    {
        public static DateTime Max(DateTime l, DateTime r) => l > r ? l : r;
        public static DateTime Min(DateTime l, DateTime r) => l < r ? l : r;
    }
}
