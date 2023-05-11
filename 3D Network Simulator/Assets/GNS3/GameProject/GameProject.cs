using System;
using System.Collections.Generic;
using System.Linq;
using GNS3.ProjectHandling.Project;
using Newtonsoft.Json;
using Objects.Devices.Common.ADevice;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace GNS3.GameProject
{
    public class GameProject : MonoBehaviour
    {
        private GnsProject _project;
        [SerializeField] private ADevice device;
        [SerializeField] private ADevice device2;
        
        [SerializeField] private List<ADevice> devicePrefabs;

        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.J))
                return;

            var devices = new List<ADevice>();
            

        }

        private void SerializeList()
        {

            KnownTypesBinder loKnownTypesBinder = new KnownTypesBinder()
            {
                KnownTypes = new List<Type> { }
            };
            
            JsonSerializerSettings loJsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                SerializationBinder = loKnownTypesBinder
            };
        }
        
        
    }

    class KnownTypesBinder : ISerializationBinder
    {
        public IList<Type> KnownTypes { get; set; }

        public Type BindToType(string assemblyName, string typeName)
        {
            return KnownTypes.SingleOrDefault(t => t.Name == typeName);
        }

        public void BindToName(Type serializedType, out string assemblyName, out string typeName)
        {
            assemblyName = null;
            typeName = serializedType.Name;
        }
    }
}
