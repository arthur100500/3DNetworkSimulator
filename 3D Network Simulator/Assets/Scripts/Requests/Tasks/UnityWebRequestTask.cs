using System;
using GNS3.GNSThread;
using UnityEngine;

namespace Requests.Tasks
{
    public class UnityWebRequestTask : IQueuedTask
    {
        private readonly Action _finish;
        private readonly AsyncOperation _operation;

        private readonly Action _start;

        public UnityWebRequestTask(Action start, AsyncOperation operation, Action finish)
        {
            _start = start;
            _finish = finish;
            _operation = operation;
            Guid = Guid.NewGuid();

            IsRunning = false;
        }

        public UnityWebRequestTask(Action start, AsyncOperation operation, Action finish, string notification)
        {
            _start = start;
            _finish = finish;
            _operation = operation;

            NotificationOnStart = "[..] " + notification;
            NotificationOnSuccess = "[<color=green>OK</color>] " + notification;
            NotificationOnError = "[<color=red>FL</color>] " + notification;

            IsRunning = false;
        }

        public Guid Guid { get; }
        public bool IsRunning { get; private set; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }
        public bool IsSuccessful => _operation.isDone;

        public void Start()
        {
            _start.Invoke();
            IsRunning = true;
        }

        public void Finish()
        {
            _finish.Invoke();
            IsRunning = false;
        }

        public AsyncOperation DoWork()
        {
            return _operation;
        }
    }
}