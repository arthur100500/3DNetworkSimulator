using UnityEngine;

namespace Objects.Player.Scripts
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")] public float moveSpeed;

        public float groundDrag;

        public float jumpForce;
        public float jumpCooldown;
        public float airMultiplier;

        [HideInInspector] public float walkSpeed;
        [HideInInspector] public float sprintSpeed;

        [Header("Key Binds")] public KeyCode jumpKey = KeyCode.Space;

        [Header("Ground Check")] public float playerHeight;

        public LayerMask ground;

        public Transform orientation;
        private bool _grounded;

        private float _horizontalInput;
        private bool _inControl = true;

        private Vector3 _moveDirection;

        private Rigidbody _rb;
        private bool _readyToJump;
        private float _verticalInput;

        public bool InControl
        {
            get => _inControl;
            set
            {
                if (value)
                    Cursor.lockState = CursorLockMode.Locked;
                else
                    Cursor.lockState = CursorLockMode.None;
                _inControl = value;
            }
        }

        public void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;

            _readyToJump = true;
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape)) InControl = !InControl;
            if (!_inControl) return;
            // ground check
            _grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);

            MyInput();
            SpeedControl();

            // handle drag
            if (_grounded)
                _rb.drag = groundDrag;
            else
                _rb.drag = 0;
        }

        public void FixedUpdate()
        {
            if (!_inControl) return;
            MovePlayer();
        }

        private void MyInput()
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal");
            _verticalInput = Input.GetAxisRaw("Vertical");

            // when to jump
            if (Input.GetKey(jumpKey) && _readyToJump && _grounded)
            {
                _readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }

        private void MovePlayer()
        {
            // calculate movement direction
            _moveDirection = orientation.forward * _verticalInput + orientation.right * _horizontalInput;

            switch (_grounded)
            {
                // on ground
                case true:
                    _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
                    break;

                // in air
                case false:
                    _rb.AddForce(_moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
                    break;
            }
        }

        private void SpeedControl()
        {
            var velocity = _rb.velocity;
            var flatVel = new Vector3(velocity.x, 0f, velocity.z);

            // limit velocity if needed
            if (!(flatVel.magnitude > moveSpeed)) return;

            var limitedVel = flatVel.normalized * moveSpeed;
            _rb.velocity = new Vector3(limitedVel.x, _rb.velocity.y, limitedVel.z);
        }

        private void Jump()
        {
            // reset y velocity
            var velocity = _rb.velocity;
            velocity = new Vector3(velocity.x, 0f, velocity.z);
            _rb.velocity = velocity;

            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        private void ResetJump()
        {
            _readyToJump = true;
        }
    }
}