using Newtonsoft.Json;
using GNSJsonObject;
using System;
using GNSThread;
using UI.NotificationConsole;

namespace GNSHandling
{
    public class GNSVPCSNode : GNSNode
    {
        public GNSJVPCSNode JNode;

        public GNSVPCSNode(GNSProject project, string name)
        {
            Init(name, project);

            var onStart = "[..] Creating node " + Name;
            var onEnd = "[<color=green>OK</color>] Creating node " + Name;
            GNSThread.GNSThread.EnqueueActionWithNotifications(InitializeNode, onStart, onEnd, 4);
        }

        private void InitializeNode()
        {
            var res = project.MakeProjectPostRequest("nodes", "{\"name\": \"" + Name + "\", \"node_type\": \"vpcs\", \"compute_id\": \"local\"}");
            JNode = JsonConvert.DeserializeObject<GNSJVPCSNode>(res);
            node_id = JNode.node_id;
            IsReady = true;
        }
    }
}