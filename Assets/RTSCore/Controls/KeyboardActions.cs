namespace Assets.RTSCore.Controls
{
    public class KeyboardActions
    {
        public void EscapeKeyPressed()
        {
            Game.Game.Instance.SwitchToWorldView();
        }
    }
}
