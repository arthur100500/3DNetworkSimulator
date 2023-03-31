using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI.NotificationConsole
{
    public class NotificationConsole : MonoBehaviour
    {
        private List<NotificationText> notifications = new();
        private readonly Queue<string> messageQueue = new();
        private readonly int shiftAmnt = 30;

        public void EnqueueMessage(string message)
        {
            messageQueue.Enqueue(message);
        }

        public void AddNotification(string text)
        {
            GameObject textObj = new();
            var nt = textObj.AddComponent<NotificationText>();
            nt.Text = text;
            textObj.transform.SetParent(gameObject.transform);

            // Shift all messages
            notifications = notifications.FindAll(a => a != null);
            foreach (var message in notifications)
            {
                message.ShiftUp(shiftAmnt);
            }

            notifications.Add(nt);
        }

        public void FixedUpdate()
        {
            if (messageQueue.Count == 0) return;
            var message = messageQueue.Dequeue();
            AddNotification(message);
        }
    }
}
