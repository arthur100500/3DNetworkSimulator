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
        private readonly Action<T> _finish;
        private readonly Func<UnityWebRequest> _requestCreateFunc;
        private readonly Action _start;
        private AsyncOperation _operation;
        protected UnityWebRequest Request;
        private readonly ILogger _logger;
        private bool _noErrorsOccured;
        private readonly bool _needDeserialization;

        public Guid Guid { get; }
        public bool IsRunning { get; protected set; }
        public string NotificationOnStart { get; set; }
        public string NotificationOnSuccess { get; set; }
        public string NotificationOnError { get; set; }
        public bool IsSuccessful => Request.isDone && _noErrorsOccured;

        protected AUnityRequestTask(Func<UnityWebRequest> urlCreate, Action start, Action<T> finish, ILogger logger, bool needDeserialization)
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
            Request = _requestCreateFunc.Invoke();
            _operation = Request.SendWebRequest();
            outerStart.Invoke();
        }
        
        public void Start()
        {
            _start.Invoke();
            IsRunning = true;
            _logger.LogDebug( "Start: " + Request.url);
        }

        public virtual void Finish()
        {
            _logger.LogDebug( "Finished: " + Request.url);
            var text = Request.downloadHandler.text;
            _logger.LogDebug( "Got: " + text);

            if (Request.responseCode is < 200 or >= 300)
            {
                _noErrorsOccured = false;
                throw new BadResponseException($"Got bad response({Request.responseCode}) from {Request.url}");
            }

            if (_needDeserialization)
            {
                var deserialized = JsonConvert.DeserializeObject<T>(text);
                _finish.Invoke(deserialized);
            }
            else
                _finish.Invoke(default);
            IsRunning = false;
        }

        public AsyncOperation DoWork()
        {
            return _operation;
        }
    }
}