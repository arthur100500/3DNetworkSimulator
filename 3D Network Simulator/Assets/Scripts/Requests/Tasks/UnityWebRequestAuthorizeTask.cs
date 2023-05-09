using System;
using GNS3.ProjectHandling.Exceptions;
using Interfaces.Logger;
using UnityEngine.Networking;

namespace Requests.Tasks
{
    public class UnityWebRequestAuthorizeTask : AUnityRequestTask<string>
    {
        private readonly Action<string> _finish;
        
        public UnityWebRequestAuthorizeTask(UnityWebRequest urlCreate, Action start, Action<string> finish, ILogger logger) 
            : base(() => urlCreate, start, finish, logger, false)
        {
            _finish = finish;
        }
        
        public override void Finish()
        {
            var headers = Request.GetResponseHeaders();

            if (headers is null || !headers.ContainsKey("Set-Cookie"))
                throw new BadResponseException("Authorization failed");
            
            var authCookie = headers["Set-Cookie"];
            
            _finish.Invoke(authCookie);
        }
    }
}