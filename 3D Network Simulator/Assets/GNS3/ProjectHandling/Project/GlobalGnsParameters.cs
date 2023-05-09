using System;
using GNS3.GNSThread;
using Logger;
using Requests;
using UnityEngine;

namespace GNS3.ProjectHandling.Project
{
    public class GlobalGnsParameters : MonoBehaviour
    {
        private static int _nid = 1;
        private static GnsProject _project;
        private static IQueuedTaskDispatcher _dispatcher;
        private static GnsProjectConfig _config;

        public static int GetNextFreeID()
        {
            return _nid++;
        }

        public static GnsProject GetProject()
        {
            return _project ??= CreateProject();
        }

        private static GnsProject CreateProject()
        {
            _dispatcher = QueuedTaskCoroutineDispatcher.GetInstance();
            var logger = new VoidLogger();
            _config = GnsProjectConfig.ProxyGnsProjectConfig();
            var addrBegin = $"http://{_config.Address}:{_config.Port}/v2/";
            var requests = new UnityWebRequestTaskMaker(addrBegin, _config.User, _config.Password, logger);
            requests.Authorize(_dispatcher, _config);
            return new GnsProject(_config, "unity_project", requests, _dispatcher, logger);
        }

        public static void Cleanup()
        {
            ((IDisposable)_dispatcher).Dispose();
        }
    }
}