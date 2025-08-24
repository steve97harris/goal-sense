using System;
namespace Framework.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime ConvertUtcTimeToGmt(this DateTime utcDateTime)
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
        
        /// <param name="gmtDateTime">DateTime in GMT timezone</param>
        /// <returns>DateTime converted to local device timezone</returns>
        public static DateTime ConvertGmtToLocalTimeExplicit(this DateTime gmtDateTime)
        {
            try
            {
                TimeZoneInfo gmtZone;
                TimeZoneInfo localZone = TimeZoneInfo.Local;
                
#if UNITY_IOS
                    gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT");
#elif UNITY_ANDROID
                    gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT");
#else
                gmtZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
#endif

                return TimeZoneInfo.ConvertTime(gmtDateTime, gmtZone, localZone);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogWarning($"Explicit GMT to Local conversion failed: {ex.Message}. Using fallback method.");
                
                // Fallback: since GMT = UTC, just convert from UTC to local
                DateTime utcDateTime = DateTime.SpecifyKind(gmtDateTime, DateTimeKind.Utc);
                return utcDateTime.ToLocalTime();
            }
        }
    }
}