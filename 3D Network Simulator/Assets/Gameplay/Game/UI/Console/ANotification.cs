using System;
using UnityEngine;

namespace UI.Console
{
    public abstract class ANotification : MonoBehaviour
    {
        public string message;
        public Guid Guid;
        public abstract void ShiftUp(int amount);
        public abstract void Configure(NotificationConsole source);
        public abstract void DestroyAfterDelay(float delay);
        public abstract void SetText(string text);
    }
}