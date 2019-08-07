using Assets.RTSCore.Misc;
using Assets.RTSCore.WorldObject;
using UnityEngine;

namespace Assets.RTSCore.Controls {
    public class MouseActions
    {
        private float _screenScrollSpeed;
		private float _minFieldOfView;
		private float _maxFieldOfView;
		private float _zoomSensitivity;

        private static Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);
        public static Vector3 InvalidPosition { get { return invalidPosition; } }

		public bool _dragging;

        public MouseActions(float scrollspeed, float minFieldOfView, float maxFieldOfView, float zoomSensitivity)
        {
            _screenScrollSpeed = scrollspeed;
			_minFieldOfView = minFieldOfView;
			_maxFieldOfView = maxFieldOfView;
			_zoomSensitivity = zoomSensitivity;
        }

		public void RightMouseButtonDown(Level.Level level)
        {

		}

		public void RightMouseButtonUp(Level.Level level)
		{
			if (level.Hud.CustomCursor) 
			{
				level.Hud.SetCustomCursor(null);
			} 
			else
			{
			    Game.Game.Instance.ActiveLevel.SetSelectedObject (level.Hud, null);
			}        
		}

		public void MouseMove(Level.Level level) 
		{
			HUDRegions region = level.Hud.MouseInRegion ();
			
			switch (region) 
			{
			case HUDRegions.Invalid :
				return;
			case HUDRegions.GameArea :
				MouseMoveGameArea(level);
				return;
			case HUDRegions.OrdersBar :
				return;
			default:
				return;
			}
		}

		public void MouseMoveGameArea(Level.Level level)
		{
		    Player.Player player = Game.Game.Instance.ActivePlayer;

            GameObject hitObject = FindHitObject();
			Vector3 hitPoint = FindHitPoint();
			
			if (level.Hud.CustomCursor && _dragging) 
			{
                BuildableWorldObject worldObject = level.Hud.CustomCursor.GetComponent<BuildableWorldObject>();
				
				if (level.Map.IsBuildable(worldObject) && Game.Game.Instance.ActiveLevel.CanBuildObject(worldObject)) 
				{
				    Game.Game.Instance.ActiveLevel.AddWorldObject(worldObject, false);
                    level.Map.ChangeTileType(worldObject);
                    level.Map.MakeOccupyingTilesUnavailable(worldObject);
				}
			}
		}

		public void LeftMouseButtonDown(Level.Level level)
        {
			_dragging = true;
        }
		
		public void LeftMouseButtonUp(Level.Level level)
		{
			_dragging = false;
			HUDRegions region = level.Hud.MouseInRegion ();
			
			switch (region) 
			{
			    case HUDRegions.Invalid :
				    return;
			    case HUDRegions.GameArea :
                    level.Hud.InventoryManager.Show = false;
                    level.Hud.ProductionManager.Show = false;
                    level.GalacticMarketBoard.Show = false;
                    LeftMouseButtonUpGameArea(level);
                    return;
			    case HUDRegions.OrdersBar :
                    level.GalacticMarketBoard.Show = false;
                    LeftMouseButtonUpOrdersBar(level);
                    return;
                case HUDRegions.InventoryManager:
                    level.GalacticMarketBoard.Show = false;
                    return;
                case HUDRegions.GalacticMarketBoard:
                    level.Hud.InventoryManager.Show = false;
                    level.Hud.ProductionManager.Show = false;
                    return;
                case HUDRegions.ProductionManager:
                    level.GalacticMarketBoard.Show = false;
                    level.Hud.InventoryManager.Show = false;
                    return;
                case HUDRegions.ResourceBar:
                    level.Hud.InventoryManager.Show = false;
                    level.Hud.ProductionManager.Show = false;
                    return;
			default:
				return;
			}
		}

		public void LeftMouseButtonUpOrdersBar(Level.Level level)
		{
			//level.Hud.LeftMouseClickOrdersBar (level);
		}

