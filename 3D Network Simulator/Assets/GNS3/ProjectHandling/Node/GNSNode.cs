using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GNSHandling
{
    public abstract class GNSNode
    {
        public string node_id;
        public abstract void ConnectTo(GNSNode other, int selfAdapterID, int otherAdapterID);
        public abstract void DisconnectFrom(GNSNode other, int selfAdapterID, int otherAdapterID);
        public List<GNSJLink> links;
        public string Name;
        public abstract void Start();
        public bool IsReady;
    }
}