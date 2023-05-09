using System;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Exceptions;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger.ILogger;

namespace Requests.Tasks
{
    public class UnityWebRequestTask : AUnityRequestTask<bool>
    {
        public UnityWebRequestTask(Action start, Action finish, UnityWebRequest request, ILogger logger)
            : base(() => request, start, _ => finish(), logger, true)
        {
            
        }
    }
}