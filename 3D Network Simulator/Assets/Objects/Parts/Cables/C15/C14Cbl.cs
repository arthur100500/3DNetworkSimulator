using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Wire
{
    public class C14Cbl : AWire
    {
        public GameObject plug;
        public GameObject c14model;
        public Material cableMaterial;
        public override int GetPortNumber() => 0;
        public override GameObject GetHandObject()
        {
            return Instantiate(plug);
        }

        public override GameObject GetPlug()
        {
            return Instantiate(c14model);
        }

        public override GameObject GetSelf()
        {
            return gameObject;
        }

        public override Material GetWireMaterial()
        {
            return cableMaterial;
        }

        public override WireType GetInputType()
        {
            return WireType.WireC14;
        }

        public override WireType GetOutputType()
        {
            return WireType.WireSocketPlug;
        }

        public override int GetPlugRotation() => 180;
        public override Vector3 GetPlugOffset() => new(0, 0, 0.08f);
    }
}