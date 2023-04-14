using GNS3.GNSConsole.WebSocket;

namespace GNS3.GNSConsole
{
    public interface IEventConsole
    {
        public void AddOnOpenListener(WebSocketOpenEventHandler action);
        public void AddOnCloseListener(WebSocketCloseEventHandler action);
        public void AddOnMessageListener(WebSocketMessageEventHandler action);
        public void AddOnErrorListener(WebSocketErrorEventHandler action);
        public void SendMessage(string message);
        public WebSocketState GetState();
    }
}