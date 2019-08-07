namespace Assets.RTSCore.StateMachineComponents
{
    public class StateOverridable : IState
    {
        public string Name { get; set; }

        public virtual void OnEnter() { }

        public virtual void OnExit() { }

        public virtual void OnUpdate() { }
    }
}
