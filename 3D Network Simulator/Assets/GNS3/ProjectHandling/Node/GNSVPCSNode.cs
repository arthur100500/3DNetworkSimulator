using Newtonsoft.Json;
using GNSJsonObject;
using UnityEngine;
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

            // Create Node
            GNSThread.GNSThread.EnqueueAction(InitializeNode);
        }

        private string InitializeNode()
        {
            GlobalNotificationManager.AddMessage("Started creating node " + Name);
            var res = project.MakeProjectPostRequest("nodes", "{\"name\": \"" + Name + "\", \"node_type\": \"vpcs\", \"compute_id\": \"local\"}");
            JNode = JsonConvert.DeserializeObject<GNSJVPCSNode>(res);
            GlobalNotificationManager.AddMessage("Finished creating node " + Name);

            node_id = JNode.node_id;
            IsReady = true;
            return "";
        }

        public override void Start()
        {
            GNSThread.GNSThread.EnqueueAction(StartNode);
        }

        private string StartNode()
        {
            GlobalNotificationManager.AddMessage("Started starting node " + Name);
            MakeNodePostRequest("start", "{}");
            GlobalNotificationManager.AddMessage("Finished starting node " + Name);
            return "";
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
            string func() => ConnectToFunc(other, selfAdapterID, otherAdapterID);
            GNSThread.GNSThread.EnqueueAction(func);
        }

        public override void DisconnectFrom(GNSNode other, int selfAdapterID, int otherAdapterID)
        {
            string func() => DeleteFromFunc(other, selfAdapterID, otherAdapterID);
            GNSThread.GNSThread.EnqueueAction(func);
        }

        private string DeleteFromFunc(GNSNode other, int selfAdapterID, int otherAdapterID)
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

            GlobalNotificationManager.AddMessage("Started unlinking " + Name + " and " + other.Name);
            var res = project.MakeProjectDeleteRequest("links/" + selectedLink.link_id);
            GlobalNotificationManager.AddMessage("Finished unlinking " + Name + " and " + other.Name);

            other.links.Remove(selectedLink);
            links.Remove(selectedLink);
            return "";
        }

        private string ConnectToFunc(GNSNode other, int selfAdapterID, int otherAdapterID)
        {
            // https://gns3-server.readthedocs.io/en/stable/curl.html
            var link_json = "{\"nodes\": [{\"adapter_number\": " + selfAdapterID + ", \"node_id\": \"" + node_id + "\", \"port_number\": 0}, {\"adapter_number\": " + otherAdapterID + ", \"node_id\": \"" + other.node_id + "\", \"port_number\": 0}]}";

            GlobalNotificationManager.AddMessage("Started linking " + Name + " and " + other.Name);
            var res = project.MakeProjectPostRequest("links", link_json);
            GlobalNotificationManager.AddMessage("Finished linking " + Name + " and " + other.Name);

            var link = JsonConvert.DeserializeObject<GNSJLink>(res);
            other.links.Add(link);
            links.Add(link);
            return "";
        }
    }
}