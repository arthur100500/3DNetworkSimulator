using Objects.Devices.Common.ConsoleDevice;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects.Player.Scripts
{
    public class PlayerConsoleInteract : MonoBehaviour
    {
        [FormerlySerializedAs("PortLayer")] [SerializeField]
        private LayerMask portLayer;

        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E))
                return;

            if (_camera == null) return;
            var ray = _camera.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, portLayer) || hit.collider == null) return;

            var foundObject = hit.collider.gameObject;

            foundObject.GetComponent<AConsoleDevice>().OpenConsole();
        }
    }
}