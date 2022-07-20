using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using MovementGame.Input;

namespace MovementGame.Player
{
    /// <summary>
    /// The PlayerController for this game.
    /// </summary>
    public class PlayerController : StateMachineBehaviour<PlayerController, PlayerEvents>
    {
        private void Update()
        {
            ActiveState = ActiveState.OnMoveNext(this);
        }

        private void FixedUpdate()
        {
            ActiveState.OnFixedUpdate(this);
        }
    }

    public enum PlayerEvents
    {
        NONE = 0,
        /// <summary>
        /// Called when entering ground.
        /// args: otherCollider, groundNormal
        /// </summary>
        OnEnterGround = 1,

        /// <summary>
        /// Called when leaving ground.
        /// </summary>
        OnExitGround = 2,

        /// <summary>
        /// Called when input is disabled.
        /// </summary>
        OnLostControl = 3,

        /// <summary>
        /// Called when input is enabled.
        /// </summary>
        OnGainedControl = 4,

        /// <summary>
        /// Called when the game is paused.
        /// </summary>
        OnPause = 5,

        /// <summary>
        /// Called when the game is resumed.
        /// </summary>
        OnResume = 6,

    }
}