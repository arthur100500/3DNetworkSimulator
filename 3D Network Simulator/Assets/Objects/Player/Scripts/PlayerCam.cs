using UnityEngine;

namespace Objects.Player.Scripts
{
    public class PlayerCam : MonoBehaviour
    {
        public float sens;
        public Transform orientation;
        private PlayerMovement _playerMovement;

        private float _xRot;
        private float _yRot;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _playerMovement = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>();
        }

        private void Update()
        {
            if (!_playerMovement.InControl) return;
            var mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sens;
            var mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sens;

            _yRot += mouseX;
            _xRot -= mouseY;

            _xRot = Mathf.Clamp(_xRot, -90, 90);


            transform.localRotation = Quaternion.Euler(_xRot, _yRot, 0);
            orientation.rotation = Quaternion.Euler(0, _yRot, 0);
        }
    }
}