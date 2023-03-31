using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wire
{
    public abstract class AWire : MonoBehaviour
    {
        // Delegates
        public delegate void ConnectEv();
        public event ConnectEv ConnectEvent;
        public delegate void DisconnectEv();
        public event DisconnectEv DisconnectEvent;


        // Components
        public abstract GameObject GetSelf();
        public abstract GameObject GetHandObject();
        public abstract GameObject GetPlug();
        public abstract Material GetWireMaterial();
        public abstract WireType GetInputType();
        public abstract WireType GetOutputType();

        // Logic
        protected GameObject visualPlug;
        [HideInInspector] public GameObject wireRenderer;
        protected bool isAvailable = true;
        [HideInInspector] public AWire ConnectedWire;

        public virtual void Connect(AWire other)
        {
            isAvailable = false;
            ConnectedWire = other;
            ConnectEvent?.Invoke();
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
        public bool IsAvailable() => isAvailable;
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