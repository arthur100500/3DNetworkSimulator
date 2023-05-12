using System;
using System.Collections.Generic;
using GNS.ProjectHandling.Node;
using GNS.ProjectHandling.Project;
using GNS3.ProjectHandling.Node;
using Objects.Devices.Common;
using Objects.Devices.Hub.SmallHub;
using Objects.Devices.PC.Laptop;
using Objects.Env.Table;
using Project.Json;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Project
{
    public class DeviceFactory : MonoBehaviour
    {
        private Dictionary<string, GameObject> _prefabs;

        [SerializeField] private GameObject laptopPrefab;
        [SerializeField] private GameObject hubPrefab;
        [SerializeField] private GameObject tablePrefab;

        public void Start()
        {
            _prefabs = new Dictionary<string, GameObject>
            {
                { nameof(Laptop), laptopPrefab },
                { nameof(SmallHub), hubPrefab },
                { nameof(Table), tablePrefab },
            };
        }

        public void Create(DeviceEntry entry, GnsProject project, Transform projectTransform)
        {
            var type = entry.Type;
            var deviceGameObject = Instantiate(_prefabs[type], projectTransform);
            deviceGameObject.transform.position = entry.Position.Vector3;
            deviceGameObject.transform.rotation = entry.Rotation.Quaternion;
            
            var deviceComponent = deviceGameObject.GetComponent<ADevice>();
            if (deviceComponent is null || entry.Node is null)
                return;
            
            var gnsNode = entry.Node;
            gnsNode.SetProject(project);
            deviceComponent.AssignNode(gnsNode);
            gnsNode.IsReady = true;
        }
    }
}