using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

namespace UI.NotificationConsole
{
    public class NotificationConsole : MonoBehaviour
    {
        /*
        * This is a class to show messages on screen.
        * Every time new message is sent it will appear on the screen bottom.
        *
        * Current message types: standart (just text, dissapears after N seconds)
        *                        loading (can be changed)
        *
        * To send notification use GlobalNotifications
        */
        private List<ANotification> notifications = new();
        private readonly Queue<string> messageQueue = new();
        private readonly Queue<(string, Guid)> loadingMessageQueue = new();
        private readonly Queue<(Guid, float)> removeMessageQueue = new();
        private readonly int shiftAmnt = 30;

        public TMP_FontAsset MessageMaterial;

        // Usually called from another thread!!!
        // Removes message after delay seconds
        public void EnqueueRemovingLoadingMessage(Guid messageID, float delay) =>
            removeMessageQueue.Enqueue((messageID, delay));

        // Usually called from another thread!!!
        public void EnqueueMessage(string message) =>
            messageQueue.Enqueue(message);

        /* 
        * Unqueues not only addition but also updates
        * Usually called from another thread!!!
        */
        public void EnqueueLoadingMessage(string messageText, Guid messageID) =>
            loadingMessageQueue.Enqueue((messageText, messageID));

        private void AddNotification(string text)
        {
            GameObject textObj = new();
            var nt = textObj.AddComponent<NotificationText>();
            nt.Text = text;
            textObj.transform.SetParent(gameObject.transform);
            nt.Configure(this);

            // Shift all messages
            notifications = notifications.FindAll(a => a != null);
            foreach (var message in notifications)
            {
                message.ShiftUp(shiftAmnt);
            }

            notifications.Add(nt);
        }

        private void UpdateLoadingMessage(string text, Guid guid)
        {
            var existingMessage = notifications.Find(a => a.guid == guid);
            if (existingMessage is null)
            {
                GameObject textObj = new();
                var nt = textObj.AddComponent<LoadingNotification>();
                nt.Text = text;
                nt.guid = guid;
                textObj.transform.SetParent(gameObject.transform);
                nt.Configure(this);

                // Shift all messages
                notifications = notifications.FindAll(a => a != null);
                foreach (var message in notifications)
                {
                    message.ShiftUp(shiftAmnt);
                }

                notifications.Add(nt);
            }
            else
            {
                existingMessage.SetText(text);
            }
        }

        public void FixedUpdate()
        {
            UpdateMessageQueue();
            UpdateLoadingMessageQueue();
            UpdateRemoveMessageQueue();
        }

        private void UpdateRemoveMessageQueue()
        {
            if (removeMessageQueue.Count == 0) return;
            (var guidToRemove, var delay) = removeMessageQueue.Dequeue();
            var existingMessage = notifications.Find(a => a.guid == guidToRemove);
            if (existingMessage is null) return;
            existingMessage.DestroyAfterDelay(delay);
        }

        private void UpdateMessageQueue()
        {
            if (messageQueue.Count == 0) return;
            var message = messageQueue.Dequeue();
            AddNotification(message);
        }

        private void UpdateLoadingMessageQueue()
        {
            if (loadingMessageQueue.Count == 0) return;
            (var message, var guid) = loadingMessageQueue.Dequeue();
            UpdateLoadingMessage(message, guid);
        }
    }
}
