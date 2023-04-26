using Interfaces.Logger;

namespace Logger
{
    public class VoidLogger : ILogger
    {
        public void LogDebug(string message)
        {
        }

        public void LogError(string message)
        {
        }

        public void LogWarning(string message)
        {
        }
    }
}
