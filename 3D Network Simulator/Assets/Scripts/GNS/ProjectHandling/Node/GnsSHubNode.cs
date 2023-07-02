using GNS.JsonObjects;
using GNS.ProjectHandling.Project;
using GNS3.JsonObjects;

namespace GNS.ProjectHandling.Node
{
    public class GnsSHubNode : GnsNode
    {
        private GnsJsHubNode _jNode;

        public GnsSHubNode(GnsProject project, string name)
        {
            Init(name, project);
            InitializeNode();
        }

        private void InitializeNode()
        {
            void AssignNode(GnsJsHubNode jNode)
            {
                _jNode = jNode;
                ID = _jNode.node_id;
                IsReady = true;
            }

            Project.CreateNode<GnsJsHubNode>(Name, "ethernet_hub", AssignNode, this);
        }
    }
}