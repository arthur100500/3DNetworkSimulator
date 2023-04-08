using GNS3.GNSThread;
using GNS3.JsonObjects;
using GNS3.ProjectHandling.Project;
using Newtonsoft.Json;

namespace GNS3.ProjectHandling.Node
{
    public class GnsVpcsNode : GnsNode
    {
        private GnsVpcsJNode _jNode;

        public GnsVpcsNode(GnsProject project, string name)
        {
            Init(name, project);

            var notification = "Creating node " + Name;
            QueuedTaskThread.GetInstance().EnqueueActionWithNotification(InitializeNode, notification, 4);
        }

        private void InitializeNode()
        {
            var res = Project.MakeProjectPostRequest("nodes",
                "{\"name\": \"" + Name + "\", \"node_type\": \"vpcs\", \"compute_id\": \"local\"}");
            _jNode = JsonConvert.DeserializeObject<GnsVpcsJNode>(res);
            NodeID = _jNode.node_id;
            IsReady = true;
        }
    }
}