﻿using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerLedgeClimbState : PlayerState
    {
        private Vector3 ledgeTopPos;
        private Vector3 startPos;
        float progress = 0f;

        private bool IsDone { get; set; }

        public PlayerLedgeClimbState(Vector3 ledgePos)
        {
            ledgeTopPos = ledgePos;
        }

        public override void OnEvent(PlayerController o, PlayerEvents e, params object[] args)
        {
        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            if (this.IsDone) //&& o.IsGrounded // grounded check not needed as that is assumed.
            {
                o.CharacterController.enabled = true;
                return new PlayerGroundedState();
            }
            return this;
        }

        public override void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous)
        {
            o.CharacterController.enabled = false;
            startPos = o.transform.position; 
            if (Physics.Raycast(ledgeTopPos, Vector3.up, out RaycastHit hit, o.Height, o.CollisionMask))
                o.IsCrouching = true;
        }

        public override void OnUpdate(PlayerController o)
        {
            progress += Time.deltaTime * 4f;
            Vector3 nextPos = Vector3.Lerp(startPos, ledgeTopPos, progress);
            o.transform.position = nextPos;

            IsDone = progress > 0.99f;
        }

    }
}
