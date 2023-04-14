using System.Collections;
using System.Collections.Generic;
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
                Debug.Log("Dequeued task");
                _currentTask.Start(); 
                yield return _currentTask.DoWork();
                _currentTask.Finish();
                Debug.Log("Finished task");
            }
        }
    }
}