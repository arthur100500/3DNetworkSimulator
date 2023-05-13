using GNS.ProjectHandling.Project;
using GNS3.JsonObjects;

namespace GNS.ProjectHandling.Node
{
    public class GnsVpcsNode : GnsNode
    {
        private GnsVpcsJNode _jNode;

        public GnsVpcsNode(GnsProject project, string name)
        {
            Init(name, project);
            InitializeNode();
        }

        private void InitializeNode()
        {
            void AssignNode(GnsVpcsJNode jNode)
            {
                _jNode = jNode;
                ID = _jNode.node_id;
                IsReady = true;
            }

            Project.CreateNode<GnsVpcsJNode>(Name, "vpcs", AssignNode, this);
        }
    }
}