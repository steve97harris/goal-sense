using UnityEngine;
#if UNITY_IOS && !UNITY_EDITOR
using Unity.Notifications.iOS;
#endif

namespace Framework.Notifications
{
    public class NotificationOpenHandler : MonoBehaviour
    {
#if UNITY_IOS && !UNITY_EDITOR
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) return;
        var responded = iOSNotificationCenter.QueryLastRespondedNotification();
        if (responded != null)
        {
            Debug.Log("Opened from notification: " + responded.Notification.Identifier);
        }
    }
#endif
    }

}