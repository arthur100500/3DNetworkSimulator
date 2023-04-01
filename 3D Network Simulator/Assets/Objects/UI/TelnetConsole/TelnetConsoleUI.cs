using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telnet;

namespace UI.TelnetConsole
{
    public class TelnetConsoleUI : MonoBehaviour
    {
        private TelnetConnection connection;
        private List<string> prev;

        // Start is called before the first frame update
        void Start()
        {
            connection = new("localhost", 5000);
            connection.Open();
            connection.ReadStringEvent += GetTelnetMessage;
        }

        void Update()
        {

        }

        private void GetTelnetMessage(string message)
        {
            prev.Add(message);
            Debug.Log(message);
        }

        public void SendTelnetMessage(string message)
        {
            Debug.Log("Sent: " + message);

            connection.Send(message);
        }
    }
}