using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System;
using UI.NotificationConsole;

namespace GNSThread
{
    public static class GNSThread
    {
        private static Thread thread;
        private static bool started;
        private static readonly ConcurrentQueue<Action> actions = new();
        public static Thread Thread { get { thread ??= new(ThreadWork); return thread; } }

        public static void Run()
        {
            started = true;
            Thread.Start();
        }

        public static void EnqueueAction(Action action)
        {
            if (!started) Run();
            actions.Enqueue(action);
        }

        public static void EnqueueActionWithNotifications(Action action, string onStart, string onEnd, float delay)
        {
            if (!started) Run();
            actions.Enqueue(() =>
            {
                var mguid = Guid.NewGuid();
                GlobalNotificationManager.AddLoadingMessage(onStart, mguid);
                action();
                GlobalNotificationManager.AddLoadingMessage(onEnd, mguid);
                GlobalNotificationManager.StartRemovingMessage(mguid, delay);
            });
        }

        public static void EnqueueActionWithNotification(Action action, string notification, float delay)
        {
            if (!started) Run();
            actions.Enqueue(() =>
            {
                var mguid = Guid.NewGuid();
                GlobalNotificationManager.AddLoadingMessage("[....] " + notification, mguid);
                try
                {
                    action();
                    GlobalNotificationManager.AddLoadingMessage("[<color=green>DONE</color>] " + notification, mguid);
                }
                catch (Exception ex)
                {
                    GlobalNotificationManager.AddLoadingMessage("[<color=red>FAIL</color>] " + notification + " due to " + ex.Message, mguid);
                }
                // TODO: Write something working here
                Thread.Sleep(10);
                GlobalNotificationManager.StartRemovingMessage(mguid, delay);
            });
        }

        private static void ThreadWork()
        {
            while (true)
            {
                // Thread yield??
                Thread.Sleep(10);
                if (!actions.TryDequeue(out var action)) continue;
                action();
            }
        }
    }
}