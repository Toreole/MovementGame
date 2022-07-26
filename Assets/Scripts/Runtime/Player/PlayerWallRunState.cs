using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerWallRunState : PlayerState
    {
        private Vector3 wallNormal;

        public PlayerWallRunState(Vector3 wallNormal)
        {
            this.wallNormal = wallNormal;
        }

        public override void OnEvent(PlayerController o, PlayerEvents e, params object[] args)
        {

        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            if(o.IsGrounded)
            {
                return o.SprintInput? new PlayerRunningState() : new PlayerGroundedState();
            }
            //check if the wall is "lost"
            if(!o.IsNextToWall(out Vector3 n))
            {
                return new PlayerFallingState();
            }
            if(o.JumpInput)
            {
                o.JumpInput.Consume();
                return new PlayerJumpingState();
            }
            return this;
        }

        public override void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous)
        {
            //preserve velocity.
            var velocity = o.Velocity;
            float sqrV = velocity.sqrMagnitude;
            //make it parallel to the wall.
            velocity = velocity - (wallNormal * Vector3.Dot(wallNormal, velocity));
            float sqrS = o.SprintSpeed * o.SprintSpeed;
            //get the velocity up to speed.
            velocity *= sqrV / sqrS;
            velocity.y = 5f;
            //re-assign velocity.
            o.Velocity = velocity;

            //align character with velocity.
            Vector3 fwd = new Vector3(velocity.x, 0, velocity.z);
            o.transform.forward = fwd;

            //set camera roll
            bool negativeR = Vector3.Dot(wallNormal, o.transform.right) > 0;
            float roll = negativeR ? -12f : 12f;
            o.CameraTransform.localRotation *= Quaternion.AngleAxis(roll, Vector3.forward);
            //offsetting the camera to be farther away from the wall. would need a way to reset it
            //o.CameraTransform.localPosition += new Vector3(negativeR ? 0.1f : -0.1f, 0);
        }

        public override void OnUpdate(PlayerController o)
        {
            var vel = o.Velocity;
            vel.y -= o.Gravity * Time.deltaTime * 0.5f;
            o.Velocity = vel;
            o.Move();
        }
    }
}
