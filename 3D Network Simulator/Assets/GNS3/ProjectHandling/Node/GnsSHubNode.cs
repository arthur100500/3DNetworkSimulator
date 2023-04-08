using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.ProjectHandling.Project;
using Newtonsoft.Json;

namespace GNS3.ProjectHandling.Node
{
    public class GnsSHubNode : GnsNode
    {
        private GnsJSHubNode _jNode;

        public GnsSHubNode(GnsProject project, string name)
        {
            Init(name, project);

            var notification = "Creating node " + Name;
            QueuedTaskThread.GetInstance().EnqueueActionWithNotification(InitializeNode, notification, 4);
        }

        private void InitializeNode()
        {
            var res = Project.MakeProjectPostRequest("nodes",
                "{\"name\": \"" + Name + "\", \"node_type\": \"ethernet_hub\", \"compute_id\": \"local\"}");

            _jNode = JsonConvert.DeserializeObject<GnsJSHubNode>(res);

            NodeID = _jNode.node_id;
            IsReady = true;
        }
    }
}