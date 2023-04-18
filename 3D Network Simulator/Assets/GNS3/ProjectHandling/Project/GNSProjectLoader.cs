using System;
using GNS3.GNSThread;
using Requests;

namespace GNS3.ProjectHandling.Project
{
    internal static class GlobalGnsParameters
    {
        private static int _nid = 1;
        private static GnsProject _project;
        private static IQueuedTaskDispatcher _dispatcher;
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
            var config = GnsProjectConfig.ProxyGnsProjectConfig();
            var addrBegin = $"http://{config.Address}:{config.Port}/v2/";
            var requests = new UnityWebRequestTaskMaker(addrBegin, config.User, config.Password);
            _dispatcher = QueuedTaskCoroutineDispatcher.GetInstance();
            return new GnsProject(config, "unity_project", requests, _dispatcher);
        }

        public static void Cleanup()
        {
            ((IDisposable)_dispatcher).Dispose();
        }
    }
}