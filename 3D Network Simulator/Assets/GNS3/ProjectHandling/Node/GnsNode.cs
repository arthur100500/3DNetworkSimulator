using System;
using System.Collections.Generic;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Link;
using GNS3.ProjectHandling.Project;
using Newtonsoft.Json;

namespace GNS3.ProjectHandling.Node
{
    public abstract class GnsNode : IDisposable
    {
        private List<GnsJLink> _links;
        public bool IsReady;
        public string Name;
        protected string NodeID;
        protected GnsProject Project;

        public string GnsWsUrl => "ws://" + Project.Config.Address + ":" + Project.Config.Port + "/v2/projects/" +
                                  Project.ID + "/nodes/" + NodeID;

        public void Dispose()
        {
            Project.MakeDeleteRequest("nodes/" + NodeID + "");
        }

        private string MakeNodePostRequest(string endpoint, string data)
        {
            return Project.MakeProjectPostRequest("nodes/" + NodeID + "/" + endpoint, data);
        }

        public string MakeNodeGetRequest(string endpoint)
        {
            return Project.MakeGetRequest("nodes/" + NodeID + "/" + endpoint);
        }

        protected void Init(string name, GnsProject project)
        {
            Name = name;
            Project = project;
            _links = new List<GnsJLink>();
        }

        public void Start()
        {
            var notification = "Starting node " + Name;
            QueuedTaskThread.GetInstance().EnqueueActionWithNotification(StartNode, notification, 4);
        }

        public void Stop()
        {
            var notification = "Stopping node " + Name;
            QueuedTaskThread.GetInstance().EnqueueActionWithNotification(StopNode, notification, 4);
        }

        private void StartNode()
        {
            MakeNodePostRequest("start", "{}");
        }

        private void StopNode()
        {
            MakeNodePostRequest("stop", "{}");
        }

        public void ConnectTo(GnsNode other, int selfAdapterID, int otherAdapterID)
        {
            var notification = "Linking " + Name + " and " + other.Name;

            void Func()
            {
                ConnectToFunc(other, selfAdapterID, otherAdapterID);
            }

            QueuedTaskThread.GetInstance().EnqueueActionWithNotification(Func, notification, 4);
        }

        public void DisconnectFrom(GnsNode other, int selfAdapterID, int otherAdapterID)
        {
            var notification = "Unlinking " + Name + " and " + other.Name;

            void Func()
            {
                DeleteFromFunc(other, selfAdapterID, otherAdapterID);
            }

            QueuedTaskThread.GetInstance().EnqueueActionWithNotification(Func, notification, 4);
        }

        private void DeleteFromFunc(GnsNode other, int selfAdapterID, int otherAdapterID)
        {
            var selectedLink = _links.Find(a =>
                (a.nodes[0].node_id == NodeID &&
                 a.nodes[1].node_id == other.NodeID &&
                 a.nodes[0].port_number == selfAdapterID &&
                 a.nodes[1].port_number == otherAdapterID)
                ||
                (a.nodes[1].node_id == NodeID &&
                 a.nodes[0].node_id == other.NodeID &&
                 a.nodes[1].port_number == selfAdapterID &&
                 a.nodes[0].port_number == otherAdapterID)
            );

            Project.MakeProjectDeleteRequest("links/" + selectedLink.link_id);
            other._links.Remove(selectedLink);
            _links.Remove(selectedLink);
        }

        private void ConnectToFunc(GnsNode other, int selfAdapterID, int otherAdapterID)
        {
            var linkJson = "{\"nodes\": [{\"adapter_number\": 0, \"node_id\": \"" + NodeID + "\", \"port_number\": " +
                           selfAdapterID + "}, {\"adapter_number\": 0, \"node_id\": \"" + other.NodeID +
                           "\", \"port_number\": " + otherAdapterID + "}]}";
            if (linkJson == null) throw new ArgumentNullException(nameof(linkJson));
            var res = Project.MakeProjectPostRequest("links", linkJson);
            var link = JsonConvert.DeserializeObject<GnsJLink>(res);
            other._links.Add(link);
            _links.Add(link);
        }
    }
}