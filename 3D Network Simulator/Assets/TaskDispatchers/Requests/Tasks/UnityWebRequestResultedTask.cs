using System;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger.ILogger;
namespace Requests.Tasks
{
    public class UnityWebRequestResultedTask<T> : AUnityRequestTask<T>
    {
        public UnityWebRequestResultedTask(Action start, Action<T> finish, UnityWebRequest request, ILogger logger) 
            : base(() => request, start, finish, logger, true)
        {

        }   
    }
}