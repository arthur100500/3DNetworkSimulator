using System;
using System.Text;
using Interfaces.Logger;
using Requests.Tasks;
using UnityEngine.Networking;

namespace Tasks.Requests
{
    public class WebRequestMaker : IRequestMaker
    {
        private readonly ILogger _logger;
        private string _cookies;
        
        public WebRequestMaker(ILogger logger)
        {
            _logger = logger;
            _cookies = "";
        }

        public void SetCookies(string cookies)
        {
            _cookies = cookies;
        }
        
        
        public UnityWebRequestLateResultedTask<T> CreateTask<T>(
            Func<string> urlCreate,
            Func<string> postDataCreate,
            Action start,
            Action<T, UnityWebRequest> finish,
            string method)
        {
            UnityWebRequest CreateRequest()
            {
                var data = postDataCreate();
                var url = urlCreate();

                var request = new UnityWebRequest(url);
                
                if (method == UnityWebRequest.kHttpVerbPOST)
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
                
                request.downloadHandler = new DownloadHandlerBuffer();
                request.method = method;
                
                SetHeaders(request);
                return request;
            }

            return new UnityWebRequestLateResultedTask<T>(CreateRequest, start, finish, _logger);
        }

        public UnityWebRequestLateTask CreateTask(
            Func<string> urlCreate,
            Func<string> postDataCreate,
            Action start,
            Action<UnityWebRequest> finish,
            string method)
        {
            UnityWebRequest CreateRequest()
            {
                var data = postDataCreate();
                var url = urlCreate();

                var request = new UnityWebRequest(url);
                
                if (method == UnityWebRequest.kHttpVerbPOST)
                    request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
                
                request.downloadHandler = new DownloadHandlerBuffer();
                request.method = method;
                
                SetHeaders(request);
                return request;
            }

            return new UnityWebRequestLateTask(CreateRequest, start, finish, _logger);
        }
        
        private void SetHeaders(UnityWebRequest request)
        {
            request.SetRequestHeader("Cookie", _cookies);
        }
    }
}