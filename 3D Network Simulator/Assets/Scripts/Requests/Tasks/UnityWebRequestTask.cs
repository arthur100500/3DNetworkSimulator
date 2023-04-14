using System;
using GNS3.GNSThread;
using UnityEngine;

namespace Requests.Tasks
{
    public class UnityWebRequestTask : IQueuedTask
    {
        public bool IsRunning { get; private set; }
        
        private readonly Action _start;
        private readonly Action _finish;
        private readonly AsyncOperation _operation;

        public UnityWebRequestTask(Action start, AsyncOperation operation, Action finish)
        {
            _start = start;
            _finish = finish;
            _operation = operation;

            IsRunning = false;
        }

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