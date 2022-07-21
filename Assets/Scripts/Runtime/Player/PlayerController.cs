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

        [Header("Camera")]
        [SerializeField]
        private Transform cameraTransform;
        [SerializeField]
        private float rotationSpeed = 20f;

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

        //other properties for states.
        internal CharacterController CharacterController => characterController;
        internal Transform CameraTransform => cameraTransform;
        internal float MoveSpeed => moveSpeed;
        internal float CrouchSpeed => crouchSpeed;
        internal float SprintSpeed => sprintSpeed;
        internal float Stickyness => stickyness;
        internal float Height => height;
        internal float CrouchHeight => crouchHeight;


        internal BufferedInput JumpInput => jumpInput;

        //runtime variables
        private float cameraRotation = 0;

        private bool isGrounded = true;

        internal bool IsGrounded => isGrounded;

        //--Unity Messages

        private void Start()
        {
            ActiveState = new PlayerGroundedState();
        }

        private void Update()
        {
            transform.localRotation *= Quaternion.Euler(0, rotationSpeed * mouseInput.x * Time.deltaTime, 0);

            cameraRotation = Mathf.Clamp(cameraRotation + rotationSpeed * -mouseInput.y * Time.deltaTime, -90f, 90f);
            cameraTransform.localRotation = Quaternion.Euler(cameraRotation, 0, 0);

            ActiveState.OnUpdate(this);
            ActiveState = ActiveState.OnMoveNext(this);
        }

        private void FixedUpdate()
        {
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
            bool ground = fl.HasFlag(CollisionFlags.Below);
            //handle enter and exist ground.
            if (ground && !isGrounded)
                ActiveState.OnEvent(this, PlayerEvents.OnEnterGround);
            else if (!ground && isGrounded)
                ActiveState.OnEvent(this, PlayerEvents.OnExitGround);
            isGrounded = ground;
        }

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
        OnGainedControl = 4
    }
}