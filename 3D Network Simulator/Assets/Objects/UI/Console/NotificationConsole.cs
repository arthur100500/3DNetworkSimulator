using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

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
        private readonly ConcurrentQueue<string> messageQueue = new();
        private readonly ConcurrentQueue<(string, Guid)> loadingMessageQueue = new();
        private readonly ConcurrentQueue<(Guid, float)> removeMessageQueue = new();
        private readonly int shiftAmnt = 30;
        private readonly List<Guid> messagesToRemove = new();

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
            foreach (var item in notifications.Where(a => messagesToRemove.Contains(a.guid)))
            {
                messagesToRemove.Remove(item.guid);
                item.DestroyAfterDelay(4);
            }

            if (!removeMessageQueue.TryDequeue(out var tuple)) return;
            (var guidToRemove, var delay) = tuple;
            var existingMessage = notifications.Find(a => a.guid == guidToRemove);

            if (existingMessage is null) { messagesToRemove.Add(guidToRemove); return; }
            existingMessage.DestroyAfterDelay(delay);
        }

        private void UpdateMessageQueue()
        {
            if (!messageQueue.TryDequeue(out var message)) return;
            AddNotification(message);
        }

        private void UpdateLoadingMessageQueue()
        {
            if (!loadingMessageQueue.TryDequeue(out var tuple)) return;
            (var message, var guid) = tuple;
            UpdateLoadingMessage(message, guid);
        }
    }
}
