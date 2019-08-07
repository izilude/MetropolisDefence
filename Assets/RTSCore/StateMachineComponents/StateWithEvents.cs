namespace Assets.RTSCore.StateMachineComponents
{
    public delegate void VoidHandler(); 

    public class StateWithEvents : IState
    {
        public event VoidHandler OnEnterEvent;
        public event VoidHandler OnExitEvent;

        public string Name { get; set; }

        public void OnEnter()
        {
            if (OnEnterEvent != null) OnEnterEvent();
        }

        public void OnExit()
        {
            if (OnExitEvent != null)
            {
                OnExitEvent();
            }
        }
    }
}
