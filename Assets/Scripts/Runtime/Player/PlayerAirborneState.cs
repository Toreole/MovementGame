﻿using UnityEngine;

namespace MovementGame.Player
{
    public abstract class PlayerAirborneState : PlayerState
    {
        protected void ApplyGravity(PlayerController o)
        {
            Vector3 vel = o.Velocity;
            vel.y -= o.Gravity * Time.deltaTime;
            o.Velocity = vel;
        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            if (o.IsGrounded)
                return new PlayerGroundedState();
            return this;
        }
    }
}