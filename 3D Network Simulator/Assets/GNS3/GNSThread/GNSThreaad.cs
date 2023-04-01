using System.Collections.Generic;
using System.Threading;
using System;

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

        private static void ThreadWork()
        {
            while (true)
            {
                Thread.Sleep(300);
                if (actions.Count == 0) continue;
                actions.Dequeue()();
            }
        }
    }
}