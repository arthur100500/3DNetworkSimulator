using UnityEngine;

namespace Objects.Parts.Controllers.Scripts
{
    public abstract class Switchable : MonoBehaviour
    {
        public abstract void SwitchOn();
        public abstract void SwitchOff();

        public void Switch(bool s)
        {
            if (s)
                SwitchOn();
            else
                SwitchOff();
        }
    }
}