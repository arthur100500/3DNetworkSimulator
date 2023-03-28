using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EthernetCbl : IWire
{
    public GameObject en;
    public Material cableMaterial;
    public override GameObject GetHandObject()
    {
        return Instantiate(en);
    }

    public override WireType GetInputType()
    {
        return WireType.Ethernet;
    }

    public override WireType GetOutputType()
    {
        return WireType.Ethernet;
    }

    public override GameObject GetPlug()
    {
        return Instantiate(en);
    }

    public override int GetPlugRotation() => 0;
    public override Vector3 GetPlugOffset() => new Vector3(-0.007F, -0.0015F, 0.001F);

    public override GameObject GetSelf()
    {
        return gameObject;
    }

    public override Material GetWireMaterial()
    {
        return cableMaterial;
    }
}
