using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light : ISwitchable
{
    public Material lit;
    public Material unlit;

    private new MeshRenderer renderer;

    public void Start()
    {
        renderer = gameObject.GetComponent<MeshRenderer>();
    }
    public override void SwitchOff()
    {
        renderer.material = unlit;
    }

    public override void SwitchOn()
    {
        renderer.material = lit;
    }
}
