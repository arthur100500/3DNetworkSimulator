using GNS.ProjectHandling.Project;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Project;
using Menu.Json;
using Newtonsoft.Json;
using Tasks.Requests;
using UnityEngine.Networking;

namespace Project.ProjectSaver
{
    public class ServerProjectSaver : IProjectSaver
    {
        private readonly NsJProject _initial;
        private readonly GnsProject _project;
        private readonly IRequestMaker _requests;
        private readonly GnsProjectConfig _config;
        private readonly IQueuedTaskDispatcher _dispatcher;

        public ServerProjectSaver(
            NsJProject nsJProject,
            GnsProject gnsProject,
            IRequestMaker requests,
            GnsProjectConfig config,
            IQueuedTaskDispatcher dispatcher
        )
        {
            _initial = nsJProject;
            _project = gnsProject;
            _requests = requests;
            _config = config;
            _dispatcher = dispatcher;
        }
        
        
        public void Save(string data)
        {
            var nsProj = new NsJProject
            {
                Id = _initial.Id,
                Name = _initial.Name,
                GnsID = _project.Id,
                JsonAnnotation = data,
                OwnerId = _initial.OwnerId
            };

            var json = JsonConvert.SerializeObject(nsProj);

            var task = _requests.CreateTask(
                () => $"http://{_config.Address}:{_config.Port}/ns/update",
                () => json,
                () => { },
                _ => { },
                UnityWebRequest.kHttpVerbPOST
            );

            _dispatcher.EnqueueActionWithNotification(task, "Saving project", 4);
        }
    }
}