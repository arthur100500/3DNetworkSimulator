using System;
using System.Text;
using Interfaces.Requests;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Requests
{
    public class UnityWebRequests : IRequestMaker
    {
        private readonly string _addrBegin;
        private readonly string _base64Authorization;
        public UnityWebRequests(string address, string user, string password)
        {
            _addrBegin = address;
            _base64Authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes(user + ":" + password));
        }
        
        public void MakeGetRequest(string url)
        {
            var request = UnityWebRequest.Get(_addrBegin + url); 
            SendRequest(request);
        }

        public T MakeGetRequest<T>(string url, string data)
        {
            var request = UnityWebRequest.Get(_addrBegin + url);
            return SendRequest<T>(request);
        }

        public void MakePostRequest(string url, string data)
        {
            var request = UnityWebRequest.Post(_addrBegin + url, data);
            SendRequest(request);
        }

        public T MakeGetRequest<T>(string url)
        {
            var request = UnityWebRequest.Get(_addrBegin + url);
            return SendRequest<T>(request);
        }

        public T MakePostRequest<T>(string url, string data)
        {
            var request = UnityWebRequest.Post(_addrBegin + url, data);
            return SendRequest<T>(request);
        }

        public void MakeDeleteRequest(string url, string data)
        {
            var request = UnityWebRequest.Delete(_addrBegin + url);
            SendRequest(request);
        }

        private void SendRequest(UnityWebRequest request)
        {
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            request.SendWebRequest();
        }
        
        private T SendRequest<T>(UnityWebRequest request)
        {
            request.SetRequestHeader("Authorization", $"Basic {_base64Authorization}");
            request.SendWebRequest();
            return JsonConvert.DeserializeObject<T>(request.downloadHandler.text);
        }
    }
}