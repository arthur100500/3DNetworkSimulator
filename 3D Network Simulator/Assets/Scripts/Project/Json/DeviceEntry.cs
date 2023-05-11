using GNS.ProjectHandling.Node;
using GNS3.ProjectHandling.Node;
using Interfaces.Json;
using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Project.Json
{
    [Preserve]
    public class DeviceEntry
    {
        [Preserve] [JsonProperty]
        public string Type;
        
        [Preserve] [JsonProperty]
        public GnsNode Node;
        
        [Preserve] [JsonProperty]
        public Vector3Serializable Position;
        
        [Preserve] [JsonProperty]
        public QuaternionSerializable Rotation;
        
        [Preserve] [JsonConstructor]
        public DeviceEntry()
        {
        }
    }
}