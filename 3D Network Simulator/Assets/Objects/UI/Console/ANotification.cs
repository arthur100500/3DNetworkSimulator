using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI.NotificationConsole
{
    public abstract class ANotification : MonoBehaviour
    {
        public abstract void ShiftUp(int amnt);
        public abstract void Configure();
        public abstract void DestroyAfterDelay(float delay);
        public abstract void SetText(string text);
        public Guid guid;
        public string Text;
    }
}
