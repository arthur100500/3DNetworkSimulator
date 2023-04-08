using Objects.Parts.Controllers.Scripts;
using UnityEngine;

namespace Objects.Parts.Controllers.Light
{
    public class Light : Switchable
    {
        public Material lit;
        public Material unlit;

        private MeshRenderer _renderer;

        public void Start()
        {
            _renderer = gameObject.GetComponent<MeshRenderer>();
        }

        public override void SwitchOff()
        {
            _renderer.material = unlit;
        }

        public override void SwitchOn()
        {
            _renderer.material = lit;
        }
    }
}