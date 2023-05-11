using GNS3.ProjectHandling.Node;
using GNS3.ProjectHandling.Project;
using Objects.Devices.Common;
using Objects.Parts.Controllers.Scripts;
using Objects.Parts.Wire;
using UnityEngine;

namespace Objects.Devices.Hub.SmallHub
{
    public class SmallHub : ADevice
    {
        // Start is called before the first frame update
        [SerializeField] private AWire[] ethernetCables = new AWire[8];
        [SerializeField] private AWire power;
        [SerializeField] private Switchable powerIndicator;

        public void Start()
        {
            power.ConnectEvent += Enable;
            power.DisconnectEvent += Disable;

            foreach (var en in ethernetCables)
            {
                en.SingleConnectEvent += other =>
                    Node.ConnectTo(
                        other.GetParent().GetComponent<ADevice>().Node,
                        en.GetPortNumber(),
                        other.GetPortNumber()
                    );
                en.SingleDisconnectEvent += other =>
                    Node.DisconnectFrom(
                        other.GetParent().GetComponent<ADevice>().Node,
                        en.GetPortNumber(),
                        other.GetPortNumber()
                    );
            }
        }

        private void Enable()
        {
            powerIndicator.SwitchOn();
            Node.Start();
        }

        private void Disable()
        {
            powerIndicator.SwitchOff();
            Node.Stop();
        }
    }
}