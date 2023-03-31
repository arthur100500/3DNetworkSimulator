using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UI.NotificationConsole
{
    public class NotificationConsole : MonoBehaviour
    {
        private readonly List<NotificationText> notifications = new();

        public void Start()
        {
            AddNotification("Message Notification 12 3");
        }

        public void AddNotification(string text)
        {
            GameObject textObj = new();
            var nt = textObj.AddComponent<NotificationText>();
            nt.Text = text;
            notifications.Add(nt);
            textObj.transform.SetParent(gameObject.transform);
        }
    }
}
