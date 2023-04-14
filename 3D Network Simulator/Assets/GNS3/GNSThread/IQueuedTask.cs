using UnityEngine;

namespace GNS3.GNSThread
{
    public interface IQueuedTask
    {
        public bool IsRunning { get; }
        public void Start();
        public void Finish();
        public AsyncOperation DoWork();
    }
}