using GNS3.JsonObjects.Basic;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace GNS3.JsonObjects
{
    [Preserve]
    public class GnsVpcsJNode : GnsJNode
    {
        [Preserve] public string compute_id { get; set; }
        [Preserve] public int console { get; set; }
        [Preserve] public string console_host { get; set; }
        [Preserve] public string console_type { get; set; }
        [Preserve] public string status { get; set; }

        [JsonConstructor] public GnsVpcsJNode() { }
    }
}