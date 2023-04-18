using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace GNS3.JsonObjects.Basic
{
    [Preserve]
    public class GnsJNode
    {
        [Preserve] public string name { get; set; }
        [Preserve] public string node_id { get; set; }
        [Preserve] public string node_type { get; set; }
        [Preserve] public string project_id { get; set; }
        
        [JsonConstructor] public GnsJNode() {}
    }
}