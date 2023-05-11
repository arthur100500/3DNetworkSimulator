using System;
using System.Collections.Generic;
using GNS.ProjectHandling.Node;
using GNS.ProjectHandling.Project;
using GNS3.GNSThread;
using GNS3.ProjectHandling.Project;
using Interfaces.Json;
using Logger;
using Newtonsoft.Json;
using Objects.Common;
using Objects.Devices.Common;
using Project.Json;
using Tasks.Requests;
using UnityEngine;
using UnityEngine.Networking;
using ILogger = Interfaces.Logger.ILogger;
using NsJProject = Menu.Json.NsJProject;

namespace Project
{
    public class Project : MonoBehaviour
    {
        private IRequestMaker _requests;
        private GnsProjectConfig _config;
        private IQueuedTaskDispatcher _dispatcher;
        private GnsProject _project;
        private ILogger _logger;
        private NsJProject _initial;

        [SerializeField] private DeviceFactory _deviceFactory;

        public void Start()
        {
            var logger = new DebugLogger();
            var reqs = new WebRequestMaker(logger);

            var cookie =
                ".AspNetCore.Identity.Application=CfDJ8KibhKtdp-hLv8zF1sQmiBHYucC6TVrF96lrzue2CTnqE6cA7PCzVk6A4Vj4_3cOI9dfCVN9MQi3fpEIYAqpMa6ITqCvzNGgw-eMXbGrN3GcAdaH3J63RpObani0lDgLviYZbXJ7ECiHGlLoYinz3Wkw-CkP5N1hC718mEdILYAOa7piHWsiOSUPLGcyA0JoJy4vvke2sVbnUYPQNw3T1OVjxfl8ePH8EUAFN7ohihdMq_ksm7bufnkeP9IwdffdQQNywfKB1U2san5oPiUCtvGDWMCi2KpN2dsRyEhKngsC-jN_qi7VIJxjWMEFp4Xv2iCYnX2eKBEWdOqdsAJmoPXWotHiBhA5nrxlyqtKy_h6nuxCGmjjt5CORuZL9FitW-vXqQwS2xq25F2ws7A8cmeZZoDWx5LIplWC3c-M-jTBjw_zUtbyr958szFNHLW3NJZAl9tRNH-QrIsXqMN84VJW-p-im1Z6PrqTmTVJeCNaoK57cG74Z6y1UIVOuaDG4M-F7yYB8lIS1oAhbLGmdEk8QL-Ki1lcc1iS_GcchFh-GbimU-FN2bHeYpXJ1AzaXIi_hPwULti792jzu_hc6W7fH7HG-ESMqOTXiFMF5TJAlsrIeEvxIbc2Q7S4tZtoNBFffy_mdWjX6bFh_uGQ0H3D8Xr35m6X0Th3ba1oHQa1124d8q-nBwBn0Mpi7ZO2l7nDIOnl481nLhFN8nEpnK_vU7gKSIZXxF21XqKJpY3c; Path=/; HttpOnly; Expires=Thu, 25 May 2023 21:01:46 GMT;";

            reqs.SetCookies(cookie);
                
            var mockProject =
                "[{\\\"Type\\\":\\\"Laptop\\\",\\\"Node\\\":{\\\"ID\\\":\\\"f7b7d8fe-b49f-4e7a-8036-e333df0b7b97\\\",\\\"IsStarted\\\":true,\\\"Name\\\":\\\"Laptop\\\",\\\"_links\\\":[{\\\"capture_file_name\\\":null,\\\"capture_file_path\\\":null,\\\"capturing\\\":false,\\\"link_id\\\":\\\"5acb7f94-c169-4c2c-9ce0-1282328500b0\\\",\\\"nodes\\\":[{\\\"adapter_number\\\":0,\\\"node_id\\\":\\\"f7b7d8fe-b49f-4e7a-8036-e333df0b7b97\\\",\\\"port_number\\\":0},{\\\"adapter_number\\\":0,\\\"node_id\\\":\\\"8d9e11ba-8b5c-4e50-be79-4b2d1ad8c11c\\\",\\\"port_number\\\":0}],\\\"project_id\\\":\\\"05f017ba-1cb7-444a-82e2-67e306d6de9a\\\"}]},\\\"Position\\\":{\\\"_x\\\":1.91631794,\\\"_y\\\":-9.97,\\\"_z\\\":-9.398485},\\\"Rotation\\\":{\\\"_x\\\":0.0,\\\"_y\\\":0.858643,\\\"_z\\\":0.0,\\\"_w\\\":-0.512574136}},{\\\"Type\\\":\\\"Laptop\\\",\\\"Node\\\":{\\\"ID\\\":\\\"8d9e11ba-8b5c-4e50-be79-4b2d1ad8c11c\\\",\\\"IsStarted\\\":true,\\\"Name\\\":\\\"Laptop\\\",\\\"_links\\\":[{\\\"capture_file_name\\\":null,\\\"capture_file_path\\\":null,\\\"capturing\\\":false,\\\"link_id\\\":\\\"5acb7f94-c169-4c2c-9ce0-1282328500b0\\\",\\\"nodes\\\":[{\\\"adapter_number\\\":0,\\\"node_id\\\":\\\"f7b7d8fe-b49f-4e7a-8036-e333df0b7b97\\\",\\\"port_number\\\":0},{\\\"adapter_number\\\":0,\\\"node_id\\\":\\\"8d9e11ba-8b5c-4e50-be79-4b2d1ad8c11c\\\",\\\"port_number\\\":0}],\\\"project_id\\\":\\\"05f017ba-1cb7-444a-82e2-67e306d6de9a\\\"}]},\\\"Position\\\":{\\\"_x\\\":2.77500057,\\\"_y\\\":-9.97,\\\"_z\\\":-11.3616323},\\\"Rotation\\\":{\\\"_x\\\":0.0,\\\"_y\\\":0.679098248,\\\"_z\\\":0.0,\\\"_w\\\":0.7340474}}]";            var mockJson =
                "{\"Id\":1,\"Name\":\"Project 1\",\"GnsId\":\"05f017ba-1cb7-444a-82e2-67e306d6de9a\",\"OwnerId\":\"3cf60524-baaf-4463-a84f-333f3c30d3da\",\"JsonAnnotation\":\"" + mockProject + "\"}";
            
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
            _requests = request;
            _dispatcher = dispatcher;
            _logger = logger;
            _config = config;

            var nsjProject = JsonConvert.DeserializeObject<NsJProject>(projectJson);
            var id = nsjProject.GnsId;

            _initial = nsjProject;
                
            _project = new GnsProject(
                config,
                id,
                _requests,
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

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.J))
                SaveDevices();
        }

