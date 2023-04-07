using UnityEngine;

namespace Wire
{
    public abstract class AWire : MonoBehaviour
    {
        // Delegates
        public delegate void ConnectEv();

        public delegate void DisconnectEv();

        public delegate void SingleConnectEv(AWire other);

        public delegate void SingleDisconnectEv(AWire other);

        [HideInInspector] public GameObject wireRenderer;
        [HideInInspector] public AWire ConnectedWire;
        protected bool isAvailable = true;

        // Logic
        protected GameObject visualPlug;
        public event ConnectEv ConnectEvent;
        public event DisconnectEv DisconnectEvent;
        public event SingleConnectEv SingleConnectEvent;
        public event SingleConnectEv SingleDisconnectEvent;


        // Components
        public abstract GameObject GetSelf();
        public abstract GameObject GetHandObject();
        public abstract GameObject GetPlug();
        public abstract Material GetWireMaterial();
        public abstract WireType GetInputType();
        public abstract WireType GetOutputType();
        public abstract int GetPortNumber();

        // Methods
        public virtual GameObject GetParent()
        {
            return gameObject.transform.parent.gameObject;
        }

        public virtual void Connect(AWire other)
        {
            isAvailable = false;
            ConnectedWire = other;
            ConnectEvent?.Invoke();
        }

        public void SingleConnect(AWire other)
        {
            SingleConnectEvent?.Invoke(other);
        }

        public void SingleDisconnect(AWire other)
        {
            SingleDisconnectEvent?.Invoke(other);
        }

        public virtual void VisualConnect()
        {
            visualPlug = GetPlug();
            visualPlug.transform.parent = gameObject.transform;
            visualPlug.transform.localScale = new Vector3(1, 1, 1);
            visualPlug.transform.localPosition = GetPlugOffset();
            visualPlug.transform.localRotation = Quaternion.Euler(0, GetPlugRotation(), 0);
        }

        public virtual void VisualDisconnect()
        {
            if (visualPlug is not null)
                Destroy(visualPlug);
            if (wireRenderer is not null)
                Destroy(wireRenderer);
        }

        public bool IsAvailable()
        {
            return isAvailable;
        }

        public virtual void Disconnect(AWire other)
        {
            isAvailable = true;
            ConnectedWire = null;
            DisconnectEvent?.Invoke();
        }

        public abstract int GetPlugRotation();
        public abstract Vector3 GetPlugOffset();
    }
}