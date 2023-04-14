using UnityEngine;

namespace GNS3.GNSThread
{
    public interface IQueuedTask
    {
        public bool IsRunning { get; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set;}
        public string NotificationOnError { get; set;}
        public void Start();
        public void Finish();
        public AsyncOperation DoWork();
        public bool IsSuccessful { get; }
    }
}