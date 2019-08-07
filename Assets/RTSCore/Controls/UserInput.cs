using UnityEngine;

namespace Assets.RTSCore.Controls 
{
    public class UserInput : MonoBehaviour
    {
        public int ScreenScrollSpeed;
        public int ScreenScrollWidth;
        public float ZoomSensitivity;
		public float MinFieldOfView;
		public float MaxFieldOfView;

        private MouseActions _mouse;
        private KeyboardActions _keyboard;

		void Start() 
		{
			_mouse = new MouseActions(ScreenScrollSpeed,
			                                      MinFieldOfView,
			                                      MaxFieldOfView,
			                                      ZoomSensitivity);

		    _keyboard = new KeyboardActions();
		}

        // Use this for initialization
        void Awake()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


        public void KeyboardActivity()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _keyboard.EscapeKeyPressed();
            }
        }

        // ---- MOUSE CONTROLS ----
        // Main Update function for all mouse activity
        // Functions declared in file MouseActions.cs
        public void MouseActivity(Level.Level level)
        {
			if (_mouse == null) 
			{
				_mouse = new MouseActions(ScreenScrollSpeed,
				                          MinFieldOfView,
				                          MaxFieldOfView,
				                          ZoomSensitivity);
			}

            // Checks to initiate mouse clicks
			if (Input.GetMouseButtonDown(0)) _mouse.LeftMouseButtonDown(level);
			else if (Input.GetMouseButtonDown(1)) _mouse.RightMouseButtonDown(level);
			else if (Input.GetMouseButtonUp(0)) _mouse.LeftMouseButtonUp(level);
			else if (Input.GetMouseButtonUp(1)) _mouse.RightMouseButtonUp(level);

			_mouse.MouseMove(level);

            // Checks to initiate screen Scrolling
            int ScreenWidth = Screen.width;
            int ScreenHeight = Screen.height;
            float x = Input.mousePosition.x;
            float y = Input.mousePosition.y;

			//Debug.Log (string.Format("{0}, {1}",x,y));

			if (x > 1 && x < ScreenScrollWidth) _mouse.MouseOnLeftEdge(level);
			else if (x > ScreenWidth - ScreenScrollWidth && x < ScreenWidth) _mouse.MouseOnRightEdge(level);
			if (y > 1 && y < ScreenScrollWidth) _mouse.MouseOnBottomEdge(level);
			else if (y > ScreenHeight - ScreenScrollWidth && y < ScreenHeight) _mouse.MouseOnTopEdge(level);

            // Mouse wheel zooming
			if (Input.GetAxis("Mouse ScrollWheel") != 0) _mouse.MouseWheel(level);

            // Miscellaneous checks
			_mouse.MouseHover(level);
        }
    }
}
