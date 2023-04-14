using System.Collections;
using System.Collections.Generic;
using UI.Console;
using Unity.VisualScripting;
using UnityEngine;

namespace GNS3.GNSThread
{
    public class QueuedTaskCoroutineDispatcher : MonoBehaviour, IQueuedTaskDispatcher, ISingleton
    {
        private readonly Queue<IQueuedTask> _tasks;
        private bool _running;
        private static QueuedTaskCoroutineDispatcher _instance;
        private IQueuedTask _currentTask;
        public static QueuedTaskCoroutineDispatcher GetInstance()
        {
            if (_instance is not null) return _instance;
            var gameObject = Instantiate(new GameObject());
            _instance = gameObject.AddComponent<QueuedTaskCoroutineDispatcher>();
            _instance.Run();
            return _instance;
        }
        
        public QueuedTaskCoroutineDispatcher()
        {
            _tasks = new Queue<IQueuedTask>();
        }
        
        public void EnqueueAction(IQueuedTask action)
        {
            _tasks.Enqueue(action);
        }

        public void EnqueueActionWithNotifications(IQueuedTask action, string onStart, string onEnd, float delay)
        {
            _tasks.Enqueue(action);
        }

        public void EnqueueActionWithNotification(IQueuedTask action, string notification, float delay)
        {
            action.NotificationOnStart = "[..] " + notification;
            action.NotificationOnSuccess = "[<color=green>OK</color>] " + notification;
            action.NotificationOnError = "[<color=red>FL</color>] " + notification;

            _tasks.Enqueue(action);
        }

        public void Run()
        {
            _running = true;
            StartCoroutine(WorkCoroutine());
        }

        public void Stop()
        {
            _running = false;
            StopCoroutine(WorkCoroutine());
        }
        
        private IEnumerator WorkCoroutine()
        {
            while(_running)
            {
                if(_tasks.Count == 0 || (_currentTask is not null && _currentTask.IsRunning))
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }
                _currentTask = _tasks.Dequeue();
                
                if (_currentTask.NotificationOnStart != "")
                    GlobalNotificationManager.AddMessage(_currentTask.NotificationOnStart);
                
                _currentTask.Start(); 
                yield return _currentTask.DoWork();
                
                _currentTask.Finish();
                
                if (_currentTask.NotificationOnSuccess != "" && _currentTask.IsSuccessful)
                    GlobalNotificationManager.AddMessage(_currentTask.NotificationOnSuccess);
                
                if (_currentTask.NotificationOnError != "" && !_currentTask.IsSuccessful)
                    GlobalNotificationManager.AddMessage(_currentTask.NotificationOnError);
            }
        }
    }
}