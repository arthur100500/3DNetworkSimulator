using GNSHandling;
using UI.NotificationConsole;
using UI.Terminal;
using UnityEngine;
using Wire;

namespace Device
{
    public class Laptop : AConsoleDevice
    {
        public AWire ethernetPort;

        [SerializeField] private GameObject UITerminalPrefab;
        [SerializeField] private Canvas ParentCanvas;
        [SerializeField] private Canvas ScreenCanvas;
        private GNSConsole.GNSConsole console;
        private TerminalManager UITerminal;

        public void Start()
        {
            UITerminal = Instantiate(UITerminalPrefab, ParentCanvas.transform).GetComponent<TerminalManager>();
            UITerminal.Initialize(ScreenCanvas);

            Node = new GNSVPCSNode(GlobalGNSParameters.GetProject(), "VPCS" + GlobalGNSParameters.GetNextFreeID());
            ethernetPort.SingleConnectEvent += Connect;
            ethernetPort.SingleDisconnectEvent += Disconnect;
            Node.Start();
        }

        private void Connect(AWire other)
        {
            Node.ConnectTo(other.GetParent().GetComponent<ADevice>().Node, ethernetPort.GetPortNumber(),
                other.GetPortNumber());
        }

        private void Disconnect(AWire other)
        {
            Node.DisconnectFrom(other.GetParent().GetComponent<ADevice>().Node, ethernetPort.GetPortNumber(),
                other.GetPortNumber());
        }

        public override void OpenTelnet()
        {
            if (!Node.IsReady)
            {
                GlobalNotificationManager.AddMessage("[<color=red>FAIL</color>]Can't connect to " + Node.Name +
                                                     " as it is not loaded");
                return;
            }

            if (console is null)
            {
                console = new GNSConsole.GNSConsole(Node);
                UITerminal.LinkTo(console);
                UITerminal.SetTitle("Terminal: " + Node.Name);
            }

            UITerminal.Show();
        }
    }
}