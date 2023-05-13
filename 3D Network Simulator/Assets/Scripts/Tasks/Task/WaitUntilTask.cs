using System;
using System.Collections;
using GNS3.GNSThread;
using Tasks.Task;
using UnityEngine;

namespace Requests.Tasks
{
    public class WaitUntilTask : IQueuedTask<IEnumerator>
    {
        public Guid Guid { get; }
        public bool IsRunning { get; private set; }
        private readonly bool _isReady;
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }
        public bool IsSuccessful { get; private set; }

        public WaitUntilTask(string notification)
        {
            Guid = Guid.NewGuid();
            IsRunning = true;
            _isReady = false;
            
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
            return new WaitUntil(() => _isReady);
        }
    }
}