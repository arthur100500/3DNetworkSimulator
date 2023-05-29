using Tasks.Task;

namespace GNS3.GNSThread
{
    public interface IQueuedTaskDispatcher
    {
        public void EnqueueAction(IQueuedTask<object> action);
        public void EnqueueActionWithNotifications(IQueuedTask<object> action, string onStart, string onEnd, float delay);
        public void EnqueueActionWithNotification(IQueuedTask<object> action, string notification, float delay);
        public void Run();
        public void Stop();
    }
}