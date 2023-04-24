using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace GNS3.JsonObjects
{
    [Preserve]
    public class GnsJProject
    {
        [Preserve] public string name { get; set; }
        [Preserve] public string project_id { get; set; }

        [JsonConstructor] public GnsJProject () { }
    }
}