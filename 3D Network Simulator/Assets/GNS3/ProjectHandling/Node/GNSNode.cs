using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GNSHandling
{
    public abstract class GNSNode
    {
        public string node_id;
        public List<GNSJLink> links;
        public string Name;
        public bool IsReady;
        protected GNSProject project;

        public string MakeNodePostRequest(string endpoint, string data)
        {
            return project.MakeProjectPostRequest("nodes/" + node_id + "/" + endpoint, data);
        }

        public string MakeNodeGetRequest(string endpoint)
        {
            return project.MakeGetRequest("nodes/" + node_id + "/" + endpoint);
        }

        protected void Init(string name, GNSProject project)
        {
            Name = name;
            this.project = project;
            links = new();
        }

        public void Start()
        {
            var onStart = "[..] Starting node " + Name;
            var onEnd = "[<color=green>OK</color>] Starting node " + Name;
            GNSThread.GNSThread.EnqueueActionWithNotifications(StartNode, onStart, onEnd, 4);
        }

        public void Stop()
        {
            var onStart = "[..] Stoping node " + Name;
            var onEnd = "[<color=green>OK</color>] Stoping node " + Name;
            GNSThread.GNSThread.EnqueueActionWithNotifications(StopNode, onStart, onEnd, 4);
        }

        private void StartNode()
        {
            MakeNodePostRequest("start", "{}");
        }

        private void StopNode()
        {
            MakeNodePostRequest("stop", "{}");
        }

        public void ConnectTo(GNSNode other, int selfAdapterID, int otherAdapterID)
        {
            var onStart = "[..] Linking " + Name + " and " + other.Name;
            var onEnd = "[<color=green>OK</color>] Linking " + Name + " and " + other.Name;

            void func() => ConnectToFunc(other, selfAdapterID, otherAdapterID);
            GNSThread.GNSThread.EnqueueActionWithNotifications(func, onStart, onEnd, 4);
        }

        public void DisconnectFrom(GNSNode other, int selfAdapterID, int otherAdapterID)
        {
            var onStart = "[..] Unlinking " + Name + " and " + other.Name;
            var onEnd = "[<color=green>OK</color>] Unlinking " + Name + " and " + other.Name;

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
            var link_json = "{\"nodes\": [{\"adapter_number\": 0, \"node_id\": \"" + node_id + "\", \"port_number\": " + selfAdapterID + "}, {\"adapter_number\": 0, \"node_id\": \"" + other.node_id + "\", \"port_number\": " + otherAdapterID + "}]}";
            var res = project.MakeProjectPostRequest("links", link_json);
            var link = JsonConvert.DeserializeObject<GNSJLink>(res);
            other.links.Add(link);
            links.Add(link);
        }
    }
}