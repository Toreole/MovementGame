using System;
using UnityEngine;

namespace MovementGame.Player
{
    public class PlayerSlideState : PlayerGroundedState
    {
        private float startTime; 

        public override void OnEvent(PlayerController o, PlayerEvents e, params object[] args)
        {

        }

        public override IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o)
        {
            //not on ground anymore.
            if (!o.IsGrounded)
                return new PlayerFallingState();
            //slide is over
            if (Time.time - startTime >= o.SlideDuration)
            {
                Debug.Log("slide over time");
                //at the end of the slide.
                o.IsCrouching = false;
                if(o.IsCrouching)
                    return new PlayerGroundedState();
                //if not crouching anymore, go to next state depending on input given.
                return o.SprintInput ? new PlayerRunningState() : new PlayerGroundedState();
            }
            return this;
        }

        public override void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous)
        {
            Debug.Log("Slide");
            //start crouching
            o.IsCrouching = true;
            //lock rotation to the one youre running in.
            o.transform.localRotation = Quaternion.LookRotation(o.Velocity, Vector3.up);
            //record start time.
            startTime = Time.time;
        }

        public override void OnUpdate(PlayerController o)
        {
            Vector3 vel = o.Velocity;
            vel *= Time.deltaTime;
            vel.y = -o.Stickyness;
            o.Move(vel);
        }
    }
}
