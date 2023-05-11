using System;
using UnityEngine;

namespace UI.Console
{
    public static class GlobalNotificationManager
    {
        private static NotificationConsole _console;

        public static void AddMessage(string message)
        {
            _console.EnqueueMessage(message);
        }

        public static void AddLoadingMessage(string messageText, Guid messageID)
        {
            _console.EnqueueLoadingMessage(messageText, messageID);
        }

        public static void StartRemovingMessage(Guid messageID, float delay)
        {
            _console.EnqueueRemovingLoadingMessage(messageID, delay);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            _console = GameObject.Find("Notifications").GetComponent<NotificationConsole>();
        }
    }
}