		public void LeftMouseButtonUpGameArea(Level.Level level)
		{
		    Player.Player player = Game.Game.Instance.ActivePlayer;

            GameObject hitObject = FindHitObject();
			Vector3 hitPoint = FindHitPoint();

			if (level.Hud.CustomCursor) 
			{
                BuildableWorldObject worldObject = level.Hud.CustomCursor.GetComponent<BuildableWorldObject>();

				if (level.Map.IsBuildable(worldObject) && Game.Game.Instance.ActiveLevel.CanBuildObject(worldObject)) 
				{
					level.Hud.CustomCursor = null;
				}
			}
			else if (hitObject)
			{ 
				WorldObject.WorldObject wob = hitObject.GetComponent<WorldObject.WorldObject>();
				if (wob)
				{ 
					if (!wob.Equals(Game.Game.Instance.ActiveLevel.SelectedObject))   // and if its not the same object
					{
						level.Hud.SelectedObject = hitObject.GetComponent<WorldObject.WorldObject>();
					    Game.Game.Instance.ActiveLevel.SetSelectedObject(level.Hud, wob);
						return;
					}
				} 
				else
				{
					level.Hud.SelectedObject = null;
				    Game.Game.Instance.ActiveLevel.SetSelectedObject(level.Hud, null);
				}
			}
			
			if (hitObject && hitPoint != InvalidPosition)
			{
				if (Game.Game.Instance.ActiveLevel.SelectedObject)
				{
				    Game.Game.Instance.ActiveLevel.SelectedObject.LeftMouseClick(hitObject, hitPoint);
				}
			}
		}

        public GameObject FindHitObject()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
            return null;
        }

        public static Vector3 FindHitPoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) return hit.point;
            return InvalidPosition;
        }

		public void MouseHover(Level.Level level)
        {

        }

		public void MouseOnRightEdge(Level.Level level)
        {
            Vector3 movement = new Vector3(0, 0, 0);
			movement.x += _screenScrollSpeed;
            movement = Camera.main.transform.TransformDirection(movement);
            movement.y = 0;
            Vector3 origin = Camera.main.transform.position;
            Vector3 destination = origin;
            destination.x += movement.x;
            destination.y += movement.y;
            destination.z += movement.z;
            if (destination != origin)
            {
				Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * _screenScrollSpeed);
            }
        }

		public void MouseOnLeftEdge(Level.Level level)
        {
            Vector3 movement = new Vector3(0, 0, 0);
			movement.x -= _screenScrollSpeed;
            movement = Camera.main.transform.TransformDirection(movement);
            movement.y = 0;
            Vector3 origin = Camera.main.transform.position;
            Vector3 destination = origin;
            destination.x += movement.x;
            destination.y += movement.y;
            destination.z += movement.z;
            if (destination != origin)
            {
				Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * _screenScrollSpeed);
            }
        }

		public void MouseOnTopEdge(Level.Level level)
        {
            Vector3 movement = new Vector3(0, 0, 0);
			movement.z += _screenScrollSpeed;
            movement = Camera.main.transform.TransformDirection(movement);
            movement.y = 0;
            Vector3 origin = Camera.main.transform.position;
            Vector3 destination = origin;
            destination.x += movement.x;
            destination.y += movement.y;
            destination.z += movement.z;
            if (destination != origin)
            {
				Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * _screenScrollSpeed);
            }
        }

		public void MouseOnBottomEdge(Level.Level level)
        {
            Vector3 movement = new Vector3(0, 0, 0);
			movement.z -= _screenScrollSpeed;
            movement = Camera.main.transform.TransformDirection(movement);
            movement.y = 0;
            Vector3 origin = Camera.main.transform.position;
            Vector3 destination = origin;
            destination.x += movement.x;
            destination.y += movement.y;
            destination.z += movement.z;
            if (destination != origin)
            {
				Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * _screenScrollSpeed);
            }
        }

		public void MouseWheel(Level.Level level)
        {
			float fov = Camera.main.fieldOfView;
			fov -= Input.GetAxis("Mouse ScrollWheel") * _zoomSensitivity;
			fov = Mathf.Clamp(fov, _minFieldOfView, _maxFieldOfView);
			Camera.main.fieldOfView = fov;
        }
    }
}
