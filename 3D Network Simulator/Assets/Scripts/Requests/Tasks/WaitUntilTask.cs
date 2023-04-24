using System;
using System.Collections;
using GNS3.GNSThread;
using UnityEngine;

namespace Requests.Tasks
{
    public class WaitUntilTask : IQueuedTask<IEnumerator>
    {
        public Guid Guid { get; }
        public bool IsRunning { get; private set; }
        public bool IsReady;
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }
        public bool IsSuccessful { get; private set; }

        public WaitUntilTask(string notification)
        {
            Guid = Guid.NewGuid();
            IsRunning = true;
            IsReady = false;
            
            NotificationOnStart = "[..] " + notification;
            NotificationOnSuccess = "[<color=green>OK</color>] " + notification;
            NotificationOnError = "[<color=red>FL</color>] " + notification;
        }
        
        public void Start() { }

        public void Finish()
        {
            IsRunning = false;
            IsSuccessful = true;
        }

        public IEnumerator DoWork()
        {
            return new WaitUntil(() => IsReady);
        }
    }
}