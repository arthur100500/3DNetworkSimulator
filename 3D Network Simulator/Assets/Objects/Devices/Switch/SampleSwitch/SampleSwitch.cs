using Objects.Parts.Controllers.Scripts;
using Objects.Parts.Wire;
using UnityEngine;

namespace Objects.Devices.Switch.SampleSwitch
{
    public class SampleSwitch : MonoBehaviour
    {
        [SerializeField] private Switchable powerIndicator;
        [SerializeField] private AWire powerPort;

        public void Start()
        {
            powerPort.ConnectEvent += Enable;
            powerPort.DisconnectEvent += Disable;
        }

        private void Enable()
        {
            Debug.Log("Switch enabled!");
            powerIndicator.SwitchOn();
        }

        private void Disable()
        {
            Debug.Log("Switch disabled!");
            powerIndicator.SwitchOff();
        }
    }
}