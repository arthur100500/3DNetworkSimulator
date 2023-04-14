using System;
using System.Text;
using GNS3.GNSThread;
using Interfaces.Requests;
using Requests.Tasks;
using UnityEngine.Networking;

namespace Requests
{
    public class UnityWebRequestTaskMaker : IRequestTaskMaker
    {
        private readonly string _addrBegin;
        private readonly string _base64Authorization;

        public UnityWebRequestTaskMaker(string address, string user, string password)
        {
            _addrBegin = address;
            _base64Authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + password));
        }

        public IQueuedTask MakeDeleteRequest(string url, string data, Action start, Action finish)
        {
            var request = UnityWebRequest.Delete(_addrBegin + url);
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            return new UnityWebRequestTask(start, request.SendWebRequest(), finish);
        }

        public IQueuedTask MakeGetRequest(string url, Action start, Action finish)
        {
            var request = UnityWebRequest.Get(_addrBegin + url);
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            return new UnityWebRequestTask(start, request.SendWebRequest(), finish);
        }

        public IQueuedTask MakePostRequest(string url, string data, Action start, Action finish)
        {
            var request = new UnityWebRequest(_addrBegin + url);
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            return new UnityWebRequestTask(start, request.SendWebRequest(), finish);
        }

        public IQueuedTask MakeDeleteRequest(Func<string> url, string data, Action start, Action finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = UnityWebRequest.Delete(_addrBegin + url());
                request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
                return request;
            }

            return new UnityWebRequestLateTask(GetRequest, start, finish);
        }

        public IQueuedTask MakeGetRequest(Func<string> url, Action start, Action finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = UnityWebRequest.Get(_addrBegin + url());
                request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
                return request;
            }

            return new UnityWebRequestLateTask(GetRequest, start, finish);
        }

        public IQueuedTask MakePostRequest(Func<string> url, string data, Action start, Action finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = new UnityWebRequest(_addrBegin + url());
                request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
                return request;
            }

            return new UnityWebRequestLateTask(GetRequest, start, finish);
        }

        public IQueuedTask MakeGetRequest<T>(string url, Action start, Action<T> finish)
        {
            var request = UnityWebRequest.Get(_addrBegin + url);
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            return new UnityWebRequestResultedTask<T>(start, request.SendWebRequest(), finish, request);
        }

        public IQueuedTask MakePostRequest<T>(string url, string data, Action start, Action<T> finish)
        {
            var request = new UnityWebRequest(_addrBegin + url);
            request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(data));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            return new UnityWebRequestResultedTask<T>(start, request.SendWebRequest(), finish, request);
        }

        public IQueuedTask MakeGetRequest<T>(string url, string data, Action start, Action<T> finish)
        {
            var request = UnityWebRequest.Get(_addrBegin + url);
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            return new UnityWebRequestResultedTask<T>(start, request.SendWebRequest(), finish, request);
        }

        public IQueuedTask MakeGetRequest<T>(Func<string> url, Action start, Action<T> finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = UnityWebRequest.Get(_addrBegin + url());
                request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
                return request;
            }

            return new UnityWebRequestLateResultedTask<T>(GetRequest, start, finish);
        }

        public IQueuedTask MakePostRequest<T>(Func<string> url, Func<string> data, Action start, Action<T> finish)
        {
            UnityWebRequest GetRequest()
            {
                var readyData = data();
                var readyUrl = url();
                var request = new UnityWebRequest(_addrBegin + readyUrl);
                request.uploadHandler = new UploadHandlerRaw(Encoding.ASCII.GetBytes(readyData));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.method = UnityWebRequest.kHttpVerbPOST;
                //var request = UnityWebRequest.Post(_addrBegin + url(), readyData);
                request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
                return request;
            }

            return new UnityWebRequestLateResultedTask<T>(GetRequest, start, finish);
        }

        public IQueuedTask MakeGetRequest<T>(Func<string> url, string data, Action start, Action<T> finish)
        {
            UnityWebRequest GetRequest()
            {
                var request = UnityWebRequest.Get(_addrBegin + url());
                request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
                return request;
            }

            return new UnityWebRequestLateResultedTask<T>(GetRequest, start, finish);
        }
    }
}