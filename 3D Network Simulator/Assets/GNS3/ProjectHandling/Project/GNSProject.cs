using System;
using System.Collections.Generic;
using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.ProjectHandling.Node;
using Interfaces.Factory;
using Interfaces.Requests;

namespace GNS3.ProjectHandling.Project
{
    public class GnsProject : IDisposable
    {
        public readonly GnsProjectConfig Config;
        private GnsJProject _jProject;
        private readonly IRequestMaker _requests;
        private readonly IFactory<GnsNode> _nodeFactory;
        private string Name { get; }
        public string ID => _jProject.project_id;
        
        public GnsProject(GnsProjectConfig config, string name, IRequestMaker requests)
        {
            Name = name;
            Config = config;
            _requests = requests;
            _nodeFactory = new GnsNodeFactory(this);

            EnqueueProjectCreation();
        }

        private GnsNode CreateNode(string type)
        {
            return _nodeFactory.Create();
        }
        
        private void EnqueueProjectCreation()
        {
            var notification = "Creating project " + Name;
            QueuedTaskThread.GetInstance().EnqueueActionWithNotification(InnerProjectCreate, notification, 4);
        }

        private void InnerProjectCreate()
        {
            var allProjects = _requests.MakeGetRequest<List<GnsJProject>>("projects");
            var existingProject = allProjects.Find(p => p.name == Name);

            _jProject = existingProject is null
                ? _requests.MakePostRequest<GnsJProject>("projects", "{\"name\": \"" + Name + "\"}")
                : _requests.MakePostRequest<GnsJProject>("projects/" + existingProject.project_id + "/open", "{}");
        }

        public void Dispose()
        {
            _requests.MakeDeleteRequest("projects/" + _jProject.project_id, "{}");
        }
    }
}