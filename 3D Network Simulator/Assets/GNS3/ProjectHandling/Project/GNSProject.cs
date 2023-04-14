using System;
using System.Collections.Generic;
using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.ProjectHandling.Link;
using GNS3.ProjectHandling.Node;
using Interfaces.Requests;
using Newtonsoft.Json;
using UnityEngine;

namespace GNS3.ProjectHandling.Project
{
    public class GnsProject : IDisposable
    {
        public readonly GnsProjectConfig Config;
        private GnsJProject _jProject;
        private readonly IRequestTaskMaker _requests;
        private string Name { get; }
        public string ID => _jProject.project_id;
        
        public GnsProject(GnsProjectConfig config, string name, IRequestTaskMaker requests)
        {
            Name = name;
            Config = config;
            _requests = requests;
            
            EnqueueProjectCreation();
        }

        public void CreateNode<T>(string name, string type, Action<T> onCreate)
        {
            var notification = "Creating node " + name;
            var data = "{\"name\": \"" + name + "\", \"node_type\": \"" + type + "\", \"compute_id\": \"local\"}";
            
            string GetUrl()
            {
                var url = "projects/" + _jProject.project_id + "/nodes";
                return url;
            }

            var nodeCreationTask = _requests.MakePostRequest(GetUrl, () => data, () => { }, onCreate);
            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(nodeCreationTask, notification, 4);
        }

        private (string, string) _tempRequest;
        private void EnqueueProjectCreation()
        {
            var notification = "Creating project " + Name;

            var getProjectsTask = _requests.MakeGetRequest<List<GnsJProject>>(
                "projects",
                () => { },
                (projectList) =>
                {
                    var foundProject = projectList.Find(p => p.name == Name);
                    _tempRequest = foundProject is null ? ("projects", "{\"name\": \"" + Name + "\"}") : ("projects/" + foundProject.project_id + "/open", "{}");
                }
                );
            
            var createProjectTask = _requests.MakePostRequest<GnsJProject>(
                (() => _tempRequest.Item1),
                (() => _tempRequest.Item2),
                () => { },
                (project) => { _jProject = project; }
            );

            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(getProjectsTask, notification, 4);
            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(createProjectTask, notification, 4);
        }

        public void Dispose()
        {
            var task = _requests.MakeDeleteRequest("projects/" + _jProject.project_id, "{}", () => { }, () => { });
            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(task, "Removing project", 4);
        }

        public void DeleteNode(GnsNode node)
        {
            var task = _requests.MakeDeleteRequest("projects/" + _jProject.project_id + "/nodes/" + node.ID, "{}", () => { }, () => { });
            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(task, "Removing node " + node.Name, 4);
        }

        public void StartNode(GnsNode node)
        {
            var notification = "Starting node " + node.Name;

            string GetUrl()
            {
                var url = "projects/" + _jProject.project_id + "/nodes/" + node.ID + "/start";
                return url;
            }
            
            var task = _requests.MakePostRequest(GetUrl, "{}", () => { }, () => { node.IsStarted = true; });
            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(task, notification, 4);
        }

        public void StopNode(GnsNode node)
        {
            var notification = "Stopping node " + node.Name;
            var url = "projects/" + _jProject.project_id + "/nodes/" + node.ID + "/stop";
            var task = _requests.MakePostRequest(url, "{}", () => { }, () => { node.IsStarted = false;} );
            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(task, notification, 4);
        }

        public void RemoveLink(string linkID, GnsNode self, GnsNode other)
        {
            var notification = "Unlinking " + self.Name + " and " + other.Name;
            var url = "projects/" + _jProject.project_id + "/links" + linkID;
            var task = _requests.MakeDeleteRequest(url, "{}", () => { }, () => { });
            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(task, notification, 4);
        }
        
        public void AddLink(string linkJson, GnsNode self, GnsNode other, Action<GnsJLink> callback)
        {
            var notification = "Linking " + self.Name + " and " + other.Name;
            var url = "projects/" + _jProject.project_id + "/links";
            var task = _requests.MakePostRequest(url, linkJson, () => { }, callback);
            QueuedTaskCoroutineDispatcher.GetInstance().EnqueueActionWithNotification(task, notification, 4);
        }
    }
}