        public void AddPlaceable(APlaceable mold)
        {
            mold.gameObject.transform.SetParent(gameObject.transform);
            
            if (mold is ADevice device)
                device.CreateNode(_project);
        }

        private void SaveDevices()
        {
            var entries = new List<DeviceEntry>();

            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);

                DeviceEntry entry = null;
                
                var component = child.GetComponent<APlaceable>();
                
                if (component is null)
                    continue;
                
                entry = component is ADevice device ? 
                    MakeEntry(device, device.Node) : 
                    MakeEntry(component, null);
                
                entries.Add(entry);
            }

            var serialized = JsonConvert.SerializeObject(entries);
            SendUpdateJson(serialized);
        }

        private static DeviceEntry MakeEntry(Component component, GnsNode node)
        {
            var o = component.gameObject;
                
            var entry = new DeviceEntry
            {
                Node = node,
                Position = new Vector3Serializable(o.transform.position),
                Rotation = new QuaternionSerializable(o.transform.rotation),
                Type = component.GetType().Name
            };

            return entry;
        }

        private void SendUpdateJson(string data)
        {
            var nsProj = new NsJProject
            {
                Name = _initial.Name,
                GnsId = _initial.GnsId,
                JsonAnnotation = data,
                OwnerId = _initial.OwnerId
            };

            var json = JsonConvert.SerializeObject(nsProj);
            
            var task = _requests.CreateTask(
                () => $"http://{_config.Address}:{_config.Port}/ns/update",
                () => json,
                () => { },
                _ => { },
                UnityWebRequest.kHttpVerbPOST
            );
            
            _dispatcher.EnqueueActionWithNotification(task, "Saving project", 4);
        }
    }
}