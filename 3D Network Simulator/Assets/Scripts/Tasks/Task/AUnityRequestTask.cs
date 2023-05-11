using System;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Exceptions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger.ILogger;

namespace Requests.Tasks
{
    public abstract class AUnityRequestTask<T> : IQueuedTask<AsyncOperation>
    {
        private readonly Action<T, UnityWebRequest> _finish;
        private readonly Func<UnityWebRequest> _requestCreateFunc;
        private readonly Action _start;
        private AsyncOperation _operation;
        private UnityWebRequest _request;
        private readonly ILogger _logger;
        private bool _noErrorsOccured;
        private readonly bool _needDeserialization;

        public Guid Guid { get; }
        public bool IsRunning { get; private set; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }
        public bool IsSuccessful => _request.isDone && _noErrorsOccured;

        protected AUnityRequestTask(Func<UnityWebRequest> urlCreate, Action start, Action<T, UnityWebRequest> finish, ILogger logger, bool needDeserialization)
        {
            _start = () => InnerStart(start);
            _requestCreateFunc = urlCreate;
            _finish = finish;
            _logger = logger;
            Guid = Guid.NewGuid();
            _noErrorsOccured = true;
            _needDeserialization = needDeserialization;

            IsRunning = false;
        }
        
        private void InnerStart(Action outerStart)
        {
            _request = _requestCreateFunc.Invoke();
            _operation = _request.SendWebRequest();
            outerStart.Invoke();
        }
        
        public void Start() 
        {
            _start.Invoke();
            IsRunning = true;
            _logger.LogDebug( "Start: " + _request.url);
        }

        public virtual void Finish()
        {
            _logger.LogDebug( "Finished: " + _request.url);
            var text = _request.downloadHandler.text;
            
            _logger.LogDebug( "Got: " + text);

            if (_request.responseCode is < 200 or >= 300)
            {
                _noErrorsOccured = false;
                throw new BadResponseException($"Got bad response({_request.responseCode}) from {_request.url}");
            }

            if (_needDeserialization)
            {
                var deserialized = JsonConvert.DeserializeObject<T>(text);
                _finish.Invoke(deserialized, _request);
            }
            else
                _finish.Invoke(default, _request);
            
            IsRunning = false;
        }

        public AsyncOperation DoWork()
        {
            return _operation;
        }
    }
}