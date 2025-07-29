using System;
namespace Framework.Extensions
{
    public class DateTimeExtensions
    {
        public static DateTime ConvertUtcTimeToGmt(DateTime utcDateTime)
        {
            try
            {
                TimeZoneInfo gmtZone;
                
                #if UNITY_IOS
                    // iOS uses "GMT" as identifier
                    gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT");
                #elif UNITY_ANDROID
                    // Android uses "GMT" as identifier
                    gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT");
                #else
                    // Windows uses "GMT Standard Time"
                    gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                #endif

                return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, gmtZone);
            }
            catch (Exception ex)
            {
                // Fallback method if timezone conversion fails
                UnityEngine.Debug.LogWarning($"Time zone conversion failed: {ex.Message}. Using UTC offset method instead.");
                return utcDateTime.AddHours(0); // GMT is UTC+0
            }
        }
    }
}