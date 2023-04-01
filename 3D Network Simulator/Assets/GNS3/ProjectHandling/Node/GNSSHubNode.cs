using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using GNSJsonObject;
using UnityEngine;

namespace GNSHandling
{
    public class GNSSHubNode : GNSNode
    {
        public GNSJSHubNode JNode;
        public GNSSHubNode(GNSProject project, string name)
        {
            Init(name, project);

            var onStart = "[..] Creating node " + Name;
            var onEnd = "[<color=green>OK</color>] Creating node " + Name;
            GNSThread.GNSThread.EnqueueActionWithNotifications(InitializeNode, onStart, onEnd, 4);
        }

        private void InitializeNode()
        {
            var res = project.MakeProjectPostRequest("nodes", "{\"name\": \"" + Name + "\", \"node_type\": \"ethernet_hub\", \"compute_id\": \"local\"}");

            JNode = JsonConvert.DeserializeObject<GNSJSHubNode>(res);

            node_id = JNode.node_id;
            IsReady = true;
        }
    }
}