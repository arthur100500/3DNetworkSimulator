using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Device
{
    public abstract class AConsoleDevice : ADevice
    {
        public abstract void OpenTelnet();
    }
}