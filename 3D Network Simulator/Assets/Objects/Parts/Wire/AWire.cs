using UnityEngine;

namespace Objects.Parts.Wire
{
    public abstract class AWire : MonoBehaviour
    {
        // Delegates
        public delegate void ConnectEv();

        public delegate void DisconnectEv();

        public delegate void SingleConnectEv(AWire other);

        public delegate void SingleDisconnectEv(AWire other);

        [HideInInspector] public GameObject wireRenderer;
        [HideInInspector] public AWire connectedWire;
        private bool _isAvailable = true;

        // Logic
        private GameObject _visualPlug;
        public event ConnectEv ConnectEvent;
        public event DisconnectEv DisconnectEvent;
        public event SingleConnectEv SingleConnectEvent;
        public event SingleConnectEv SingleDisconnectEvent;


        // Components
        public abstract GameObject GetSelf();
        public abstract GameObject GetHandObject();
        protected abstract GameObject GetPlug();
        public abstract Material GetWireMaterial();
        public abstract WireType GetInputType();
        public abstract WireType GetOutputType();
        public abstract int GetPortNumber();

        // Methods
        public GameObject GetParent()
        {
            return gameObject.transform.parent.gameObject;
        }

        public void Connect(AWire other)
        {
            _isAvailable = false;
            connectedWire = other;
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

        public void VisualConnect()
        {
            _visualPlug = GetPlug();
            _visualPlug.transform.parent = gameObject.transform;
            _visualPlug.transform.localScale = new Vector3(1, 1, 1);
            _visualPlug.transform.localPosition = GetPlugOffset();
            _visualPlug.transform.localRotation = Quaternion.Euler(0, GetPlugRotation(), 0);
        }

        public void VisualDisconnect()
        {
            if (_visualPlug is not null)
                Destroy(_visualPlug);
            if (wireRenderer is not null)
                Destroy(wireRenderer);
        }

        public bool IsAvailable()
        {
            return _isAvailable;
        }

        public void Disconnect(AWire other)
        {
            _isAvailable = true;
            connectedWire = null;
            DisconnectEvent?.Invoke();
        }

        protected abstract int GetPlugRotation();
        protected abstract Vector3 GetPlugOffset();
    }
}