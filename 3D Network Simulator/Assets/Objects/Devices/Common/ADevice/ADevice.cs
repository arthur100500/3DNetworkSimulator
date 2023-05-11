using GNS3.ProjectHandling.Node;
using Interfaces.Json;
using Newtonsoft.Json;
using Objects.Devices.PC.Laptop;
using UnityEngine;

namespace Objects.Devices.Common.ADevice
{
    public abstract class ADevice : MonoBehaviour, IJsonObject
    {
        [JsonProperty]
        public GnsNode Node;
        
        public string ToJson()
        {
            var type = GetType();

            var aJsonDevice = new AJDevice(
                Node,
                gameObject.transform.rotation,
                gameObject.transform.position,
                type
            );

            return JsonConvert.SerializeObject(aJsonDevice);
        }
    }
}