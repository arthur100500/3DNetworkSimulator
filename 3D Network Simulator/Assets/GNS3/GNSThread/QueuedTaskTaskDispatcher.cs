using System;
using System.Collections.Concurrent;
using System.Threading;
using UI.Console;
using Unity.VisualScripting;

namespace GNS3.GNSThread
{
    public class QueuedTaskTaskDispatcher : IQueuedTaskDispatcher, ISingleton
    {
        private static QueuedTaskTaskDispatcher _queuedTaskTaskDispatcher;
        private static bool _running = true;
        private readonly ConcurrentQueue<Action> _actions = new();
        private bool _started;

        private Thread _thread;

        private Thread Thread
        {
            get
            {
                _thread ??= new Thread(ThreadWork);
                return _thread;
            }
        }

        public void Run()
        {
            if (!_started) Thread.Start();
            _started = true;
        }

        public void Stop()
        {
            _running = false;
            _thread.Join();
        }

        public void EnqueueAction(IQueuedTask action)
        {
            if (!_started) Run();
            _actions.Enqueue(() =>
            {
                action.Start();
                action.DoWork();
                action.Finish();
            });
        }

        public void EnqueueActionWithNotifications(IQueuedTask action, string onStart, string onEnd, float delay)
        {
            if (!_started) Run();
            _actions.Enqueue(() =>
            {
                var guid = Guid.NewGuid();
                GlobalNotificationManager.AddLoadingMessage(onStart, guid);
                action.Start();
                action.DoWork();
                action.Finish();
                GlobalNotificationManager.AddLoadingMessage(onEnd, guid);
                GlobalNotificationManager.StartRemovingMessage(guid, delay);
            });
        }

        public void EnqueueActionWithNotification(IQueuedTask action, string notification, float delay)
        {
            if (!_started) Run();
            _actions.Enqueue(() =>
            {
                var guid = Guid.NewGuid();
                GlobalNotificationManager.AddLoadingMessage("[..] " + notification, guid);
                try
                {
                    action.Start();
                    action.DoWork();
                    action.Finish();
                    GlobalNotificationManager.AddLoadingMessage("[<color=green>OK</color>] " + notification, guid);
                }
                catch (Exception ex)
                {
                    GlobalNotificationManager.AddLoadingMessage(
                        "[<color=red>FL</color>] " + notification + " due to " + ex.Message, guid);
                }
                Thread.Sleep(10);
                GlobalNotificationManager.StartRemovingMessage(guid, delay);
            });
        }
        
        public static QueuedTaskTaskDispatcher GetInstance()
        {
            return _queuedTaskTaskDispatcher ??= new QueuedTaskTaskDispatcher();
        }

        private void ThreadWork()
        {
            while (_running)
            {
                Thread.Sleep(10);
                if (!_actions.TryDequeue(out var action)) continue;
                action();
            }
        }
    }
}