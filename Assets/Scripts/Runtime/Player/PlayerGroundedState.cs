using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerGroundedState : PlayerState
    {
        private bool isCrouching = false;

        //tbh i dont really know anymore how useful this is if im gonna check everything in OnMoveNext anyway.
        public override void OnEvent(PlayerController o, PlayerEvents e, params object[] args)
        {
            if(e == PlayerEvents.OnExitGround)
            {
                Debug.Log("Exit Ground");
            }
        }

        public override void OnUpdate(PlayerController o)
        {
            //crouch stuff.
            if (o.CrouchInput && !isCrouching) //TODO: Smoothing
            {
                o.CharacterController.height = o.CrouchHeight;
                o.CharacterController.center = new Vector3(0, o.CrouchHeight * 0.5f, 0);
                o.CameraTransform.localPosition = new Vector3(0, o.CrouchHeight - 0.15f);
                isCrouching = true;
            }
            else if (!o.CrouchInput && o.HasHeadClearence() && isCrouching) //stand back up
            {
                o.CharacterController.height = o.Height;
                o.CharacterController.center = new Vector3(0, o.Height * 0.5f, 0);
                o.CameraTransform.localPosition = new Vector3(0, o.Height - 0.15f);
                isCrouching = false;
            }

            ApplyDefaultCameraRotation(o);

            //ground movement.
            Vector3 fwDir = o.transform.forward;
            Vector3 swDir = o.transform.right;

            Vector2 input = Vector2.ClampMagnitude(o.MoveInput, 1f);

            Vector3 move = fwDir * input.y + swDir * input.x;
            move *= (isCrouching ? o.CrouchSpeed : o.MoveSpeed);

            //set the velocity buffer.
            o.Velocity = move;

            move.y = -o.Stickyness;

            o.Move(move * Time.deltaTime);
        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            if (o.IsGrounded == false)
                return new PlayerFallingState();
            if(o.JumpInput)
            {
                o.JumpInput.Consume();
                return new PlayerJumpingState();
            }
            return this;
        }

        public override void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous)
        {
            Debug.Log("Grounded State");
        }
    }
}
