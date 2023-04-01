using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Device;

namespace Player
{
    public class PlayerConsoleInteract : MonoBehaviour
    {
        [SerializeField] private LayerMask PortLayer;
        public void Update()
        {
            if (!Input.GetKeyDown(KeyCode.E))
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, PortLayer) && hit.collider != null)
            {
                var RaycastReturn = hit.collider.gameObject.name;
                var FoundObject = GameObject.Find(RaycastReturn);

                FoundObject.GetComponent<AConsoleDevice>().OpenTelnet();
            }
        }
    }
}