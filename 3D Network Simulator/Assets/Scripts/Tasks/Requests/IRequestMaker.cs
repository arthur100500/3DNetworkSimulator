using System;
using Requests.Tasks;
using UnityEngine.Networking;

namespace Tasks.Requests
{
    public interface IRequestMaker
    {
        public UnityWebRequestLateResultedTask<T> CreateTask<T>(
            Func<string> urlCreate,
            Func<string> postDataCreate,
            Action start,
            Action<T, UnityWebRequest> finish,
            string method
        );
        
        public UnityWebRequestLateTask CreateTask(
            Func<string> urlCreate,
            Func<string> postDataCreate,
            Action start,
            Action<UnityWebRequest> finish,
            string method
        );
    }
}
