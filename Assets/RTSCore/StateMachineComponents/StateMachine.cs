using System;
using System.Collections.Generic;
using Assets.RTSCore.Game;
using UnityEngine;

namespace Assets.RTSCore.StateMachineComponents
{
    public class StateMachine : MonoBehaviour, IStateMachine
    {
        protected virtual void Update()
        {
            CallUpdateFunction();
        }

        public IState CurrentState { get; private set; }

        public void MoveToState(IState nextState)
        {
            if (nextState == null)
            {
                throw new Exception("Can't transition to null state!");
            }

            MoveToState(nextState.Name);
        }

        public void MoveToState(string nextStateKey)
        {
            if (!States.ContainsKey(nextStateKey))
            {
                throw new Exception(String.Format("Can't transition to state {0} as the state does not exist!", nextStateKey));
            }

            if (CurrentState == null)
            {
                IState state;
                States.TryGetValue(nextStateKey, out state);
                CurrentState = state;
                if (CurrentState != null) CurrentState.OnEnter();
            }
            else
            {
                List<string> transitions;
                if (Transitions.TryGetValue(CurrentState.Name, out transitions))
                {
                    if (transitions.Contains(nextStateKey))
                    {
                        CurrentState.OnExit();

                        IState state;
                        States.TryGetValue(nextStateKey, out state);
                        CurrentState = state;
                        if (CurrentState != null) CurrentState.OnEnter();
                        return;
                    }
                }
                throw new Exception(String.Format("Can't transition to state {0} as the state {1} doesn't have a transition to it!", nextStateKey, CurrentState.Name));
            }
        }

        public List<string> GetPossibleTransitions()
        {
            // Note, creating a new list to avoid modifying the original list outside this class.
            List<string> newList = new List<string>();

            List<string> transitions;
            if (CurrentState != null && Transitions.TryGetValue(CurrentState.Name, out transitions))
            {
                newList.AddRange(transitions);
                return newList;
            }

            return newList;
        }

        protected Dictionary<string, IState> States = new Dictionary<string, IState>();
        protected Dictionary<string, List<string>> Transitions = new Dictionary<string, List<string>>();

        public virtual void AddState(IState state)
        {
            if (States.ContainsKey(state.Name))
            {
                return;
            }

            States.Add(state.Name, state);
        }

        public void CallUpdateFunction()
        {
            if (CurrentState != null) CurrentState.OnUpdate();
        }

        public void AddState(string key, VoidHandler updateFunction)
        {
            var newState = new StateWithEvents();
            newState.OnUpdateEvent += updateFunction;
            newState.Name = key;
            AddState(newState);
        }

        public virtual void AddTransition(IState originalStateKey, IState finalStateKey)
        {
            AddTransition(originalStateKey.Name, finalStateKey.Name);
        }

        public virtual void AddTransition(string originalStateKey, string finalStateKey)
        {
            if (!States.ContainsKey(originalStateKey))
            {
                throw new Exception(String.Format("Cannot Add transition because {0} is not in this state machine", originalStateKey));
            }

            if (!States.ContainsKey(finalStateKey))
            {
                throw new Exception(String.Format("Cannot Add transition because {0} is not in this state machine", finalStateKey));
            }

            List<string> finalTransitions;
            if (Transitions.TryGetValue(originalStateKey, out finalTransitions))
            {

            }
            else
            {
                finalTransitions = new List<string>();
                finalTransitions.Add(finalStateKey);
                Transitions.Add(originalStateKey, finalTransitions);
            }
        }

        public virtual void RemoveTransition(IState originalStateKey, IState finalStateKey)
        {
            if (!States.ContainsKey(originalStateKey.Name))
            {
                return;
            }

            if (!States.ContainsKey(finalStateKey.Name))
            {
                return;
            }

            List<string> finalTransitions;
            if (Transitions.TryGetValue(originalStateKey.Name, out finalTransitions))
            {
                finalTransitions.Remove(finalStateKey.Name);
            }
        }

        public virtual void RemoveState(IState state)
        {
            if (!States.ContainsKey(state.Name))
            {
                return;
            }

            States.Remove(state.Name);
            Transitions.Remove(state.Name);
            foreach (var pair in Transitions)
            {
                pair.Value.Remove(state.Name);
            }
        }
    }
}
