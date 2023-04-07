using UnityEngine;
using Wire;

namespace Device
{
    public class SampleSwitch : MonoBehaviour
    {
        public ISwitchable powerIndicator;
        public AWire powerPort;

        public void Start()
        {
            powerPort.ConnectEvent += Enable;
            powerPort.DisconnectEvent += Disable;
        }

        public void Enable()
        {
            Debug.Log("Switch enabled!");
            powerIndicator.SwitchOn();
        }

        public void Disable()
        {
            Debug.Log("Switch disabled!");
            powerIndicator.SwitchOff();
        }
    }
}