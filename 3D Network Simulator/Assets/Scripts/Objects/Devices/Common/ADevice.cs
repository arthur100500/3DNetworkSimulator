using GNS.ProjectHandling.Node;
using GNS.ProjectHandling.Project;
using GNS3.ProjectHandling.Node;
using UnityEngine;

namespace Objects.Devices.Common
{
    public abstract class ADevice : MonoBehaviour
    {
        public GnsNode Node;

        public void AssignNode(GnsNode node)
        {
            Node = node;
        }

        public abstract void CreateNode(GnsProject parent);
    }
}