using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using UI.NotificationConsole;
using System;

namespace Telnet
{
    /*
    * Used to connect and exchange data via telnet
    */
    public class TelnetConnection
    {
        private readonly string address;
        private readonly int port;
        private TcpClient tcpSocket;
        private NetworkStream stream;

        public delegate void ReadStringEv(string message);
        public event ReadStringEv ReadStringEvent;

        private readonly byte[] readBuffer = new byte[65536];

        public TelnetConnection(string address, int port)
        {
            this.address = address;
            this.port = port;
        }

        public void Open()
        {
            GlobalNotificationManager.AddMessage("Opening connection to " + address + ":" + port);
            tcpSocket = new(address, port);
            stream = tcpSocket.GetStream();
            tcpSocket.NoDelay = true;
            GlobalNotificationManager.AddMessage("Opened connection to " + address + ":" + port);

            // Create event to recieve string
            AsyncCallback MyCallBack = new(DataReceived);
            stream.BeginRead(readBuffer, 0, 200, MyCallBack, readBuffer);
        }

        private void DataReceived(IAsyncResult res)
        {
            byte[] buffer = (byte[])res.AsyncState;
            ReadStringEvent?.Invoke(System.Text.Encoding.UTF8.GetString(buffer));
        }

        // Will return the response
        public void Send(string message)
        {
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
    }
}
