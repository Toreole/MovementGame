using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerFallingState : PlayerAirborneState
    {
        public override void OnEvent(PlayerController o, PlayerEvents e, params object[] args)
        {
            
        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            return base.OnMoveNext(o);
        }

        public override void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous)
        {
            //correct the offset caused by stickyness
            if(previous is PlayerGroundedState)
                o.CharacterController.Move(new Vector3(0, o.LastGroundPos.y - o.transform.position.y, 0));
        }

        public override void OnUpdate(PlayerController o)
        {
            ApplyDefaultCameraRotation(o);
            ApplyGravity(o);
            o.Move();
        }
    }
}
