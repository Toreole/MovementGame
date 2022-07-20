using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerGroundedState : PlayerState
    {
        public override void OnEvent(PlayerController o, PlayerEvents e, params object[] args)
        {
            //
        }

        public override void OnUpdate(PlayerController o)
        {
            Vector3 fwDir = o.transform.forward;
            Vector3 swDir = o.transform.right;

            Vector2 input = Vector2.ClampMagnitude(o.MoveInput, 1f);

            Vector3 move = fwDir * input.y + swDir * input.x;
            move *= (Time.deltaTime * o.MoveSpeed);

            o.CharacterController.Move(move);
        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            return this;
        }

        public override void OnStateEnter(PlayerController o)
        {
            Debug.Log("Grounded State");
        }
    }
}
