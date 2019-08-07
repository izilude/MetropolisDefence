using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
