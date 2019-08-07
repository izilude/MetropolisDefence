using System.Collections.Generic;

namespace Assets.RTSCore.StateMachineComponents
{
    public interface IStateMachine
    {
        IState CurrentState { get; }

        void MoveToState(IState nextState);
        void MoveToState(string nextStateKey);

        List<string> GetPossibleTransitions();

        void AddState(IState state);
        void RemoveState(IState state);
        void AddTransition(IState originalStateKey, IState finalStateKey);
        void RemoveTransition(IState originalStateKey, IState finalStateKey);
    }
}
