using UnityEngine;

namespace Wire
{
    public class SocketCbl : AWire
    {
        public GameObject plug;
        public GameObject c14model;
        public Material cableMaterial;

        public override int GetPortNumber()
        {
            return 0;
        }

        public override GameObject GetHandObject()
        {
            return Instantiate(c14model);
        }

        public override GameObject GetPlug()
        {
            return Instantiate(plug);
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
            return WireType.WireSocketPlug;
        }

        public override WireType GetOutputType()
        {
            return WireType.WireC14;
        }

        public override int GetPlugRotation()
        {
            return 180;
        }

        public override Vector3 GetPlugOffset()
        {
            return new Vector3(0, 0, -0.01f);
        }
    }
}