using System;
using System.Collections.Generic;
using System.Linq;
using GNS.ProjectHandling.Project;
using Objects.Devices.Common;
using Objects.Devices.Hub.SmallHub;
using Objects.Devices.PC.Laptop;
using Objects.Env.Table;
using Objects.Parts.Wire;
using Project.Json;
using UnityEngine;
using Wire;

namespace Project
{
    public class DeviceFactory : MonoBehaviour
    {
        private Dictionary<string, GameObject> _prefabs;
        private Dictionary<(int, int, string), (int, int, string)> _links;

        [SerializeField] private GameObject laptopPrefab;
        [SerializeField] private GameObject hubPrefab;
        [SerializeField] private GameObject tablePrefab;
        [SerializeField] private Transform floor;

        public void Start()
        {
            _links = new Dictionary<(int, int, string), (int, int, string)>();

            _prefabs = new Dictionary<string, GameObject>
            {
                { nameof(Laptop), laptopPrefab },
                { nameof(SmallHub), hubPrefab },
                { nameof(Table), tablePrefab },
            };
        }

        private GameObject Create(DeviceEntry entry, GnsProject project, Transform projectTransform)
        {
            var type = entry.Type;
            var deviceGameObject = Instantiate(_prefabs[type], projectTransform);
            deviceGameObject.transform.position = entry.Position.Vector3;
            deviceGameObject.transform.rotation = entry.Rotation.Quaternion;

            var deviceComponent = deviceGameObject.GetComponent<ADevice>();
            if (deviceComponent is null || entry.Node is null)
                return deviceGameObject;

            var gnsNode = entry.Node;
            gnsNode.SetProject(project);
            project.AddNodeRaw(gnsNode);
            deviceComponent.AssignNode(gnsNode);
            gnsNode.IsReady = true;

            return deviceGameObject;
        }

        public void CreateAll(IEnumerable<DeviceEntry> entries, GnsProject project, Transform projectTransform)
        {
            var objects = new List<GameObject>();

            foreach (var entry in entries)
            {
                var gameObject = Create(entry, project, projectTransform);
                objects.Add(gameObject);
            }

            var devices = objects
                .Where(x => x.GetComponent<ADevice>() is not null)
                .Select(x => x.GetComponent<ADevice>());

            var aDevices = devices as ADevice[] ?? devices.ToArray();

            project.RefreshNodeLinks(() => LinkDevices(aDevices));
        }

        private void LinkDevices(IEnumerable<ADevice> aDevices)
        {
            foreach (var device in aDevices)
            {
                var links = device
                    .GetComponent<ADevice>().Node.Links;

                foreach (var link in links)
                {
                    var firstDev = aDevices.First(x => x.Node.ID == link.nodes[0].node_id);
                    var secondDev = aDevices.First(x => x.Node.ID == link.nodes[1].node_id);

                    var notAdded = _links.TryAdd(
                        (link.nodes[0].adapter_number, link.nodes[0].port_number, link.nodes[0].node_id),
                        (link.nodes[1].adapter_number, link.nodes[1].port_number, link.nodes[1].node_id)
                    );
                    
                    notAdded = notAdded && _links.TryAdd(
                        (link.nodes[1].adapter_number, link.nodes[1].port_number, link.nodes[1].node_id),
                        (link.nodes[0].adapter_number, link.nodes[0].port_number, link.nodes[0].node_id)
                    );

                    if (!notAdded)
                        continue;

                    var first = firstDev.GetWire(link.nodes[0].adapter_number, link.nodes[0].port_number);
                    var second = secondDev.GetWire(link.nodes[1].adapter_number, link.nodes[1].port_number);

                    CreateConnection(first, second);
                }
            }
        }

        private void CreateConnection(AWire first, AWire second)
        {
            var wireGameObject = new GameObject();

            var wireRenderer = wireGameObject.AddComponent<WireRenderer>();
            wireRenderer.width = 0.006f;
            wireRenderer.p1 = first.transform;
            wireRenderer.p2 = second.transform;
            wireRenderer.floor = floor.transform;
            wireRenderer.sagging = 3;
            wireRenderer.GetComponent<Renderer>().material = first.GetWireMaterial();

            var o = wireRenderer.gameObject;
            first.wireRenderer = o;
            second.wireRenderer = o;

            first.VisualConnect();
            second.VisualConnect();

            first.Connect(second);
            second.Connect(first);
        }
    }
}