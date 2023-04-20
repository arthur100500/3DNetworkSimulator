using System;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Exceptions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger;

namespace Requests.Tasks
{
    public class UnityWebRequestLateResultedTask<T1> : IQueuedTask<AsyncOperation>
    {
        private readonly Action<T1> _finish;
        private readonly Func<UnityWebRequest> _requestCreateFunc;
        private readonly Action _start;
        private AsyncOperation _operation;
        private UnityWebRequest _request;
        private ILogger.ILogger _logger;

        public UnityWebRequestLateResultedTask(Func<UnityWebRequest> urlCreate, Action start, Action<T1> finish,
            ILogger.ILogger logger)
        {
            _start = () => InnerStart(start);
            _requestCreateFunc = urlCreate;
            _finish = finish;
            _logger = logger;
            Guid = Guid.NewGuid();

            IsRunning = false;
        }

        public UnityWebRequestLateResultedTask(Func<UnityWebRequest> urlCreate, Action start, Action<T1> finish,
            string notification)
        {
            _start = () => InnerStart(start);
            _requestCreateFunc = urlCreate;
            _finish = finish;

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
            _logger.LogDebug( "Start: " + _request.url);
        }

        public void Finish()
        {
            _logger.LogDebug( "Finished: " + _request.url);
            var text = _request.downloadHandler.text;
            _logger.LogDebug( "Got: " + text);
            var deserialized = JsonConvert.DeserializeObject<T1>(text);
            if (_request.responseCode is < 200 or >= 300)
                throw new BadResponseException($"Got bad response({_request.responseCode}) from {_request.url}");
            _finish.Invoke(deserialized);
            IsRunning = false;
        }

        public AsyncOperation DoWork()
        {
            return _operation;
        }

        public bool IsSuccessful => _request.isDone;

        private void InnerStart(Action outerStart)
        {
            _request = _requestCreateFunc.Invoke();
            _operation = _request.SendWebRequest();
            outerStart.Invoke();
        }
    }
}