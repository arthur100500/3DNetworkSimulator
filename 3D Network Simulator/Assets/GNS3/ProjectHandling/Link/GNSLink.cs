using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GNSHandling
{
    public class GNSJLink
    {
        public object capture_file_name { get; set; }
        public object capture_file_path { get; set; }
        public bool capturing { get; set; }
        public string link_id { get; set; }
        public List<GNSJConnectedNode> nodes { get; set; }
        public string project_id { get; set; }
    }

    public class GNSJConnectedNode
    {
        public int adapter_number { get; set; }
        public string node_id { get; set; }
        public int port_number { get; set; }
    }
}
