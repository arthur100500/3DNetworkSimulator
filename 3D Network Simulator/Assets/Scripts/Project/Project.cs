using System;
using System.Collections.Generic;
using GNS.ProjectHandling.Project;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Project;
using Interfaces.Json;
using Logger;
using Newtonsoft.Json;
using Objects.Devices.Common;
using Project.Json;
using Tasks.Requests;
using UnityEngine;
using ILogger = Interfaces.Logger.ILogger;
using NsJProject = Menu.Json.NsJProject;

namespace Project
{
    public class Project : MonoBehaviour
    {
        private IRequestMaker _request;
        private IQueuedTaskDispatcher _dispatcher;
        private GnsProject _project;
        private ILogger _logger;

        [SerializeField] private DeviceFactory _deviceFactory;

        public void Start()
        {
            var logger = new DebugLogger();
            var reqs = new WebRequestMaker(logger);
            var mockJson =
                "{\"Id\":1,\"Name\":\"Project 1\",\"GnsId\":\"05f017ba-1cb7-444a-82e2-67e306d6de9a\",\"OwnerId\":\"3cf60524-baaf-4463-a84f-333f3c30d3da\",\"JsonAnnotation\":\"{}\"}";
            
            // MOCK INIT
            Init(
                reqs,
                QueuedTaskCoroutineDispatcher.GetInstance(),
                GnsProjectConfig.ProxyGnsProjectConfig(), 
                mockJson,
                logger
            );
        }

        public void Init(
            IRequestMaker request,
            IQueuedTaskDispatcher dispatcher,
            GnsProjectConfig config,
            string projectJson,
            ILogger logger
        )
        {
            _request = request;
            _dispatcher = dispatcher;
            _logger = logger;

            var nsjProject = JsonConvert.DeserializeObject<NsJProject>(projectJson);
            var id = nsjProject.GnsId;

            _project = new GnsProject(
                config,
                id,
                _request,
                _dispatcher,
                _logger
            );

            InitializeDevices(nsjProject.JsonAnnotation);
        }

        private void InitializeDevices(string projectJson)
        {
            var projectJsonList = JsonConvert.DeserializeObject<List<DeviceEntry>>(projectJson);

            foreach (var project in projectJsonList)
            {
                _deviceFactory.Create(project, _project, gameObject.transform);
            }
        }

        private void SaveDevices()
        {
            var entries = new List<DeviceEntry>();

            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);
                var component = child.GetComponent<ADevice>();
                var o = component.gameObject;
                
                var entry = new DeviceEntry
                {
                    Node = component.Node,
                    Position = new Vector3Serializable(o.transform.position),
                    Rotation = new QuaternionSerializable(o.transform.rotation),
                    Type = component.GetType().Name
                };
                
                entries.Add(entry);
            }

            var serialized = JsonConvert.SerializeObject(entries);
            Debug.Log(serialized);
        }
    }
}