using UnityEngine;

namespace Wire
{
    public class EthernetCbl : AWire
    {
        public GameObject en;
        public Material cableMaterial;
        public int portID;

        public override int GetPortNumber()
        {
            return portID;
        }

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

        public override int GetPlugRotation()
        {
            return 0;
        }

        public override Vector3 GetPlugOffset()
        {
            return new Vector3(-0.007F, -0.0015F, 0.001F);
        }

        public override GameObject GetSelf()
        {
            return gameObject;
        }

        public override Material GetWireMaterial()
        {
            return cableMaterial;
        }
    }
}