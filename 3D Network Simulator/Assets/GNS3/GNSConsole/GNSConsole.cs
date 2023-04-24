using System.Text;
using GNS3.GNSConsole.WebSocket;
using UnityEngine;
using ILogger = Interfaces.Logger;

namespace GNS3.GNSConsole
{
    public class GnsConsole : IEventConsole
    {
        private WebSocket.WebSocket _socket;
        private string _url;
        private ILogger.ILogger _logger;
        public GnsConsole(string url, ILogger.ILogger logger)
        {
            _logger = logger;
            Open(url);
            _socket.OnMessage += msg => Debug.Log("Socket got: " + Encoding.ASCII.GetString(msg));
        }

        public void AddOnOpenListener(WebSocketOpenEventHandler action)
        {
            _socket.OnOpen += () =>
            {
                _logger.LogDebug("Opened WS");
                action();
            };
        }

        public void AddOnCloseListener(WebSocketCloseEventHandler action)
        {
            _socket.OnClose += action;
        }

        public void AddOnMessageListener(WebSocketMessageEventHandler action)
        {
            Debug.Log("Linked action to WS Message!");
            _socket.OnMessage += action;
        }

        public void AddOnErrorListener(WebSocketErrorEventHandler action)
        {
            
            _socket.OnError += action;
        }

        public void SendMessage(string message)
        {
            _logger.LogDebug($"Sent message of length {message.Length}");
            _socket.Send(Encoding.ASCII.GetBytes(message));
        }

        public WebSocketState GetState()
        {
            return _socket.GetState();
        }

        private void Open(string url)
        {
            _url = url;

            _socket = WebSocketFactory.CreateInstance(_url);
        }

        public void Connect()
        {
            _socket.Connect();
        }
    }
}