using Newtonsoft.Json;
using GNSJsonObject;
using UnityEngine;

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

            // Create Node
            var res = this.project.MakeProjectPostRequest("nodes", "{\"name\": \"" + Name + "\", \"node_type\": \"vpcs\", \"compute_id\": \"local\"}");
            JNode = JsonConvert.DeserializeObject<GNSJVPCSNode>(res);
            node_id = JNode.node_id;
        }

        public override void Start()
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
            // https://gns3-server.readthedocs.io/en/stable/curl.html
            var link_json = "{\"nodes\": [{\"adapter_number\": " + selfAdapterID + ", \"node_id\": \"" + node_id + "\", \"port_number\": 0}, {\"adapter_number\": " + selfAdapterID + ", \"node_id\": \"" + other.node_id + "\", \"port_number\": 0}]}";
            var res = project.MakeProjectPostRequest("links", link_json);
            link = JsonConvert.DeserializeObject<GNSJLink>(res);
            other.link = link;
        }
    }
}