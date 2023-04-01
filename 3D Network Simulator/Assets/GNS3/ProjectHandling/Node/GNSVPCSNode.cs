using Newtonsoft.Json;
using GNSJsonObject;
using System;
using GNSThread;
using UI.NotificationConsole;

namespace GNSHandling
{
    public class GNSVPCSNode : GNSNode
    {
        public GNSJVPCSNode JNode;
        private readonly GNSProject project;

        public GNSVPCSNode(GNSProject project, string name)
        {
            Name = name;
            this.project = project;
            links = new();

            var onStart = "[..] Creating node " + Name;
            var onEnd = "[OK] Creating node " + Name;
            GNSThread.GNSThread.EnqueueActionWithNotifications(InitializeNode, onStart, onEnd, 4);
        }

        private void InitializeNode()
        {
            var res = project.MakeProjectPostRequest("nodes", "{\"name\": \"" + Name + "\", \"node_type\": \"vpcs\", \"compute_id\": \"local\"}");
            JNode = JsonConvert.DeserializeObject<GNSJVPCSNode>(res);
            node_id = JNode.node_id;
            IsReady = true;
        }

        public override void Start()
        {
            var onStart = "[..] Starting node " + Name;
            var onEnd = "[OK] Starting node " + Name;
            GNSThread.GNSThread.EnqueueActionWithNotifications(StartNode, onStart, onEnd, 4);
        }

        private void StartNode()
        {
            MakeNodePostRequest("start", "{}");
        }
        public string MakeNodePostRequest(string endpoint, string data)
        {
            return project.MakeProjectPostRequest("nodes/" + JNode.node_id + "/" + endpoint, data);
        }

        public string MakeNodeGetRequest(string endpoint)
        {
            return project.MakeGetRequest("nodes/" + JNode.node_id + "/" + endpoint);
        }

        public override void ConnectTo(GNSNode other, int selfAdapterID, int otherAdapterID)
        {
            var onStart = "[..] Linking " + Name + " and " + other.Name;
            var onEnd = "[OK] Linking " + Name + " and " + other.Name;

            void func() => ConnectToFunc(other, selfAdapterID, otherAdapterID);
            GNSThread.GNSThread.EnqueueActionWithNotifications(func, onStart, onEnd, 4);
        }

        public override void DisconnectFrom(GNSNode other, int selfAdapterID, int otherAdapterID)
        {
            var onStart = "[..] Unlinking " + Name + " and " + other.Name;
            var onEnd = "[OK] Unlinking " + Name + " and " + other.Name;

            void func() => DeleteFromFunc(other, selfAdapterID, otherAdapterID);
            GNSThread.GNSThread.EnqueueActionWithNotifications(func, onStart, onEnd, 4);
        }

        private void DeleteFromFunc(GNSNode other, int selfAdapterID, int otherAdapterID)
        {
            // Get Link ID
            // TOP 10 LAMBDA EXPRESSIONS!!!!
            var selectedLink = links.Find(a =>
                (a.nodes[0].node_id == node_id &&
                a.nodes[1].node_id == other.node_id &&
                a.nodes[0].port_number == selfAdapterID &&
                a.nodes[1].port_number == otherAdapterID)
                ||
                (a.nodes[1].node_id == node_id &&
                a.nodes[0].node_id == other.node_id &&
                a.nodes[1].port_number == selfAdapterID &&
                a.nodes[0].port_number == otherAdapterID)
            );
            var res = project.MakeProjectDeleteRequest("links/" + selectedLink.link_id);
            other.links.Remove(selectedLink);
            links.Remove(selectedLink);
        }

        private void ConnectToFunc(GNSNode other, int selfAdapterID, int otherAdapterID)
        {
            var link_json = "{\"nodes\": [{\"adapter_number\": " + selfAdapterID + ", \"node_id\": \"" + node_id + "\", \"port_number\": 0}, {\"adapter_number\": " + otherAdapterID + ", \"node_id\": \"" + other.node_id + "\", \"port_number\": 0}]}";
            var res = project.MakeProjectPostRequest("links", link_json);
            var link = JsonConvert.DeserializeObject<GNSJLink>(res);
            other.links.Add(link);
            links.Add(link);
        }
    }
}