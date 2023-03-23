using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ISwitchable : MonoBehaviour
{
    public abstract void SwitchOn();
    public abstract void SwitchOff();


    public virtual void Switch(bool s)
    {
        if (s)
            SwitchOn();
        else
            SwitchOff();
    }
}
