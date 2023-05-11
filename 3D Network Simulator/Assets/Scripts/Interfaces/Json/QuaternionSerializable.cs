using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;

namespace Interfaces.Json
{
    public class QuaternionSerializable
    {
        [Preserve] [JsonProperty] private float _x;
        [Preserve] [JsonProperty] private float _y;
        [Preserve] [JsonProperty] private float _z;
        [Preserve] [JsonProperty] private float _w;

        [JsonIgnore]
        public Quaternion Quaternion
        {
            get => new Quaternion(_x, _y, _z, _w);
            set
            {
                _x = value.x;
                _y = value.y;
                _z = value.z;
                _w = value.w;
            }
        }
        
        [Preserve] [JsonConstructor] 
        public QuaternionSerializable()
        {
        }
        
        public QuaternionSerializable(Quaternion actual)
        {
            this.Quaternion = actual;
        }
    }
}