﻿using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerGroundedState : PlayerState
    {
        //tbh i dont really know anymore how useful this is if im gonna check everything in OnMoveNext anyway.
        public override void OnEvent(PlayerController o, PlayerEvents e, params object[] args)
        {

        }

        public override void OnUpdate(PlayerController o)
        {
            //crouch control via input is primarily controlled by the grounded state.
            if (o.CrouchInput && !o.IsCrouching)
            {
                o.IsCrouching = true;
            }
            else if (!o.CrouchInput && o.IsCrouching) //stand back up
            {
                o.IsCrouching = false;
            }

            ApplyDefaultCameraRotation(o);

            //ground movement.
            Vector3 fwDir = o.transform.forward;
            Vector3 swDir = o.transform.right;

            Vector2 input = Vector2.ClampMagnitude(o.MoveInput, 1f);

            Vector3 move = fwDir * input.y + swDir * input.x;
            move *= (o.IsCrouching ? o.CrouchSpeed : o.MoveSpeed);

            //set the velocity buffer.
            o.Velocity = move;

            move.y = -o.Stickyness;

            o.Move(move * Time.deltaTime);
        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            if (o.IsGrounded == false)
                return new PlayerFallingState();
            if (o.SprintInput && !o.IsCrouching) //dont try running while crouching.
                return new PlayerRunningState();
            if(o.JumpInput)
            {
                o.JumpInput.Consume();
                return new PlayerJumpingState();
            }
            return this;
        }

        public override void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous)
        {

        }
    }
}
