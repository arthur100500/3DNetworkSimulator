using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleSwitch : MonoBehaviour
{
    public ISwitchable powerIndicator;
    public IWire powerPort;

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
