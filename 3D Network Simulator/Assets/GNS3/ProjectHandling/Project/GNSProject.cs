using System;
using System.Collections.Generic;
using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.JsonObjects.Basic;
using GNS3.ProjectHandling.Link;
using GNS3.ProjectHandling.Node;
using Interfaces.Requests;

namespace GNS3.ProjectHandling.Project
{
    public class GnsProject : IDisposable
    {
        private readonly IRequestTaskMaker _requests;
        public readonly GnsProjectConfig Config;
        private GnsJProject _jProject;
        private readonly IQueuedTaskDispatcher _dispatcher;
        private (string, string) _tempRequest;
        private List<GnsJNode> _nodes;

        public GnsProject(GnsProjectConfig config, string name, IRequestTaskMaker requests,
            IQueuedTaskDispatcher dispatcher)
        {
            Name = name;
            Config = config;
            _requests = requests;
            _dispatcher = dispatcher;
            _nodes = new List<GnsJNode>();

            EnqueueProjectCreation();
        }

        private string Name { get; }
        public string ID => _jProject.project_id;

        public void Dispose()
        {
            var task = _requests.MakeDeleteRequest("projects/" + _jProject.project_id, "{}", () => { }, () => { });
            _dispatcher.EnqueueActionWithNotification(task, "Removing project", 4);
        }

        public void CreateNode<T>(string name, string type, Action<T> onCreate) where T : GnsJNode
        {
            var notification = "Creating node " + name;
            var data = $"{{\"name\": \"{name}\", \"node_type\": \"{type}\", \"compute_id\": \"local\"}}";

            string GetUrl()
            {
                var url = $"projects/{_jProject.project_id}/nodes";
                return url;
            }

            void AddAndOnCreate(T node)
            {
                _nodes.Add(node);
                onCreate(node);
            }

            var nodeCreationTask = _requests.MakePostRequest(GetUrl, () => data, () => { }, (Action<T>)AddAndOnCreate);
            _dispatcher.EnqueueActionWithNotification(nodeCreationTask, notification, 4);
        }

        private void EnqueueProjectCreation()
        {
            var notification = "Creating project " + Name;

            var getProjectsTask = _requests.MakeGetRequest<List<GnsJProject>>(
                "projects",
                () => { },
                projectList =>
                {
                    var foundProject = projectList.Find(p => p.name == Name);
                    _tempRequest = foundProject is null
                        ? ("projects", $"{{\"name\": \"{Name}\"}}")
                        : ($"projects/{foundProject.project_id}/open", "{}");
                }
            );

            var createProjectTask = _requests.MakePostRequest<GnsJProject>(
                () => _tempRequest.Item1,
                () => _tempRequest.Item2,
                () => { },
                project => { _jProject = project; }
            );

            _dispatcher.EnqueueActionWithNotification(getProjectsTask, notification, 4);
            _dispatcher.EnqueueActionWithNotification(createProjectTask, notification, 4);
        }

        public void DeleteNode(GnsNode node)
        {
            var task = _requests.MakeDeleteRequest($"projects/{_jProject.project_id}/nodes/{node.ID}", "{}",
                () => { }, () => { });
            _dispatcher.EnqueueActionWithNotification(task, "Removing node " + node.Name, 4);
        }

        public void StartNode(GnsNode node)
        {
            var notification = "Starting node " + node.Name;

            string GetUrl()
            {
                var url = $"projects/{_jProject.project_id}/nodes/{node.ID}/start";
                return url;
            }

            var task = _requests.MakePostRequest(GetUrl, "{}", () => { }, () => { node.IsStarted = true; });
            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        public void StopNode(GnsNode node)
        {
            var notification = "Stopping node " + node.Name;
            var url = $"projects/{_jProject.project_id}/nodes/{node.ID}/stop";
            var task = _requests.MakePostRequest(url, "{}", () => { }, () => { node.IsStarted = false; });
            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        public void RemoveLink(string linkID, GnsNode self, GnsNode other)
        {
            var notification = $"Unlinking {self.Name} and {other.Name}";
            var url = $"projects/{_jProject.project_id}/links/{linkID}";
            var task = _requests.MakeDeleteRequest(url, "{}", () => { }, () => { });
            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        public void AddLink(string linkJson, GnsNode self, GnsNode other, Action<GnsJLink> callback)
        {
            var notification = "Linking " + self.Name + " and " + other.Name;
            var url = $"projects/{_jProject.project_id}/links";
            var task = _requests.MakePostRequest(url, linkJson, () => { }, callback);
            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }
    }
}