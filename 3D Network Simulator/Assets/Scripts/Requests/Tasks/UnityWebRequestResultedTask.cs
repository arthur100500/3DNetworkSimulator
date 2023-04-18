using System;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Exceptions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger.ILogger;
namespace Requests.Tasks
{
    public class UnityWebRequestResultedTask<T> : IQueuedTask
    {
        private readonly Action<T> _finish;
        private readonly AsyncOperation _operation;
        private readonly UnityWebRequest _request;
        private readonly Action _start;
        private readonly ILogger _logger;
        
        public UnityWebRequestResultedTask(Action start, AsyncOperation operation, Action<T> finish,
            UnityWebRequest request, ILogger logger)
        {
            _start = start;
            _finish = finish;
            _operation = operation;
            _request = request;
            Guid = Guid.NewGuid();
            _logger = logger;
            
            IsRunning = false;
        }

        public UnityWebRequestResultedTask(Action start, AsyncOperation operation, Action<T> finish,
            UnityWebRequest request, string notification, ILogger logger)
        {
            _start = start;
            _finish = finish;
            _operation = operation;
            _request = request;
            _logger = logger;

            NotificationOnStart = "[..] " + notification;
            NotificationOnSuccess = "[<color=green>OK</color>] " + notification;
            NotificationOnError = "[<color=red>FL</color>] " + notification;

            IsRunning = false;
        }

        public bool IsSuccessful => _request.isDone;


        public Guid Guid { get; }
        public bool IsRunning { get; private set; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }

        public void Start()
        {
            _logger.LogDebug( "Start: " + _request.url);
            _start.Invoke();
            IsRunning = true;
        }

        public void Finish()
        {
            _logger.LogDebug( "Finished: " + _request.url);
            var text = _request.downloadHandler.text;
            _logger.LogDebug( "Got: " + text);
            var deserialized = JsonConvert.DeserializeObject<T>(text);
            if (_request.responseCode is < 200 or >= 300 )
                throw new BadResponseException($"Got bad response({_request.responseCode}) from {_request.url}");
            _finish.Invoke(deserialized);
            IsRunning = false;
        }

        public AsyncOperation DoWork()
        {
            return _operation;
        }
    }
}