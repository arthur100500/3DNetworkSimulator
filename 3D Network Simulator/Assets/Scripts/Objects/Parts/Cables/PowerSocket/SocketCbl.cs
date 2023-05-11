using Objects.Parts.Wire;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects.Parts.Cables.PowerSocket
{
    public class SocketCbl : AWire
    {
        public GameObject plug;
        [FormerlySerializedAs("c14model")] public GameObject c14Model;
        public Material cableMaterial;

        public override int GetPortNumber()
        {
            return 0;
        }

        public override GameObject GetHandObject()
        {
            return Instantiate(c14Model);
        }

        protected override GameObject GetPlug()
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

        protected override int GetPlugRotation()
        {
            return 180;
        }

        protected override Vector3 GetPlugOffset()
        {
            return new Vector3(0, 0, -0.01f);
        }
    }
}