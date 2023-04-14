using GNS3.GNSConsole;
using GNS3.ProjectHandling.Node;
using GNS3.ProjectHandling.Project;
using Objects.Devices.Common.ADevice;
using Objects.Devices.Common.ConsoleDevice;
using Objects.Parts.Wire;
using UI.Console;
using UI.Terminal;
using UnityEngine;

namespace Objects.Devices.PC.Laptop
{
    public class Laptop : AConsoleDevice
    {
        public AWire ethernetPort;

        [SerializeField] private GameObject uiTerminalPrefab;
        [SerializeField] private Canvas parentCanvas;
        [SerializeField] private Canvas screenCanvas;

        private IEventConsole _console;
        private TerminalManager _uiTerminal;

        public void Start()
        {
            _uiTerminal = Instantiate(uiTerminalPrefab, parentCanvas.transform).GetComponent<TerminalManager>();
            _uiTerminal.Initialize(screenCanvas);

            Node = new GnsVpcsNode(GlobalGnsParameters.GetProject(), "PC" + GlobalGnsParameters.GetNextFreeID());
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

        public override void OpenConsole()
        {
            if (!Node.IsReady)
            {
                GlobalNotificationManager.AddMessage("[<color=red>FL</color>] Can't connect to " + Node.Name +
                                                     " as it is not loaded");
                return;
            }
            
            if (!Node.IsStarted)
            {
                GlobalNotificationManager.AddMessage("[<color=red>FL</color>] Can't connect to " + Node.Name +
                                                     " as it is not started");
                return;
            }

            if (_console is null)
            {
                _console = Node.GetTerminal();
                _uiTerminal.LinkTo(_console);
                _uiTerminal.SetTitle(Node.Name);
            }

            _uiTerminal.Show();
        }
    }
}