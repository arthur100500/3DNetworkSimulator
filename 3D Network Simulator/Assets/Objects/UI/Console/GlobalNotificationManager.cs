using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.NotificationConsole
{
    public static class GlobalNotificationManager
    {
        private static NotificationConsole console;

        public static void AddMessage(string message)
        {
            console.EnqueueMessage(message);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            console = GameObject.Find("Notifications").GetComponent<NotificationConsole>();
        }
    }
}