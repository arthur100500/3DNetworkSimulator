using System;

namespace Tasks.Task
{
    public interface IQueuedTask<out T>
    {
        public Guid Guid { get; }
        public bool IsRunning { get; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }
        public bool IsSuccessful { get; }
        public void Start();
        public void Finish();
        public T DoWork();
    }
}