using GNS3.GNSThread;
using Requests;

namespace GNS3.ProjectHandling.Project
{
    /*
    * Class for containing global parameters like project, last node name id etc
    */
    internal static class GlobalGnsParameters
    {
        private static int _nid = 1;
        private static GnsProject _project;

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
            var config = GnsProjectConfig.LocalGnsProjectConfig();
            var addrBegin = "http://" + config.Address + ":" + config.Port + "/v2/";
            // var requests = new HttpRequests(addrBegin, config.User, config.Password);
            var requests = new UnityWebRequestTaskMaker(addrBegin, config.User, config.Password);
            var dispatcher = QueuedTaskCoroutineDispatcher.GetInstance();
            return new GnsProject(config, "unity_project", requests, dispatcher);
        }
    }
}