using Objects.Devices.Common.ConsoleDevice;
using UnityEngine;
using UnityEngine.Serialization;

namespace Objects.Player.Scripts
{
    public class PlayerConsoleInteract : MonoBehaviour
    {
        [FormerlySerializedAs("PortLayer")] [SerializeField]
        private LayerMask portLayer;

        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E))
                return;

            if (Camera.main == null) return;
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, portLayer) || hit.collider == null) return;

            var foundObject = hit.collider.gameObject;

            foundObject.GetComponent<AConsoleDevice>().OpenConsole();
        }
    }
}