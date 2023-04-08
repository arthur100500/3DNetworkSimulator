using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Console
{
    public class NotificationConsole : MonoBehaviour
    {
        private const int ShiftAmount = 30;

        [FormerlySerializedAs("MessageMaterial")]
        public TMP_FontAsset messageMaterial;

        private readonly ConcurrentQueue<(string, Guid)> _loadingMessageQueue = new();
        private readonly ConcurrentQueue<string> _messageQueue = new();
        private readonly List<Guid> _messagesToRemove = new();
        private readonly ConcurrentQueue<(Guid, float)> _removeMessageQueue = new();

        /*
        * This is a class to show messages on screen.
        * Every time new message is sent it will appear on the screen bottom.
        *
        * Current message types: standart (just text, dissapears after N seconds)
        *                        loading (can be changed)
        *
        * To send notification use GlobalNotifications
        */
        private List<ANotification> _notifications = new();

        public void FixedUpdate()
        {
            UpdateMessageQueue();
            UpdateLoadingMessageQueue();
            UpdateRemoveMessageQueue();
        }

        // Usually called from another thread!!!
        // Removes message after delay seconds
        public void EnqueueRemovingLoadingMessage(Guid messageID, float delay)
        {
            _removeMessageQueue.Enqueue((messageID, delay));
        }

        // Usually called from another thread!!!
        public void EnqueueMessage(string message)
        {
            _messageQueue.Enqueue(message);
        }

        /* 
        * Unqueues not only addition but also updates
        * Usually called from another thread!!!
        */
        public void EnqueueLoadingMessage(string messageText, Guid messageID)
        {
            _loadingMessageQueue.Enqueue((messageText, messageID));
        }

        private void AddNotification(string text)
        {
            GameObject textObj = new();
            var nt = textObj.AddComponent<NotificationText>();
            nt.message = text;
            textObj.transform.SetParent(gameObject.transform);
            nt.Configure(this);

            // Shift all messages
            _notifications = _notifications.FindAll(a => a != null);
            foreach (var message in _notifications) message.ShiftUp(ShiftAmount);

            _notifications.Add(nt);
        }

        private void UpdateLoadingMessage(string text, Guid guid)
        {
            var existingMessage = _notifications.Find(a => a.Guid == guid);
            if (existingMessage is null)
            {
                GameObject textObj = new();
                var nt = textObj.AddComponent<LoadingNotification>();
                nt.message = text;
                nt.Guid = guid;

                textObj.transform.SetParent(gameObject.transform);
                nt.Configure(this);

                // Shift all messages
                _notifications = _notifications.FindAll(a => a != null);
                foreach (var message in _notifications) message.ShiftUp(ShiftAmount);

                _notifications.Add(nt);
            }
            else
            {
                existingMessage.SetText(text);
            }
        }

        private void UpdateRemoveMessageQueue()
        {
            foreach (var item in _notifications.Where(a => _messagesToRemove.Contains(a.Guid)))
            {
                _messagesToRemove.Remove(item.Guid);
                item.DestroyAfterDelay(4);
            }

            if (!_removeMessageQueue.TryDequeue(out var tuple)) return;
            var (guidToRemove, delay) = tuple;
            var existingMessage = _notifications.Find(a => a.Guid == guidToRemove);

            if (existingMessage is null)
            {
                _messagesToRemove.Add(guidToRemove);
                return;
            }

            existingMessage.DestroyAfterDelay(delay);
        }

        private void UpdateMessageQueue()
        {
            if (!_messageQueue.TryDequeue(out var message)) return;
            AddNotification(message);
        }

        private void UpdateLoadingMessageQueue()
        {
            if (!_loadingMessageQueue.TryDequeue(out var tuple)) return;
            var (message, guid) = tuple;
            UpdateLoadingMessage(message, guid);
        }
    }
}