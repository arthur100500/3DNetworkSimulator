using System;
using System.Collections.Generic;
using Gameplay.MainMenu.InterScene;
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
            
            Init(
                MenuToGameExchanger.RequestMaker,
                MenuToGameExchanger.Dispatcher,
                MenuToGameExchanger.ProjectConfig,
                MenuToGameExchanger.InitialProjectJson,
                logger
            );
        }
        
        private void Init(
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