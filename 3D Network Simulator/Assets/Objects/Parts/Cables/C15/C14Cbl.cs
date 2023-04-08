using Objects.Parts.Wire;
using UnityEngine;

namespace Objects.Parts.Cables.C15
{
    public class C14Cbl : AWire
    {
        [SerializeField] private GameObject plug;
        [SerializeField] private GameObject c14Model;
        [SerializeField] private Material cableMaterial;

        public override int GetPortNumber()
        {
            return 0;
        }

        public override GameObject GetHandObject()
        {
            return Instantiate(plug);
        }

        protected override GameObject GetPlug()
        {
            return Instantiate(c14Model);
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

        protected override int GetPlugRotation()
        {
            return 180;
        }

        protected override Vector3 GetPlugOffset()
        {
            return new Vector3(0, 0, 0.08f);
        }
    }
}