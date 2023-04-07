using UnityEngine;

namespace Player
{
    public class PlayerCam : MonoBehaviour
    {
        public float sens;
        public Transform orientation;
        private PlayerMovement playerMovement;

        private float xRot;
        private float yRot;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (!playerMovement.InControl) return;
            var mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
            var mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

            yRot += mouseX;
            xRot -= mouseY;

            xRot = Mathf.Clamp(xRot, -90, 90);


            transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
            orientation.rotation = Quaternion.Euler(0, yRot, 0);
        }
    }
}