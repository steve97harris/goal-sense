using System;
using System.Collections.Generic;
using Framework.Extensions;
using Framework.Services;
using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using Unity.Notifications.iOS;
#endif

namespace Framework.Notifications
{
    public static class IosPredictionsNotificationScheduler
    {
        public static void ScheduleGameweekNotifications(List<Fixture> fixtures)
        {
#if UNITY_IOS && !UNITY_EDITOR
        if (fixtures == null) 
            return;

        // Optional: only remove previously scheduled "gw_" notifications to avoid nuking unrelated ones.
        RemovePreviouslyScheduledGameweekNotifications();

        var nowLocal = DateTime.UtcNow.ConvertUtcTimeToGmt();

        var byGameweek = fixtures.GetFirstFixturePerGameweek();

        foreach (var fixture in byGameweek)
        {
            var notifyTimeGmt = fixture.Kickoff.AddHours(-3);

            // Skip if already in the past
            if (notifyTimeGmt <= nowLocal) 
                continue;

            // Build a stable identifier so you can update/cancel later if fixtures change
            var id = $"gw_{fixture.Matchweek}";

            var notif = new iOSNotification
            {
                Identifier = id,
                Title = $"Gameweek {fixture.Matchweek} starts soon",
                Body = $"This is your predictions reminder! First kickoff is {fixture.Kickoff:h:mm tt}. Make sure to submit your predictions.",
                ShowInForeground = true,
                ForegroundPresentationOption = PresentationOption.Alert | PresentationOption.Sound,
                CategoryIdentifier = "gameweek",
                ThreadIdentifier = "gameweek",
                Trigger = new iOSNotificationCalendarTrigger
                {
                    Year = notifyTimeGmt.Year,
                    Month = notifyTimeGmt.Month,
                    Day = notifyTimeGmt.Day,
                    Hour = notifyTimeGmt.Hour,
                    Minute = notifyTimeGmt.Minute,
                    Second = notifyTimeGmt.Second,
                    Repeats = false
                }
            };

            iOSNotificationCenter.ScheduleNotification(notif);
            Debug.Log($"Scheduled GW{fixture.Matchweek} notification for {notifyTimeGmt} (local).");
        }
#endif
        }

#if UNITY_IOS && !UNITY_EDITOR
    private static void RemovePreviouslyScheduledGameweekNotifications()
    {
        var scheduled = iOSNotificationCenter.GetScheduledNotifications();
        foreach (var n in scheduled)
        {
            if (n.Identifier != null && n.Identifier.StartsWith("gw_"))
                iOSNotificationCenter.RemoveScheduledNotification(n.Identifier);
        }
    }
#endif
    }
}
