namespace Assets.RTSCore.StateMachineComponents
{
    public interface IState
    {
        string Name { get; }
        void OnEnter();
        void OnExit();
        void OnUpdate();
    }
}
