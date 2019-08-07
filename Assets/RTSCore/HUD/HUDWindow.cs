using UnityEngine;

namespace Assets.RTSCore.HUD
{
    public abstract class HUDWindow
    {
        protected bool _show = false;
        protected Rect windowArea;
        protected GUISkin BackgroundSkin;

        public virtual bool Show
        {
            get
            {
                return _show;
            }
            set
            {
                _show = value;
            }
        }

        public HUDWindow(GUISkin background)
        {
            BackgroundSkin = background;
        }

        public void Draw()
        {
            if (!Show) { return; }

            windowArea = new Rect(Screen.width / 4, Screen.height / 8, Screen.width / 2, Screen.height * 6 / 8);

            GUI.skin = BackgroundSkin;
            GUI.BeginGroup(windowArea);

            Rect internalArea = new Rect(windowArea);
            internalArea.x = 0;
            internalArea.y = 0;

            GUI.Box(internalArea, "");
            DrawInternal(internalArea);

            GUI.EndGroup();
        }

        protected abstract void DrawInternal(Rect area);

        public bool MouseInside(Vector3 mousePos)
        {
            return windowArea.Contains(mousePos) && Show;
        }

    }
}
