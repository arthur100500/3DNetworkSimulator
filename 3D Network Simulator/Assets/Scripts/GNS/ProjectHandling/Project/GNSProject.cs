using System;
using System.Collections.Generic;
using GNS.ProjectHandling.Node;
using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.JsonObjects.Basic;
using GNS3.ProjectHandling.Node;
using GNS3.ProjectHandling.Project;
using Interfaces.Logger;
using Tasks.Requests;
using UnityEngine.Networking;

namespace GNS.ProjectHandling.Project
{
    public class GnsProject
    {
        private readonly IRequestMaker _requests;
        public readonly GnsProjectConfig Config;
        private GnsJProject _jProject;
        private readonly IQueuedTaskDispatcher _dispatcher;
        private (string, string) _tempRequest;
        private readonly ILogger _logger;

        private string _serverAddress;

        public GnsProject(
            GnsProjectConfig config,
            string id,
            IRequestMaker requests,
            IQueuedTaskDispatcher dispatcher,
            ILogger logger
        )
        {
            Id = id;
            Config = config;
            _requests = requests;
            _dispatcher = dispatcher;
            _logger = logger;

            _serverAddress = $"http://{config.Address}:{config.Port}/v2/";

            EnqueueProjectCreation();
        }

        private string Name { get; }
        public string Id { get; private set; }


        public void CreateNode<T>(string name, string type, Action<T> onCreate) where T : GnsJNode
        {
            var notification = "Creating node " + name;
            var data = $"{{\"name\": \"{name}\", \"node_type\": \"{type}\", \"compute_id\": \"local\"}}";

            string GetUrl()
            {
                var url = $"{_serverAddress}projects/{_jProject.project_id}/nodes";
                return url;
            }

            void AddAndOnCreate(T node)
            {
                onCreate(node);
                _logger.LogDebug($"Created node {node.name}({node.node_id})");
            }

            var nodeCreationTask = _requests.CreateTask(
                GetUrl, 
                () => data, 
                () => { }, 
                (Action<T, UnityWebRequest>)((x, _) => AddAndOnCreate(x)),
                UnityWebRequest.kHttpVerbPOST
            );
            
            _dispatcher.EnqueueActionWithNotification(nodeCreationTask, notification, 4);
        }

        private void EnqueueProjectCreation()
        {
            var notification = "Creating project " + Name;

            var getProjectsTask = _requests.CreateTask<List<GnsJProject>>(
                () => $"{_serverAddress}projects",
                () => "{}",
                () => { },
                (projectList, _) =>
                {
                    var foundProject = projectList.Find(p => p.project_id == Id);
                    _tempRequest = foundProject is null
                        ? ($"{_serverAddress}projects", $"{{\"name\": \"{Name}\"}}")
                        : ($"{_serverAddress}projects/{foundProject.project_id}/open", "{}");
                },
                UnityWebRequest.kHttpVerbGET
            );

            var createProjectTask = _requests.CreateTask<GnsJProject>(
                () => _tempRequest.Item1,
                () => _tempRequest.Item2,
                () => { },
                (project, _) =>
                {
                    _jProject = project;
                    Id = _jProject.project_id;
                    _logger.LogDebug($"Created project {Name}({Id})");
                },
                UnityWebRequest.kHttpVerbPOST
            );
            

            _dispatcher.EnqueueActionWithNotification(getProjectsTask, notification, 4);
            _dispatcher.EnqueueActionWithNotification(createProjectTask, notification, 4);
        }

        public void DeleteNode(GnsNode node)
        {
            var task = _requests.CreateTask(
                () => $"{_serverAddress}projects/{_jProject.project_id}/nodes/{node.ID}",
                () => "{}",
                () => { },
                _ => { _logger.LogDebug($"Created node {node.Name}({node.ID})"); },
                UnityWebRequest.kHttpVerbDELETE
            );

            _dispatcher.EnqueueActionWithNotification(task, "Removing node " + node.Name, 4);
        }

        public void StartNode(GnsNode node)
        {
            var notification = "Starting node " + node.Name;

            string GetUrl()
            {
                var url = $"{_serverAddress}projects/{_jProject.project_id}/nodes/{node.ID}/start";
                return url;
            }

            var task = _requests.CreateTask(
                GetUrl,
                () => "{}",
                () => { },
                _ =>
                {
                    node.IsStarted = true;
                    _logger.LogDebug($"Started node {node.Name}({node.ID})");
                },
                UnityWebRequest.kHttpVerbPOST
            );
            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        public void StopNode(GnsNode node)
        {
            var notification = "Stopping node " + node.Name;
            var url = $"{_serverAddress}projects/{_jProject.project_id}/nodes/{node.ID}/stop";

            var task = _requests.CreateTask(
                () => url,
                () => "{}",
                () => { },
                _ =>
                {
                    node.IsStarted = false;
                    _logger.LogDebug($"Stopped node {node.Name}({node.ID})");
                }, UnityWebRequest.kHttpVerbPOST
            );

            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        public void RemoveLink(string linkID, GnsNode self, GnsNode other)
        {
            var notification = $"Unlinking {self.Name} and {other.Name}";
            var url = $"{_serverAddress}projects/{_jProject.project_id}/links/{linkID}";

            var task = _requests.CreateTask(
                () => url,
                () => "{}",
                () => { },
                _ =>
                {
                    _logger.LogDebug($"Unlinking node {self.Name} and {other.Name}");
                },
                UnityWebRequest.kHttpVerbDELETE
            );

            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        public void AddLink(string linkJson, GnsNode self, GnsNode other, Action<GnsJLink> callback)
        {
            var notification = "Linking " + self.Name + " and " + other.Name;
            var url = $"{_serverAddress}projects/{_jProject.project_id}/links";
            
            var task = _requests.CreateTask<GnsJLink>(
                () => url,
                () => linkJson,
                () =>
                {
                    _logger.LogDebug($"Linking node {self.Name} and {other.Name}");
                },
                (link, _) => callback(link),
                UnityWebRequest.kHttpVerbPOST
            );
            
            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }
    }
}