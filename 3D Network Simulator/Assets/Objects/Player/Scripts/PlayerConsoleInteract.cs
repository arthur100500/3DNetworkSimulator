using Device;
using UnityEngine;

namespace Player
{
    public class PlayerConsoleInteract : MonoBehaviour
    {
        [SerializeField] private LayerMask PortLayer;

        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E))
                return;

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, PortLayer) && hit.collider != null)
            {
                var FoundObject = hit.collider.gameObject;

                FoundObject.GetComponent<AConsoleDevice>().OpenTelnet();
            }
        }
    }
}