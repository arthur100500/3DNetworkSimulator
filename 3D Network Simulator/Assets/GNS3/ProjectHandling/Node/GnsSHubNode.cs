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
            _jNode = Project.CreateNode<GnsJSHubNode>(Name, "ethernet_hub");
            NodeID = _jNode.node_id;
            IsReady = true;
        }
    }
}