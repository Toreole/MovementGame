using UnityEngine;

namespace MovementGame.Player
{
    /// <summary>
    /// The baseline functionality for all player states is given here.
    /// </summary>
    public abstract class PlayerState : IStateBehaviour<PlayerController, PlayerEvents>
    {
        /// <inheritdocs />
        public abstract void OnEvent(PlayerController o, PlayerEvents e, params object[] args);

        /// <summary>
        /// Update.
        /// </summary>
        public abstract void OnUpdate(PlayerController o);

        public abstract IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o);

        public abstract void OnStateEnter(PlayerController o, IStateBehaviour<PlayerController, PlayerEvents> previous);

        /// <summary>
        /// Rotates the player and the camera in the default way.
        /// </summary>
        protected void ApplyDefaultCameraRotation(PlayerController o)
        {
            o.transform.localRotation *= Quaternion.Euler(0, o.RotationSpeed * o.MouseInput.x * Time.deltaTime, 0);

            o.CameraRotation = Mathf.Clamp(o.CameraRotation + o.RotationSpeed * -o.MouseInput.y * Time.deltaTime, o.CameraMinRot, o.CameraMaxRot);
        }
    }
}