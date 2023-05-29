using System;
using UnityEngine;

namespace UI.Console
{
    public static class GlobalNotificationManager
    {
        private static NotificationConsole _console;

        public static void AddMessage(string message)
        {
            if (_console is null)
            {
                Init();
                if (_console is null)
                    return;
            }

            _console.EnqueueMessage(message);
        }

        public static void AddLoadingMessage(string messageText, Guid messageID)
        {
            if (_console is null)
            {
                Init();
                if (_console is null)
                    return;
            }

            _console.EnqueueLoadingMessage(messageText, messageID);
        }

        public static void StartRemovingMessage(Guid messageID, float delay)
        {
            if (_console is null)
            {
                Init();
                if (_console is null)
                    return;
            }

            _console.EnqueueRemovingLoadingMessage(messageID, delay);
        }
        
        private static void Init()
        {
            var notifications = GameObject.Find("Notifications");

            if (notifications is not null)
                _console = notifications.GetComponent<NotificationConsole>();
        }
    }
}