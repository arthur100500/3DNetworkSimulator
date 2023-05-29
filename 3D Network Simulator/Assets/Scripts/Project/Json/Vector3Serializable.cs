using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace Interfaces.Json
{
    [Preserve]
    public class Vector3Serializable
    {
        [Preserve] [JsonProperty] private float _x;
        [Preserve] [JsonProperty] private float _y;
        [Preserve] [JsonProperty] private float _z;

        [JsonIgnore]
        public Vector3 Vector3
        {
            get => new Vector3(_x, _y, _z);
            set
            {
                _x = value.x;
                _y = value.y;
                _z = value.z;
            }
        }
        
        [Preserve] [JsonConstructor] 
        public Vector3Serializable()
        {
        }
        
        public Vector3Serializable(Vector3 actual)
        {
            Vector3 = actual;
        }
    }
}