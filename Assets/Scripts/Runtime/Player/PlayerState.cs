using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovementGame.Player
{
    /// <summary>
    /// The baseline functionality for all player states is given here.
    /// </summary>
    public abstract class PlayerState : IStateBehaviour<PlayerController, PlayerEvents>
    {
        /// <inheritdocs />
        public abstract void OnEvent(PlayerController o, PlayerEvents e, params object[] args);

        public abstract void OnFixedUpdate(PlayerController o);

        public abstract IStateBehaviour<PlayerController, PlayerEvents> OnMoveNext(PlayerController o);

        public abstract void OnStateEnter(PlayerController o);
    }
}