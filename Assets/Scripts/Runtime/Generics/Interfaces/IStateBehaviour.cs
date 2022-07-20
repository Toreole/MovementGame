using System;

namespace MovementGame
{
    public interface IStateBehaviour<TOwner, TEvents> where TOwner : class where TEvents : Enum
    {
        /// <summary>
        /// Call when this state is set as active in a StateMachine.
        /// </summary>
        void OnStateEnter(TOwner o);

        /// <summary>
        /// Fixed Update
        /// </summary>
        void OnUpdate(TOwner o);

        /// <summary>
        /// The method that handles transitions to other states.
        /// </summary>
        /// <param name="o"></param>
        /// <returns>The next state to transition to. [this] if the machine shouldnt transition.</returns>
        IStateBehaviour<TOwner, TEvents> OnMoveNext(TOwner o);

        /// <summary>
        /// Callback for events specified in the TEvents enum.
        /// </summary>
        /// <param name="o">The owner statemachine</param>
        /// <param name="e">The event (enum)</param>
        /// <param name="args">Optional arguments that provide data about the event.</param>
        void OnEvent(TOwner o, TEvents e, params object[] args);
    }

    /// <summary>
    /// An empty state with no functionality.
    /// Used to avoid nulls or stop behaviour from executing.
    /// </summary>
    public class EmptyState<TOwner, TEvents> : IStateBehaviour<TOwner, TEvents> where TOwner : class where TEvents : Enum
    {
        public void OnEvent(TOwner o, TEvents e, params object[] args) { }

        public void OnUpdate(TOwner o) { }

        public void OnStateEnter(TOwner o) { }

        public IStateBehaviour<TOwner, TEvents> OnMoveNext(TOwner o) => this;
    }
}