using UnityEngine;
using ILogger = Interfaces.Logger.ILogger;

namespace Logger
{
    public class DebugLogger : ILogger
    {
        public void LogDebug(string message)
        {
            Debug.Log("LOG: " + message);
        }

        public void LogError(string message)
        {
            Debug.Log("ERROR: " + message);
        }

        public void LogWarning(string message)
        {
            Debug.Log("WARN: " + message);
        }
    }
}
