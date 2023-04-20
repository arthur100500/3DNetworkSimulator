using System;
using System.Text;
using GNS3.GNSThread;
using Interfaces.Requests;
using Requests.Tasks;
using UnityEngine.Networking;
using UnityEngine;
using ILogger = Interfaces.Logger.ILogger;

namespace Requests
{
    public class UnityWebRequestTaskMaker : IRequestTaskMaker
    {
        private readonly string _addrBegin;
        private readonly string _base64Authorization;
        private readonly ILogger _logger;
        public UnityWebRequestTaskMaker(string address, string user, string password, ILogger logger)
        {
            _addrBegin = address;
            _logger = logger;
            _base64Authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + password));
        }

        public IQueuedTask<AsyncOperation> MakeDeleteRequest(string url, string data, Action start, Action finish)
        {
            var request = UnityWebRequest.Delete(_addrBegin + url);
            SetHeaders(request);
            return new UnityWebRequestTask(start, request.SendWebRequest(), finish, request);
        }

        public IQueuedTask<AsyncOperation> MakeGetRequest(string url, Action start, Action finish)
        {
            var request = UnityWebRequest.Get(_addrBegin + url);
            SetHeaders(request);
            return new UnityWebRequestTask(start, request.SendWebRequest(), finish, request);
        }

        public IQueuedTask<AsyncOperation> MakePostRequest(string url, string data, Action start, Action finish)
        {
            var request = new UnityWebRequest(_addrBegin + url);
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            SetHeaders(request);
            return new UnityWebRequestTask(start, request.SendWebRequest(), finish, request);
        }

        public IQueuedTask<AsyncOperation> MakeDeleteRequest(Func<string> url, string data, Action start, Action finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = UnityWebRequest.Delete(_addrBegin + url());
                SetHeaders(request);
                return request;
            }

            return new UnityWebRequestLateTask(GetRequest, start, finish, _logger);
        }

        public IQueuedTask<AsyncOperation> MakeGetRequest(Func<string> url, Action start, Action finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = UnityWebRequest.Get(_addrBegin + url());
                SetHeaders(request);
                return request;
            }

            return new UnityWebRequestLateTask(GetRequest, start, finish, _logger);
        }

        public IQueuedTask<AsyncOperation> MakePostRequest(Func<string> url, string data, Action start, Action finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = new UnityWebRequest(_addrBegin + url());
                request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.method = UnityWebRequest.kHttpVerbPOST;
                SetHeaders(request);
                return request;
            }

            return new UnityWebRequestLateTask(GetRequest, start, finish, _logger);
        }

        public IQueuedTask<AsyncOperation> MakeGetRequest<T>(string url, Action start, Action<T> finish)
        {
            var request = UnityWebRequest.Get(_addrBegin + url);
            SetHeaders(request);
            return new UnityWebRequestResultedTask<T>(start, request.SendWebRequest(), finish, request, _logger);
        }

        public IQueuedTask<AsyncOperation> MakePostRequest<T>(string url, string data, Action start, Action<T> finish)
        {
            var request = new UnityWebRequest(_addrBegin + url);
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            SetHeaders(request);
            return new UnityWebRequestResultedTask<T>(start, request.SendWebRequest(), finish, request, _logger);
        }

        public IQueuedTask<AsyncOperation> MakeGetRequest<T>(string url, string data, Action start, Action<T> finish)
        {
            var request = UnityWebRequest.Get(_addrBegin + url);
            SetHeaders(request);
            return new UnityWebRequestResultedTask<T>(start, request.SendWebRequest(), finish, request, _logger);
        }

        public IQueuedTask<AsyncOperation> MakeGetRequest<T>(Func<string> url, Action start, Action<T> finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = UnityWebRequest.Get(_addrBegin + url());
                SetHeaders(request);
                return request;
            }

            return new UnityWebRequestLateResultedTask<T>(GetRequest, start, finish, _logger);
        }

        public IQueuedTask<AsyncOperation> MakePostRequest<T>(Func<string> url, Func<string> data, Action start, Action<T> finish)
        {
            UnityWebRequest GetRequest()
            {
                var readyData = data();
                var readyUrl = url();
                var request = new UnityWebRequest(_addrBegin + readyUrl);
                request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(readyData));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.method = UnityWebRequest.kHttpVerbPOST;
                SetHeaders(request);
                return request;
            }

            return new UnityWebRequestLateResultedTask<T>(GetRequest, start, finish, _logger);
        }

        public IQueuedTask<AsyncOperation> MakeGetRequest<T>(Func<string> url, string data, Action start, Action<T> finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = UnityWebRequest.Get(_addrBegin + url());
                SetHeaders(request);
                return request;
            }

            return new UnityWebRequestLateResultedTask<T>(GetRequest, start, finish, _logger);
        }

        private void SetHeaders(UnityWebRequest request)
        {
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            request.SetRequestHeader("Access-Control-Allow-Credentials", "true");
            request.SetRequestHeader("Access-Control-Allow-Headers", "Accept, X-Access-Token, X-Application-Name, X-Request-Sent-Time");
            request.SetRequestHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS, DELETE"); 
            request.SetRequestHeader("Access-Control-Allow-Origin", "*"); 
        }
    }
}