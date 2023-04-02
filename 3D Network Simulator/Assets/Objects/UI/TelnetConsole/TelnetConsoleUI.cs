using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Telnet;
using TMPro;
using System.Linq;

namespace UI.TelnetConsole
{
    public class TelnetConsoleUI : MonoBehaviour
    {
        private TelnetConnection connection;
        [SerializeField] private TextMeshProUGUI message;
        private List<string> prev = new();
        private string messagesConcat = "";
        private readonly int maxLen = 22;

        // Start is called before the first frame update
        void Start()
        {
            connection = new("localhost", 5000);
            connection.Open();
            //connection.ReadStringEvent += GetTelnetMessage;
        }

        void Update()
        {

        }

        private void RenderMessages()
        {
            //Debug.Log(prev.Count);
            message.text = messagesConcat;
        }

        // private void GetTelnetMessage(string message)
        // {
        //     prev.Add(message);
        //     Debug.Log(message);
        // }

        public void SendTelnetMessage(string message)
        {
            Debug.Log("Sent: " + message);
            Debug.Log(connection);

            var res = connection.Send(message);
            prev.AddRange(res.Split("\n"));
            messagesConcat += res;
            Debug.Log(messagesConcat);

            RenderMessages();
        }
    }
}