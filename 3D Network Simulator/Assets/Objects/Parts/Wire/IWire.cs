using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IWire : MonoBehaviour
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
    public GameObject wireRenderer;
    protected bool isAvailable = true;
    public IWire ConnectedWire;

    public virtual void Connect(IWire other)
    {
        isAvailable = false;
        ConnectedWire = other;
        ConnectEvent?.Invoke();
    }
    public virtual void VisualConnect()
    {
        visualPlug = GetPlug();
        visualPlug.transform.parent = gameObject.transform;
        visualPlug.transform.localPosition = GetPlugOffset();
        visualPlug.transform.localRotation = Quaternion.Euler(0, GetPlugRotation(), 0);
    }
    public virtual void VisualDisconnect()
    {
        if (!(visualPlug is null))
            Destroy(visualPlug);
        if (!(wireRenderer is null))
            Destroy(wireRenderer);
    }
    public bool IsAvailable() => isAvailable;
    public virtual void Disconnect(IWire other)
    {
        isAvailable = true;
        ConnectedWire = null;
        DisconnectEvent?.Invoke();
    }

    public abstract int GetPlugRotation();
    public abstract Vector3 GetPlugOffset();
}
