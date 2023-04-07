using System;
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

        public static void AddLoadingMessage(string messageText, Guid messageID)
        {
            console.EnqueueLoadingMessage(messageText, messageID);
        }

        public static void StartRemovingMessage(Guid messageID, float delay)
        {
            console.EnqueueRemovingLoadingMessage(messageID, delay);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            console = GameObject.Find("Notifications").GetComponent<NotificationConsole>();
        }
    }
}