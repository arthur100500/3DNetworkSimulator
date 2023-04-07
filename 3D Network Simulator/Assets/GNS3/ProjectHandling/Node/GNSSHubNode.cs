using GNSJsonObject;
using Newtonsoft.Json;

namespace GNSHandling
{
    public class GNSSHubNode : GNSNode
    {
        public GNSJSHubNode JNode;

        public GNSSHubNode(GNSProject project, string name)
        {
            Init(name, project);

            var notification = "Creating node " + Name;
            GNSThread.GNSThread.EnqueueActionWithNotification(InitializeNode, notification, 4);
        }

        private void InitializeNode()
        {
            var res = project.MakeProjectPostRequest("nodes",
                "{\"name\": \"" + Name + "\", \"node_type\": \"ethernet_hub\", \"compute_id\": \"local\"}");

            JNode = JsonConvert.DeserializeObject<GNSJSHubNode>(res);

            node_id = JNode.node_id;
            IsReady = true;
        }
    }
}