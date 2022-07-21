using System;
using UnityEngine;

namespace MovementGame
{
    public abstract class StateMachineBehaviour<TClass, TEvents> : MonoBehaviour where TEvents : Enum where TClass : StateMachineBehaviour<TClass, TEvents>
    {
        private IStateBehaviour<TClass, TEvents> activeState = new EmptyState<TClass, TEvents>();
        protected IStateBehaviour<TClass, TEvents> ActiveState
        {
            get => activeState;
            set
            {
                if (activeState != value)
                {
                    value.OnStateEnter(this as TClass, activeState);
                    activeState = value;
                }
            }
        }
    }
}
