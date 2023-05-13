using GNS.ProjectHandling.Node;
using GNS.ProjectHandling.Project;
using Objects.Common;
using Objects.Parts.Wire;

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
        public abstract AWire GetWire(int adapterNumber, int portNumber);
    }
}