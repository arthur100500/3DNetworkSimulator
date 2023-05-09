using System;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Exceptions;
using UnityEngine;
using UnityEngine.Networking;

namespace Requests.Tasks
{
    public class UnityWebRequestTask : IQueuedTask<AsyncOperation>
    {
        private readonly Action _finish;
        private readonly AsyncOperation _operation;

        private readonly Action _start;
        private readonly UnityWebRequest _request;

        private bool _noErrorsOccured;

        public UnityWebRequestTask(Action start, AsyncOperation operation, Action finish, UnityWebRequest request)
        {
            _start = start;
            _finish = finish;
            _operation = operation;
            _request = request;
            Guid = Guid.NewGuid();

            _noErrorsOccured = true;
            
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

            _noErrorsOccured = true;
            
            IsRunning = false;
        }

        public Guid Guid { get; }
        public bool IsRunning { get; private set; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }
        public bool IsSuccessful => _operation.isDone && _noErrorsOccured;

        public void Start()
        {
            _start.Invoke();
            IsRunning = true;
        }

        public void Finish()
        {
            if (_request.responseCode is < 200 or >= 300)
            {
                _noErrorsOccured = false;
                throw new BadResponseException($"Got bad response({_request.responseCode}) from {_request.url}");
            }

            _finish.Invoke();
            IsRunning = false;
        }

        public AsyncOperation DoWork()
        {
            return _operation;
        }
    }
}