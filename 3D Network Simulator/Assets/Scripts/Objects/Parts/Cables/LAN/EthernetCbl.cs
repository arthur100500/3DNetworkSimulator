using Objects.Parts.Wire;
using UnityEngine;

namespace Objects.Parts.Cables.LAN
{
    public class EthernetCbl : AWire
    {
        [SerializeField] private GameObject en;
        [SerializeField] private Material cableMaterial;
        [SerializeField] private int portID;

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

        protected override GameObject GetPlug()
        {
            return Instantiate(en);
        }

        protected override int GetPlugRotation()
        {
            return 0;
        }

        protected override Vector3 GetPlugOffset()
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