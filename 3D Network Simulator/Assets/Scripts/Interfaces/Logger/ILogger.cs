namespace Interfaces.Logger
{
    public interface ILogger
    {
        public void LogDebug(string message);
        public void LogError(string message);
        public void LogWarning(string message);
    }
}
