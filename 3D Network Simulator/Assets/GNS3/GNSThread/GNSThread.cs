using System.Collections.Generic;
using System.Threading;
using System;
using UI.NotificationConsole;

namespace GNSThread
{
    public static class GNSThread
    {
        private static Thread thread;
        private static bool started;
        private static readonly Queue<Action> actions = new();
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

        private static void ThreadWork()
        {
            while (true)
            {
                // Thread yield??
                Thread.Sleep(300);
                if (actions.Count == 0) continue;
                actions.Dequeue()();
            }
        }
    }
}