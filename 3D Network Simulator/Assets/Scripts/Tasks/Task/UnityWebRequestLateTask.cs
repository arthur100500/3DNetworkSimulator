using System;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Exceptions;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger.ILogger;

namespace Requests.Tasks
{
    public class UnityWebRequestLateTask : AUnityRequestTask<bool>
    {
        public UnityWebRequestLateTask(
            Func<UnityWebRequest> urlCreate,
            Action start,
            Action<UnityWebRequest> finish,
            ILogger logger
        ) : base(
            urlCreate, 
            start, 
            (_, request) => { finish(request); },
            logger, 
            false)
        {
            
        }
    }
}