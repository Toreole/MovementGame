using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerJumpingState : PlayerAirborneState
    {
        public override void OnEvent(PlayerController o, PlayerEvents e, params object[] args)
        {
            if(e == PlayerEvents.OnHitCeiling)
            {
                var vel = o.Velocity;
                vel.y = Mathf.Min(vel.y, -0.1f);
                o.Velocity = vel;
            }
        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            return base.OnMoveNext(o);
        }

        public override void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous)
        {
            Vector3 vel = o.Velocity;
            vel.y += o.JumpVelocity;
            o.Velocity = vel;
        }

        public override void OnUpdate(PlayerController o)
        {
            ApplyDefaultCameraRotation(o);
            ApplyGravity(o);
            o.Move();
        }
    }
}
