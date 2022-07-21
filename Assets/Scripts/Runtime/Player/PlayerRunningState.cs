using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerRunningState : PlayerGroundedState
    {
        public override void OnUpdate(PlayerController o)
        {
            ApplyDefaultCameraRotation(o);

            //ground movement.
            Vector3 fwDir = o.transform.forward;
            Vector3 swDir = o.transform.right;

            Vector2 input = Vector2.ClampMagnitude(o.MoveInput, 1f);

            Vector3 move = fwDir * input.y + swDir * input.x;

            move *= o.SprintSpeed;

            //set the velocity buffer.
            o.Velocity = move;

            move.y = -o.Stickyness;

            o.Move(move * Time.deltaTime);
        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            if (!o.IsGrounded)
                return new PlayerFallingState();
            if(o.CrouchInput)
            {
                return new PlayerSlideState();
            }
            if (!o.SprintInput)
                return new PlayerGroundedState();
            if(o.JumpInput)
            {
                o.JumpInput.Consume();
                return new PlayerJumpingState();
            }
            return this;
        }

        public override void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous)
        {
            base.OnStateEnter(o, previous);
            o.IsCrouching = false;
        }
    }
}
