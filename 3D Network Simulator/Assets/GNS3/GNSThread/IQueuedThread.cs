using System;

namespace GNS3.GNSThread
{
    public interface IQueuedThread
    {
        public void EnqueueAction(Action action);
        public void EnqueueActionWithNotifications(Action action, string onStart, string onEnd, float delay);
        public void EnqueueActionWithNotification(Action action, string notification, float delay);
        public void Run();
        public void Stop();
    }
}