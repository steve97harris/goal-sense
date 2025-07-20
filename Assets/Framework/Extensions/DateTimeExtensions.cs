using System;
namespace Framework.Extensions
{
    public class DateTimeExtensions
    {
        public static DateTime ConvertUtcTimeToGmt(DateTime utcDate)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcDate,
                TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time"));
        }
    }
}