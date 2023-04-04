using System.Text;
using System.Collections.Generic;
using UnityEngine;
using GNSHandling;
using HybridWebSocket;

namespace GNSConsole
{
    public class GNSConsole
    {
        //v2/projects/f922c93a-5cc5-43b1-b2fb-1f8b3bb92bff/nodes/6894c0b2-7fb5-481c-9f97-7ff63fac7827/console/ws
        private string url;
        private WebSocket socket;
        public GNSConsole(GNSNode node)
        {
            Open(node);
        }
        private void Open(GNSNode node)
        {
            url = node.GNSWsUrl + "/console/ws";

            socket = WebSocketFactory.CreateInstance(url);

            socket.Connect();
        }

        public void AddOnOpenListener(WebSocketOpenEventHandler action) => socket.OnOpen += action;
        public void AddOnCloseListener(WebSocketCloseEventHandler action) => socket.OnClose += action;
        public void AddOnMessageListener(WebSocketMessageEventHandler action) => socket.OnMessage += action;
        public void AddOnErrorListener(WebSocketErrorEventHandler action) => socket.OnError += action;
        public void SendMessage(string message) => socket.Send(Encoding.ASCII.GetBytes(message));
    }
}
