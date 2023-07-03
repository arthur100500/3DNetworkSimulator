using System;
using System.Collections.Generic;
using System.Linq;
using GNS.JsonObjects;
using GNS.ProjectHandling.Node;
using GNS.ServerCommunication;
using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.JsonObjects.Basic;
using GNS3.ProjectHandling.Project;
using Interfaces.Logger;
using Tasks.Requests;
using UnityEngine.Networking;
using GlobalProject = global::Project.Project;

namespace GNS.ProjectHandling.Project
{
    public class GnsProject
    {
        private readonly IRequestMaker _requests;
        public readonly GnsProjectConfig Config;
        private readonly IQueuedTaskDispatcher _dispatcher;
        private (string, string) _tempRequest;
        private readonly ILogger _logger;
        private readonly List<GnsNode> _gnsNodes;
        private readonly string _serverAddress;
        private readonly GlobalProject _globalProject;

        public GnsProject(
            GnsProjectConfig config,
            string id,
            string name,
            IRequestMaker requests,
            IQueuedTaskDispatcher dispatcher,
            ILogger logger,
            GlobalProject project
        )
        {
            Id = id;
            Config = config;
            _requests = requests;
            _dispatcher = dispatcher;
            _logger = logger;
            _gnsNodes = new List<GnsNode>();
            _globalProject = project;
            Name = name;
            
            _serverAddress = $"http://{config.Address}:{config.Port}/v2/";
        }

        private string Name { get; }
        public string Id { get; }


        public void CreateNode<T>(string name, string type, Action<T> onCreate, GnsNode self) where T : GnsJNode
        {
            var notification = "Creating node " + name;
            var data = $"{{\"name\": \"{name}\", \"node_type\": \"{type}\", \"compute_id\": \"local\"}}";

            string GetUrl()
            {
                return new GnsUrl(_serverAddress).Project(Id).Nodes().Url;
            }

            void AddAndOnCreate(T node)
            {
                onCreate(node);
                _gnsNodes.Add(self);
                _logger.LogDebug($"Created node {node.name}({node.node_id})");
            }

            var nodeCreationTask = _requests.CreateTask(
                GetUrl,
                () => data,
                () => { },
                (Action<T, UnityWebRequest>)((x, _) => { AddAndOnCreate(x); _globalProject.SaveDevices(); }),
                UnityWebRequest.kHttpVerbPOST
            );

            _dispatcher.EnqueueActionWithNotification(nodeCreationTask, notification, 4);
        }

        public void Open()
        {
            var task = _requests.CreateTask(
                () => new GnsUrl(_serverAddress).Project(Id).Open().Url,
                () => "{}",
                () => { },
                _ => { _logger.LogDebug("Project Opened"); _globalProject.SaveDevices(); },
                UnityWebRequest.kHttpVerbPOST
            );
        }

        public void DeleteNode(GnsNode node)
        {
            var task = _requests.CreateTask(
                () => new GnsUrl(_serverAddress).Project(Id).Node(node.ID).Url,
                () => "{}",
                () => { },
                _ =>
                {
                    _logger.LogDebug($"Created node {node.Name}({node.ID})");
                    _gnsNodes.Remove(node);
                    _globalProject.SaveDevices();
                },
                UnityWebRequest.kHttpVerbDELETE
            );

            _dispatcher.EnqueueActionWithNotification(task, "Removing node " + node.Name, 4);
        }

        public void StartNode(GnsNode node)
        {
            var notification = "Starting node " + node.Name;

            string GetUrl()
            {
                return new GnsUrl(_serverAddress).Project(Id).Node(node.ID).Start().Url;
            }

            var task = _requests.CreateTask(
                GetUrl,
                () => "{}",
                () => { },
                _ =>
                {
                    node.IsStarted = true;
                    _logger.LogDebug($"Started node {node.Name}({node.ID})");
                    _globalProject.SaveDevices();
                },
                UnityWebRequest.kHttpVerbPOST
            );
            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        public void StopNode(GnsNode node)
        {
            var notification = "Stopping node " + node.Name;
            var url = new GnsUrl(_serverAddress).Project(Id).Node(node.ID).Stop().Url;

            var task = _requests.CreateTask(
                () => url,
                () => "{}",
                () => { },
                _ =>
                {
                    node.IsStarted = false;
                    _logger.LogDebug($"Stopped node {node.Name}({node.ID})");
                    _globalProject.SaveDevices();
                }, UnityWebRequest.kHttpVerbPOST
            );

            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        public void RemoveLink(string linkID, GnsNode self, GnsNode other)
        {
            var notification = $"Unlinking {self.Name} and {other.Name}";
            var url = new GnsUrl(_serverAddress).Project(Id).Link(linkID).Url;

            var task = _requests.CreateTask(
                () => url,
                () => "{}",
                () => { },
                _ => _globalProject.SaveDevices(),
                UnityWebRequest.kHttpVerbDELETE
            );

            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
            
        }

        public void AddLink(string linkJson, GnsNode self, GnsNode other, Action<GnsJLink> callback)
        {
            var notification = "Linking " + self.Name + " and " + other.Name;
            var url = new GnsUrl(_serverAddress).Project(Id).Links().Url;

            var task = _requests.CreateTask<GnsJLink>(
                () => url,
                () => linkJson,
                () => { },
                (link, _) => { callback(link); _globalProject.SaveDevices(); },
                UnityWebRequest.kHttpVerbPOST
            );

            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
            
            _globalProject.SaveDevices();
        }

        public void RefreshNodeLinks(Action callback)
        {
            var notification = "Refreshing links for all nodes";

            var task = _requests.CreateTask<List<GnsJLink>>(
                () => new GnsUrl(_serverAddress).Project(Id).Links().Url,
            () => "{}",
                () => { },
                (links, _) => { RefreshNodes(links, callback); _globalProject.SaveDevices(); },
                UnityWebRequest.kHttpVerbGET
            );

            _dispatcher.EnqueueActionWithNotification(task, notification, 4);
        }

        private void RefreshNodes(List<GnsJLink> links, Action callback)
        {
            _gnsNodes.ForEach(x => x.Links.Clear());

            foreach (var link in links)
            {
                if (link.nodes.Count == 0)
                    continue;

                var targetNodes = _gnsNodes.Where(
                    x => x.ID == link.nodes[0].node_id
                         || x.ID == link.nodes[1].node_id
                );

                foreach (var node in targetNodes)
                {
                    node.Links.Add(link);
                }
            }

            callback();
        }

        public void AddNodeRaw(GnsNode gnsNode)
        {
            _gnsNodes.Add(gnsNode);
        }
    }
}