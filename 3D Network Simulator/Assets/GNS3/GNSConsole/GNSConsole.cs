using System.Text;
using GNS3.GNSConsole.WebSocket;
using GNS3.ProjectHandling.Node;

namespace GNS3.GNSConsole
{
    public class GnsConsole : IEventConsole
    {
        private WebSocket.WebSocket _socket;
        private string _url;

        public GnsConsole(GnsNode node)
        {
            Open(node);
        }

        public void AddOnOpenListener(WebSocketOpenEventHandler action)
        {
            _socket.OnOpen += action;
        }

        public void AddOnCloseListener(WebSocketCloseEventHandler action)
        {
            _socket.OnClose += action;
        }

        public void AddOnMessageListener(WebSocketMessageEventHandler action)
        {
            _socket.OnMessage += action;
        }

        public void AddOnErrorListener(WebSocketErrorEventHandler action)
        {
            _socket.OnError += action;
        }

        public void SendMessage(string message)
        {
            _socket.Send(Encoding.ASCII.GetBytes(message));
        }

        private void Open(GnsNode node)
        {
            _url = node.GnsWsUrl + "/console/ws";

            _socket = WebSocketFactory.CreateInstance(_url);

            _socket.Connect();
        }
    }
}