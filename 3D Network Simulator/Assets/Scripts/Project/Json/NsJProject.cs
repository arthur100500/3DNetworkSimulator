using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Project.Json
{
    [Preserve]
    public class NsJProject
    {
        [Preserve] [JsonProperty] public int Id { get; set; }
        [Preserve] [JsonProperty] public string Name { get; set; }
        [Preserve] [JsonProperty] public string GnsId { get; set; }
        [Preserve] [JsonProperty] public string OwnerId { get; set; }
        [Preserve] [JsonProperty] public string JsonAnnotation { get; set; }

        [Preserve]
        [JsonConstructor]
        public NsJProject()
        {
        }
    }
}