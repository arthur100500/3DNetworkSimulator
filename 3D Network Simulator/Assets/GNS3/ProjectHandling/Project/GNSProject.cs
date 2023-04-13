using System;
using System.Collections.Generic;
using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.ProjectHandling.Link;
using Interfaces.Requests;

namespace GNS3.ProjectHandling.Project
{
    public class GnsProject : IDisposable
    {
        public readonly GnsProjectConfig Config;
        private GnsJProject _jProject;
        private readonly IRequestMaker _requests;
        private string Name { get; }
        public string ID => _jProject.project_id;
        
        public GnsProject(GnsProjectConfig config, string name, IRequestMaker requests)
        {
            Name = name;
            Config = config;
            _requests = requests;

            EnqueueProjectCreation();
        }

        public T CreateNode<T>(string name, string type)
        {
            var data = "{\"name\": \"" + name + "\", \"node_type\": \"" + type + "\", \"compute_id\": \"local\"}";
            var url = "projects/" + _jProject.project_id + "/nodes";
            return _requests.MakePostRequest<T>(url, data);
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

        public void DeleteNode(string nodeID)
        {
            _requests.MakeDeleteRequest("projects/" + _jProject.project_id + "/nodes/" + nodeID, "{}");
        }

        public void StartNode(string nodeID)
        {
            var url = "projects/" + _jProject.project_id + "/nodes/" + nodeID + "/start";
            _requests.MakePostRequest(url, "{}");
        }

        public void StopNode(string nodeID)
        {
            var url = "projects/" + _jProject.project_id + "/nodes/" + nodeID + "/stop";
            _requests.MakePostRequest(url, "{}");
        }

        public void RemoveLink(string linkID)
        {
            var url = "projects/" + _jProject.project_id + "/links/" + linkID;
            _requests.MakeDeleteRequest(url, "{}");
        }
        
        public GnsJLink AddLink(string linkJson)
        {
            var url = "projects/" + _jProject.project_id + "/links/";
            return _requests.MakePostRequest<GnsJLink>(url, linkJson);
        }
    }
}