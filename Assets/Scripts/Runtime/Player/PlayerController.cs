using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MovementGame.Input;
using System;

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
        private float slideDuration = 2f;
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
        [SerializeField]
        private float crouchTransTime = 0.15f;
        [SerializeField]
        private AnimationCurve crouchTransCurve;

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
        internal float SlideDuration => slideDuration;
        internal int CollisionMask => collisionMask;

        /// <summary>
        /// Set checks value change, then automatically adjusts the height of the character controller.
        /// </summary>
        internal bool IsCrouching
        {
            get => isCrouching;
            set
            {
                if(isCrouching && !value) //stop crouching
                {
                    if(HasHeadClearence())
                    {
                        CharacterController.height = Height;
                        CharacterController.center = new Vector3(0, Height * 0.5f, 0);
                        //CameraTransform.localPosition = new Vector3(0, Height - 0.15f);
                        isCrouching = value;
                    }
                }
                else if (!isCrouching && value) //start crouching
                {
                    CharacterController.height = CrouchHeight;
                    CharacterController.center = new Vector3(0, CrouchHeight * 0.5f, 0);
                    //CameraTransform.localPosition = new Vector3(0, CrouchHeight - 0.15f);
                    isCrouching = value;
                }
            }
        }

        //runtime variables
        private float cameraRotation = 0;

        private bool isGrounded = true;
        private bool isCrouching = false;

        private Collider[] colliderBuffer = new Collider[8];

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
            AnimateCamera();
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
            float radius = characterController.radius;
            Vector3 pos = transform.position + new Vector3(0, radius + 0.05f);

            if (Physics.SphereCast(new Ray(pos, Vector3.up), radius, height - radius * 2f, collisionMask))
                return false;
            return true;
        }

        /// <summary>
        /// Checks if the character is looking at a wall (defined by near vertical collision at half height of the character)
        /// </summary>
        /// <returns></returns>
        internal bool IsLookingAtWall()
        {
            Vector3 bodyCenter = transform.position + characterController.center;
            Vector3 lookDirection = transform.forward;
            float checkDistance = characterController.radius + 0.15f; //magic number alert
            if(Physics.Raycast(bodyCenter, lookDirection, out RaycastHit hit, checkDistance, collisionMask))
            {
                //collision has been found, check the normal.
                if(Vector3.Dot(lookDirection, hit.normal) <= -0.85f)
                {
                    return true;
                }
                //there is collision, but it is not recognized as a wall.
                return false; 
            }
            //no collision found at all.
            return false;
        }

        internal bool IsLookingAtWall(out Vector3 normal)
        {
            Vector3 bodyCenter = transform.position + characterController.center;
            Vector3 lookDirection = transform.forward;
            float checkDistance = characterController.radius + 0.15f; //magic number alert
            if (Physics.Raycast(bodyCenter, lookDirection, out RaycastHit hit, checkDistance, collisionMask))
            {
                normal = hit.normal;
                //collision has been found, check the normal.
                if (Vector3.Dot(lookDirection, hit.normal) <= -0.85f)
                {
                    return true;
                }
                //there is collision, but it is not recognized as a wall.
                return false;
            }
            //no collision found at all.
            normal = Vector3.zero;
            return false;
        }

        internal bool IsNextToWall(out Vector3 normal)
        {
            Vector3 bodyCenter = transform.position + characterController.center;
            Vector3 right = transform.right;

            if(Physics.SphereCast(bodyCenter, characterController.radius, right, out RaycastHit hit, 0.15f, collisionMask))
            {
                normal = hit.normal;
                return true;
            }
            if (Physics.SphereCast(bodyCenter, characterController.radius, -right, out hit, 0.15f, collisionMask))
            {
                normal = hit.normal;
                return true;
            }
            normal = Vector3.zero;
            return false;
        }

        /// <summary>
        /// Check if the wall/ledge youre standing in front of can be climbed.
        /// </summary>
        /// <returns></returns>
        internal bool IsAtClimbableLedge(out Vector3 ledgeTop)
        {
            float verticalGrabDistance = 0.45f; //magic number
            if(Velocity.sqrMagnitude > MoveSpeed*MoveSpeed && IsLookingAtWall())
            {
                verticalGrabDistance *= 1.45f; //another magic number
            }
            //the very top of the players head.
            Vector3 headTopPos = transform.position + new Vector3(0, Height);
            Vector3 downDirection = Vector3.down;
            Vector3 lookDirection = transform.forward;
            //the grabdistance (roughly arms length + modifiers from speed) + the character height.
            float checkDistance = verticalGrabDistance + Height;

            Vector3 checkOrigin = headTopPos + lookDirection * (characterController.radius + 0.15f) + new Vector3(0, verticalGrabDistance);

            if(Physics.Raycast(checkOrigin, downDirection, out RaycastHit hit, checkDistance, collisionMask))
            {
                //the hit is valid ground
                if(hit.normal.y > 0.8f)
                {
                    //validate vertical the offset.
                    //must not be handled by the charactercontroller already -> greater offset than step limit.
                    if(hit.point.y - transform.position.y > characterController.stepOffset)
                    {
                        //there is collision above it, oof.
                        if(0 < Physics.OverlapSphereNonAlloc(hit.point + new Vector3(0, 0.3f), 0.15f, colliderBuffer, collisionMask))
                        {
                            ledgeTop = Vector3.zero;
                            return false;
                        }    
                        //no collision, free to go.
                        ledgeTop = hit.point;
                        return true;
                    }
                }
            }
            ledgeTop = Vector3.zero;
            return false;
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

        /// <summary>
        /// Animates the camera in local space, essentially smoothes between the two heights when crouching.
        /// </summary>
        private void AnimateCamera()
        {
            float targetHeight = (IsCrouching? crouchHeight : height) - 0.15f;
            float fromHeight = (IsCrouching ? height : crouchHeight) - 0.15f;
            Vector3 pos = cameraTransform.localPosition;
            float il = Mathf.InverseLerp(fromHeight, targetHeight, pos.y);
            il += Time.deltaTime / crouchTransTime;
            float cv = crouchTransCurve.Evaluate(il);
            pos.y = Mathf.Lerp(fromHeight, targetHeight, cv);
            cameraTransform.localPosition = pos;
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
        OnGainedControl = 4,

        /// <summary>
        /// Called when you jump into the ceiling. (CollisionFlag.Above)
        /// </summary>
        OnHitCeiling = 5
    }
}