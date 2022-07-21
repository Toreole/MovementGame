using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MovementGame.Input;

namespace MovementGame.Player
{
    /// <summary>
    /// The PlayerController for this game.
    /// </summary>
    public class PlayerController : StateMachineBehaviour<PlayerController, PlayerEvents>
    {
        [Header("Movement")]
        [SerializeField]
        private CharacterController characterController;
        [SerializeField]
        private float moveSpeed = 2f;
        [SerializeField]
        private float crouchSpeed = 1.4f;
        [SerializeField]
        private float sprintSpeed = 3.4f;
        [SerializeField]
        private LayerMask collisionMask;

        [SerializeField]
        private float height;
        [SerializeField]
        private float crouchHeight;
        [SerializeField, Tooltip("How much to stick to surfaces while grounded")]
        private float stickyness = 1f;
        [SerializeField]
        private float jumpHeight = 1.5f;
        [SerializeField]
        private float gravity = 10f;

        [Header("Camera")]
        [SerializeField]
        private Transform cameraTransform;
        [SerializeField]
        private float rotationSpeed = 20f;
        [SerializeField]
        private float cameraMinRot = -80f, cameraMaxRot = 80f;

        //--Input
        [SerializeField]
        private BufferedInput jumpInput;

        private bool sprintInput = false;
        private bool crouchInput = false;
        private Vector2 moveInput = Vector2.zero;
        private Vector2 mouseInput = Vector2.zero;

        //properties so the states can read the input
        internal Vector2 MoveInput => moveInput;
        internal bool SprintInput => sprintInput;
        internal bool CrouchInput => crouchInput;
        internal BufferedInput JumpInput => jumpInput;
        public Vector2 MouseInput => mouseInput;

        //properties for reading settings.
        internal CharacterController CharacterController => characterController;
        internal Transform CameraTransform => cameraTransform;
        internal float MoveSpeed => moveSpeed;
        internal float CrouchSpeed => crouchSpeed;
        internal float SprintSpeed => sprintSpeed;
        internal float Stickyness => stickyness;
        internal float Height => height;
        internal float CrouchHeight => crouchHeight;
        internal float CameraMinRot => cameraMinRot;
        internal float CameraMaxRot => cameraMaxRot;
        internal float RotationSpeed => rotationSpeed;
        internal float JumpVelocity { get; private set; }
        internal float Gravity => gravity;
        internal Vector3 LastGroundPos { get; private set; }

        //runtime variables
        private float cameraRotation = 0;

        private bool isGrounded = true;

        internal bool IsGrounded => isGrounded;

        /// <summary>
        /// The current angle of the camera on the local X axis. Setting this property applies the value to the cameraTransform.
        /// </summary>
        internal float CameraRotation 
        { 
            get => cameraRotation; 
            set 
            { 
                cameraRotation = value; 
                cameraTransform.localRotation = Quaternion.Euler(cameraRotation, 0, 0); 
            } 
        }

        /// <summary>
        /// A buffer for the current velocity. Does not do anything in particular.
        /// </summary>
        internal Vector3 Velocity { get; set; } = Vector3.zero;

        //--Unity Messages

        private void Start()
        {
            JumpVelocity = Mathf.Sqrt(jumpHeight * (2.0f * gravity));
            ActiveState = new PlayerGroundedState();
        }

        private void Update()
        {
            ActiveState.OnUpdate(this);
            ActiveState = ActiveState.OnMoveNext(this);
        }

        private void OnDrawGizmos()
        {
            var pos = transform.position;
            Debug.DrawLine(pos, pos + Velocity, Color.red);
            pos += new Vector3(0, Height, 0);
            Debug.DrawLine(pos, pos + Velocity, Color.red);
        }

        //--Checks
        internal bool HasHeadClearence()
        {
            bool b = Physics.Raycast(transform.position + new Vector3(0, 0.05f), transform.up, out RaycastHit hit, height, collisionMask);
            //if (b)
            //    Debug.Log($"hit col: {hit.collider.GetType().Name}", hit.collider);
            return !b;
        }

        /// <summary>
        /// Moves the character controller with the specified vector.
        /// Handles grounded state.
        /// </summary>
        /// <param name="mv">Movement Vector</param>
        internal void Move(Vector3 mv)
        {
            CollisionFlags fl = characterController.Move(mv);
            //handle hitting your head on the ceiling while jumping
            if(fl.HasFlag(CollisionFlags.Above))
                ActiveState.OnEvent(this, PlayerEvents.OnHitCeiling);

            //ground
            bool ground = fl.HasFlag(CollisionFlags.Below);
            //handle enter and exist ground.
            if (ground && !isGrounded)
                ActiveState.OnEvent(this, PlayerEvents.OnEnterGround);
            else if (!ground && isGrounded)
                ActiveState.OnEvent(this, PlayerEvents.OnExitGround);
            isGrounded = ground;
            if (isGrounded)
                LastGroundPos = transform.position;
        }

        /// <summary>
        /// Moves according to .Velocity.
        /// </summary>
        internal void Move() => Move(Velocity * Time.deltaTime);

        //--Input Messages

        private void OnControlsChanged()
        {
            //unused for now.
        }

        private void OnMouseMove(InputValue iv)
        {
            mouseInput = iv.Get<Vector2>();
        }

        private void OnMoveInput(InputValue iv)
        {
            moveInput = iv.Get<Vector2>();
        }

        private void OnCrouch(InputValue iv)
        {
            crouchInput = iv.Get<float>() > 0;
        }

        private void OnSprint(InputValue iv)
        {
            sprintInput = iv.Get<float>() > 0;
        }

        private void OnJump()
        {
            jumpInput.Set();
        }

        private void OnPause()
        {

        }
    }

    public enum PlayerEvents
    {
        NONE = 0,

        /// <summary>
        /// Called when entering ground.
        /// </summary>
        OnEnterGround = 1,

        /// <summary>
        /// Called when leaving ground.
        /// </summary>
        OnExitGround = 2,

        /// <summary>
        /// Called when input is disabled.
        /// </summary>
        OnLostControl = 3,

        /// <summary>
        /// Called when input is enabled.
        /// </summary>
        OnGainedControl = 4,

        /// <summary>
        /// Called when you jump into the ceiling. (CollisionFlag.Above)
        /// </summary>
        OnHitCeiling = 5
    }
}