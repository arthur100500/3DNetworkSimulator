using GNSHandling;
using Wire;

namespace Device.Hub
{
    public class SmallHub : ADevice
    {
        // Start is called before the first frame update
        public AWire[] ethernets = new AWire[8];
        public AWire power;
        public ISwitchable powerIndicator;

        public void Start()
        {
            Node = new GNSSHubNode(GlobalGNSParameters.GetProject(),
                "Small Hub " + GlobalGNSParameters.GetNextFreeID());

            power.ConnectEvent += Enable;
            power.DisconnectEvent += Disable;

            foreach (var en in ethernets)
            {
                en.SingleConnectEvent += other =>
                    Node.ConnectTo(other.GetParent().GetComponent<ADevice>().Node, en.GetPortNumber(),
                        other.GetPortNumber());
                en.SingleDisconnectEvent += other =>
                    Node.DisconnectFrom(other.GetParent().GetComponent<ADevice>().Node, en.GetPortNumber(),
                        other.GetPortNumber());
            }
        }

        public void Enable()
        {
            powerIndicator.SwitchOn();
            Node.Start();
        }

        public void Disable()
        {
            powerIndicator.SwitchOff();
            Node.Stop();
        }
    }
}