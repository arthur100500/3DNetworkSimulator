using System;
using GNS3.GNSThread;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Requests.Tasks
{
    public class UnityWebRequestResultedTask<T> : IQueuedTask
    {
        private readonly Action _start;
        private readonly Action<T> _finish;
        private readonly AsyncOperation _operation;
        private readonly UnityWebRequest _request;
        public bool IsSuccessful => _request.isDone;

        public UnityWebRequestResultedTask(Action start, AsyncOperation operation, Action<T> finish,
            UnityWebRequest request)
        {
            _start = start;
            _finish = finish;
            _operation = operation;
            _request = request;
            Guid = Guid.NewGuid();
            
            IsRunning = false;
        }

        public UnityWebRequestResultedTask(Action start, AsyncOperation operation, Action<T> finish,
            UnityWebRequest request, string notification)
        {
            _start = start;
            _finish = finish;
            _operation = operation;
            _request = request;

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

        public void Start()
        {
            _start.Invoke();
            IsRunning = true;
        }

        public void Finish()
        {
            var deserialized = JsonConvert.DeserializeObject<T>(_request.downloadHandler.text);
            Debug.Log(_request.downloadHandler.text);
            _finish.Invoke(deserialized);
            IsRunning = false;
        }

        public AsyncOperation DoWork()
        {
            return _operation;
        }
    }
}