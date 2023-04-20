using System;
// ReSharper disable all
using System.Runtime.InteropServices;
// ReSharper enable all
using GNS3.GNSThread;
using Logger;
using Requests;
using Requests.Tasks;
using UnityEngine;
using UnityEngine.Scripting;

namespace GNS3.ProjectHandling.Project
{
    public class GlobalGnsParameters : MonoBehaviour
    {
        private static int _nid = 1;
        private static GnsProject _project;
        private static IQueuedTaskDispatcher _dispatcher;
        private static GnsProjectConfig _config;
        private static WaitUntilTask _initTask;

        public static int GetNextFreeID()
        {
            return _nid++;
        }
        // ReSharper disable all
        [DllImport("__Internal")]
        private static extern void RequestGnsConfig();

        // ReSharper enable all
        public static GnsProject GetProject()
        {
            return _project ??= CreateProject();
        }

        private static GnsProject CreateProject()
        {
            // ReSharper disable all
//#if UNITY_WEBGL == true && UNITY_EDITOR == false
            RequestGnsConfig();
//#endif
            // ReSharper enable all
            _initTask = new WaitUntilTask("Waiting for server data...");
            var logger = new DebugLogger();
            _config = GnsProjectConfig.ProxyGnsProjectConfig();
            var addrBegin = $"http://{_config.Address}:{_config.Port}/v2/";
            var requests = new UnityWebRequestTaskMaker(addrBegin, _config.User, _config.Password, logger);
            _dispatcher = QueuedTaskCoroutineDispatcher.GetInstance();
            _dispatcher.EnqueueActionWithNotification(_initTask, "Waiting for server data...", 4);
            return new GnsProject(_config, "unity_project", requests, _dispatcher, logger);
        }

        public static void Cleanup()
        {
            ((IDisposable)_dispatcher).Dispose();
        }

        [Preserve]
        public void SetGnsConfig(string address, int port, string key)
        {
            _config.Address = address;
            _config.Port = port;
            _config.User = key;

            _initTask.IsReady = true;
        }
    }
}