using System;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger;

namespace Requests.Tasks
{
    public class UnityWebRequestLateResultedTask<T> : AUnityRequestTask<T>
    {
        public UnityWebRequestLateResultedTask(Func<UnityWebRequest> urlCreate, Action start, Action<T> finish, ILogger.ILogger logger) : base(urlCreate, start, finish, logger, true)
        {
            
        }
    }
}