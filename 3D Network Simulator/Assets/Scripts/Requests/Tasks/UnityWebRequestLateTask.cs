using System;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Exceptions;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger.ILogger;

namespace Requests.Tasks
{
    public class UnityWebRequestLateTask : IQueuedTask<AsyncOperation>
    {
        private readonly Action _finish;
        private readonly Func<UnityWebRequest> _requestCreateFunc;
        private readonly Action _start;
        private AsyncOperation _operation;
        private UnityWebRequest _request;
        private ILogger _logger;
        private bool _noErrorsOccured;

        public UnityWebRequestLateTask(Func<UnityWebRequest> urlCreate, Action start, Action finish, ILogger logger)
        {
            _start = () => InnerStart(start);
            _requestCreateFunc = urlCreate;
            _logger = logger;
            _finish = finish;
            Guid = Guid.NewGuid();
            _noErrorsOccured = true;
            IsRunning = false;
        }

        public UnityWebRequestLateTask(Func<UnityWebRequest> urlCreate, Action start, Action finish,
            string notification, ILogger logger)
        {
            _start = () => InnerStart(start);
            _requestCreateFunc = urlCreate;
            _finish = finish;
            _logger = logger;

            NotificationOnStart = "[..] " + notification;
            NotificationOnSuccess = "[<color=green>OK</color>] " + notification;
            NotificationOnError = "[<color=red>FL</color>] " + notification;
            _noErrorsOccured = true;

            IsRunning = false;
        }

        public bool IsSuccessful => _request.isDone && _noErrorsOccured;

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
            _logger.LogDebug( "Finished: " + _request.url);
            var text = _request.downloadHandler.text;
            _logger.LogDebug( "Got: " + text);
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


        private void InnerStart(Action outerStart)
        {
            _request = _requestCreateFunc.Invoke();
            _operation = _request.SendWebRequest();
            outerStart.Invoke();
            _logger.LogDebug( "Start: " + _request.url);
        }
    }
}