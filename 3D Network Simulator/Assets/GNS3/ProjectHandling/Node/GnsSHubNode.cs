using GNS3.JsonObjects;
using GNS3.ProjectHandling.Project;

namespace GNS3.ProjectHandling.Node
{
    public class GnsSHubNode : GnsNode
    {
        private GnsJSHubNode _jNode;

        public GnsSHubNode(GnsProject project, string name)
        {
            Init(name, project);
            InitializeNode();
        }

        private void InitializeNode()
        {
            void AssignNode(GnsJSHubNode jNode)
            {
                _jNode = jNode;
                ID = _jNode.node_id;
                IsReady = true;
            }

            Project.CreateNode<GnsJSHubNode>(Name, "ethernet_hub", AssignNode);
        }
    }
}