using Newtonsoft.Json;
using UnityEngine.Scripting;

namespace Gameplay.MainMenu.Json
{
    [Preserve]
    public class NsProjectRaw
    {
        [Preserve] public int Id { get; set; }
        [Preserve] public string Name { get; set; }
        [Preserve] public string GnsID { get; set; }
        [Preserve] public string OwnerId { get; set; }
        [Preserve] public string JsonAnnotation { get; set; }

        [Preserve]
        [JsonConstructor]
        public NsProjectRaw()
        {
        }
    }
}