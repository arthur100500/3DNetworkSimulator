using System;
using System.Collections;
using System.Collections.Generic;
using UI.Console;
using Unity.VisualScripting;
using UnityEngine;

namespace GNS3.GNSThread
{
    public class QueuedTaskCoroutineDispatcher : MonoBehaviour, IQueuedTaskDispatcher, ISingleton, IDisposable
    {
        private static QueuedTaskCoroutineDispatcher _instance;
        private readonly Queue<IQueuedTask<object>> _tasks;
        private IQueuedTask<object> _currentTask;
        private bool _running;

        private QueuedTaskCoroutineDispatcher()
        {
            _tasks = new Queue<IQueuedTask<object>>();
            Debug.Log("Queued Task Coroutine Dispatcher created");
        }

        public void EnqueueAction(IQueuedTask<object> action)
        {
            _tasks.Enqueue(action);
        }

        public void EnqueueActionWithNotifications(IQueuedTask<object> action, string onStart, string onEnd, float delay)
        {
            _tasks.Enqueue(action);
        }

        public void EnqueueActionWithNotification(IQueuedTask<object> action, string notification, float delay)
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

        public static QueuedTaskCoroutineDispatcher GetInstance()
        {
            if (_instance is not null) return _instance;
            var gameObject = Instantiate(new GameObject());
            _instance = gameObject.AddComponent<QueuedTaskCoroutineDispatcher>();
            _instance.Run();
            return _instance;
        }

        private IEnumerator WorkCoroutine()
        {
            while (_running)
            {
                if (_tasks.Count == 0 || (_currentTask is not null && _currentTask.IsRunning))
                {
                    yield return new WaitForSeconds(0.1f);
                    continue;
                }

                _currentTask = _tasks.Dequeue();

                if (_currentTask.NotificationOnStart != "")
                    GlobalNotificationManager.AddLoadingMessage(_currentTask.NotificationOnStart, _currentTask.Guid);

                _currentTask.Start();
                yield return _currentTask.DoWork();

                _currentTask.Finish();

                if (_currentTask.NotificationOnSuccess != "" && _currentTask.IsSuccessful)
                    GlobalNotificationManager.AddLoadingMessage(_currentTask.NotificationOnSuccess, _currentTask.Guid);

                if (_currentTask.NotificationOnError != "" && !_currentTask.IsSuccessful)
                    GlobalNotificationManager.AddLoadingMessage(_currentTask.NotificationOnError, _currentTask.Guid);

                yield return new WaitForSeconds(0.1f);
                GlobalNotificationManager.StartRemovingMessage(_currentTask.Guid, 4);
            }
        }

        public void Dispose()
        {
            _tasks.Clear();
        }
    }
}