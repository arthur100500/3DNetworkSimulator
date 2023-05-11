using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace Menu.Json
{
    [Preserve]
    public class NsJProject
    {
        [Preserve] public int Id { get; set; }
        [Preserve] public string Name { get; set; }
        [Preserve] public string GnsId { get; set; }
        [Preserve] public string OwnerId { get; set; }
        [Preserve] public string JsonAnnotation { get; set; }

        [Preserve]
        [JsonConstructor]
        public NsJProject()
        {
        }
    }
}