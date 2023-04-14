using System;
using UnityEngine;

namespace GNS3.GNSThread
{
    public interface IQueuedTask
    {
        public Guid Guid { get; }
        public bool IsRunning { get; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }
        public bool IsSuccessful { get; }
        public void Start();
        public void Finish();
        public AsyncOperation DoWork();
    }
}