using GNS.ProjectHandling.Node;
using GNS.ProjectHandling.Project;
using Objects.Common;

namespace Objects.Devices.Common
{
    public abstract class ADevice : APlaceable
    {
        public GnsNode Node;

        public void AssignNode(GnsNode node)
        {
            Node = node;
        }

        public abstract void CreateNode(GnsProject parent);
    }
}