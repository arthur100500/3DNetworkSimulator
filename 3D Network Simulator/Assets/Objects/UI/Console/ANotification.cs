using System;
using UnityEngine;

namespace UI.NotificationConsole
{
    public abstract class ANotification : MonoBehaviour
    {
        public string Text;
        public Guid guid;
        public abstract void ShiftUp(int amnt);
        public abstract void Configure(NotificationConsole source);
        public abstract void DestroyAfterDelay(float delay);
        public abstract void SetText(string text);
    }
}