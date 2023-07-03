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
using Project.ProjectSaver;
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
        private IProjectSaver _saver;

        [SerializeField] private DeviceFactory deviceFactory;


        public void Start()
        {
            var logger = new DebugLogger();

            Init(
                MenuToGameExchanger.RequestMaker,
                QueuedTaskCoroutineDispatcher.GetInstance(),
                MenuToGameExchanger.ProjectConfig,
                MenuToGameExchanger.InitialProject,
                logger
            );
        }

        private void Init(
            IRequestMaker request,
            IQueuedTaskDispatcher dispatcher,
            GnsProjectConfig config,
            NsJProject nsjProject,
            ILogger logger
        )
        {
            _requests = request;
            _dispatcher = dispatcher;
            _logger = logger;
            _config = config;

            var id = nsjProject.GnsID;
            var prName = nsjProject.Name;
            var initial = nsjProject;

            _project = new GnsProject(
                config,
                id,
                prName,
                _requests,
                _dispatcher,
                _logger,
                this
            );

            
            if (MenuToGameExchanger.UseLocalGns)
                _saver = new FileProjectSaver(initial);
            
            else
                _saver = new ServerProjectSaver(initial, _project, _requests, _config, _dispatcher);
            
            
            InitializeDevices(nsjProject.JsonAnnotation);
        }

        private void InitializeDevices(string projectJson)
        {
            var projectJsonList = JsonConvert.DeserializeObject<List<DeviceEntry>>(projectJson);

            deviceFactory.CreateAll(projectJsonList, _project, gameObject.transform);
        }

        public void AddPlaceable(APlaceable mold)
        {
            mold.gameObject.transform.SetParent(gameObject.transform);

            if (mold is ADevice device)
                device.CreateNode(_project);
        }

        public void SaveDevices()
        {
            var entries = new List<DeviceEntry>();

            for (var i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);
                
                var component = child.GetComponent<APlaceable>();

                if (component is null)
                    continue;

                var entry = component is ADevice device ? MakeEntry(device, device.Node) : MakeEntry(component, null);

                entries.Add(entry);
            }

            var serialized = JsonConvert.SerializeObject(entries);
            
            
            _saver.Save(serialized);
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
    }
}