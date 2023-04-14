using System;
using GNS3.GNSThread;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Requests.Tasks
{
    public class UnityWebRequestLateTask : IQueuedTask
    {
        private readonly Action _start;
        private readonly Action _finish;
        private AsyncOperation _operation;
        private UnityWebRequest _request;
        private readonly Func<UnityWebRequest> _requestCreateFunc;

        public UnityWebRequestLateTask(Func<UnityWebRequest> urlCreate, Action start, Action finish)
        {
            _start = () => InnerStart(start);
            _requestCreateFunc = urlCreate;
            _finish = finish;

            IsRunning = false;
        }

        private void InnerStart(Action outerStart)
        {
            _request = _requestCreateFunc.Invoke();
            _operation = _request.SendWebRequest();
            outerStart.Invoke();
        }

        public bool IsRunning { get; private set; }

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