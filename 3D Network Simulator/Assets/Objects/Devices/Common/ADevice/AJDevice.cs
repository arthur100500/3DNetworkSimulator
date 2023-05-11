using System;
using GNS3.ProjectHandling.Node;
using Interfaces.Json;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Scripting;

namespace Objects.Devices.Common.ADevice
{
    [Preserve]
    public class AJDevice
    {
        [Preserve] [JsonProperty] public GnsNode Node;
        [Preserve] [JsonProperty] public QuaternionSerializable Rotation;
        [Preserve] [JsonProperty] public Vector3Serializable Position;
        [Preserve] [JsonProperty] public Type T;
        
        [Preserve] [JsonConstructor]
        public AJDevice()
        {
        }

        public AJDevice(GnsNode node, Quaternion rotation, Vector3 position, Type t)
        {
            Position = new Vector3Serializable(position);
            Rotation = new QuaternionSerializable(rotation);
            Node = node;
            T = t;
        }
    }
}