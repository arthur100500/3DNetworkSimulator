using System.Collections.Generic;
using UnityEngine.Scripting;

namespace GNS3.JsonObjects
{
    [Preserve] 
    public class GnsJLink
    {
        [Preserve] public object capture_file_name { get; set; }
        [Preserve] public object capture_file_path { get; set; }
        [Preserve] public bool capturing { get; set; }
        [Preserve] public string link_id { get; set; }
        [Preserve] public List<GNSJConnectedNode> nodes { get; set; }
        [Preserve] public string project_id { get; set; }

        [Preserve] public GnsJLink() { }
    }
    
    [Preserve] 
    public class GNSJConnectedNode
    {
        [Preserve] public int adapter_number { get; set; }
        [Preserve] public string node_id { get; set; }
        [Preserve] public int port_number { get; set; }
        
        [Preserve] public GNSJConnectedNode() { }
    }
}