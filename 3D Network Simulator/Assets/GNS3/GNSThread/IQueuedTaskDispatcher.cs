using System;

namespace GNS3.GNSThread
{
    public interface IQueuedTaskDispatcher
    {
        public void EnqueueAction(IQueuedTask action);
        public void EnqueueActionWithNotifications(IQueuedTask action, string onStart, string onEnd, float delay);
        public void EnqueueActionWithNotification(IQueuedTask action, string notification, float delay);
        public void Run();
        public void Stop();
    }
}