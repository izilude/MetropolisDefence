using System;
using System.Collections.Generic;
using Assets.RTSCore.Controls;
using Assets.RTSCore.Misc;
using Assets.RTSCore.Styles;
using Assets.RTSCore.WorldObject.Buildings;
using UnityEngine;

namespace Assets.RTSCore.HUD
{
    public class HUD : MonoBehaviour
    {
        public Texture2D topBarSkin;
        public GUISkin leftBottomBarSkin;
        public GUISkin rightBottomBarSkin;
        public Texture2D bottomBarTexture;
        public GUISkin miniMapSkin;
        public List<BuildTab> BuildTabList;

        public GameObject CustomCursor { get; set; }
        public WorldObject.WorldObject SelectedObject { get; set;}

        public GUISkin InventoryManagerBackgroundSkin;
        public GUISkin ProductionManagerBackgroundSkin;
        public GUISkin GameEventBackgroundSkin;

        public HUDInventoryManager InventoryManager;
        public HUDProductionManager ProductionManager;
        public HUDCityControlBars CityControlBars;
        public HUDGameEventMessage GameEventMessage;

        public bool OverlayProduction { get; set; }

        public bool Ready = false;

        protected void Start()
        {
            GameEventMessage = new HUDGameEventMessage(GameEventBackgroundSkin);
            InventoryManager = new HUDInventoryManager(InventoryManagerBackgroundSkin);
            ProductionManager = new HUDProductionManager(ProductionManagerBackgroundSkin);
            CityControlBars = new HUDCityControlBars(BuildTabList, bottomBarTexture, leftBottomBarSkin, rightBottomBarSkin, topBarSkin);
            CityControlBars.BuildItemSelectedEvent += CityControlBars_BuildItemSelectedEvent;
            CityControlBars.ToggleShowBuildingInventoryEvent += CityControlBars_ToggleShowBuildingInventoryEvent;
            CityControlBars.ToggleShowProductionItemsEvent += CityControlBars_ToggleShowProductionItemsEvent;
            Ready = true;
        }

        private void CityControlBars_ToggleShowProductionItemsEvent(object sender, EventArgs e)
        {
            if (SelectedObject is ProductionBuilding)
            {
                ToggleShowProductionItems(SelectedObject as ProductionBuilding);
            }
        }

        private void CityControlBars_ToggleShowBuildingInventoryEvent(object sender, EventArgs e)
        {
            if (SelectedObject is Building)
            {
                ToggleShowBuildingInventory(SelectedObject as Building);
            }
        }

        private void CityControlBars_BuildItemSelectedEvent(object sender, EventArgs e)
        {
            if (sender is GameObject)
            {
                SetCustomCursor(sender as GameObject);
            }
        }

        // Update is called once per frame
        void OnGUI()
        {
            CityControlBars.Draw(SelectedObject);

            InventoryManager.SelectedBuilding = SelectedObject as Building;
            ProductionManager.SelectedBuilding = SelectedObject as ProductionBuilding;
            InventoryManager.Draw();
            ProductionManager.Draw();
            GameEventMessage.Draw();
        }

        private void ToggleShowBuildingInventory(Building building)
        {
            InventoryManager.Show = !InventoryManager.Show;
            if (InventoryManager.Show) { ProductionManager.Show = false; }
        }

        private void ToggleShowProductionItems(ProductionBuilding building)
        {
            ProductionManager.Show = !ProductionManager.Show;
            if (ProductionManager.Show) { InventoryManager.Show = false; }
        }

        public void SetCustomCursor(GameObject gameObject)
		{
			if (CustomCursor != null) 
			{
				GameObject.Destroy (CustomCursor);
				CustomCursor = null;
			}

			if (gameObject == null) { return; }

			GameObject newGameObject = (GameObject)Instantiate (gameObject, new Vector3 (0, 0, 0), new Quaternion ());
			if (newGameObject)
			{
				CustomCursor = newGameObject;
			}
		}

		public void ObjectFollowCursor(Map.Map map) 
		{
			if (CustomCursor == null) 
			{
				return;
			}

			Vector3 point = MouseActions.FindHitPoint();
			CustomCursor.transform.position = point;
			map.SnapToGrid(CustomCursor.GetComponent<WorldObject.WorldObject>());
		}

		public HUDRegions MouseInRegion()
        {
            //Screen coordinates start in the lower-left corner of the screen
            //not the top-left of the screen like the drawing coordinates do
            Vector3 mousePos = Input.mousePosition;
            bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width;
			bool insideHeight = mousePos.y >= HUDStyles.LEFTRIGHT_BAR_HEIGHT && mousePos.y <= Screen.height - HUDStyles.RESOURCE_BAR_HEIGHT;
            if (mousePos.x > HUDStyles.LEFTRIGHT_BAR_WIDTH && mousePos.x < Screen.width - HUDStyles.LEFTRIGHT_BAR_WIDTH)
            {
                insideHeight = mousePos.y >= HUDStyles.BOTTOM_BAR_HEIGHT && mousePos.y <= Screen.height - HUDStyles.RESOURCE_BAR_HEIGHT;
            }

            if (InventoryManager.MouseInside(mousePos)) { return HUDRegions.InventoryManager; }
            if (ProductionManager.MouseInside(mousePos)) { return HUDRegions.ProductionManager; }
            if (Game.Game.Instance.ActiveLevel.GalacticMarketBoard.MouseInside(mousePos)) { return HUDRegions.GalacticMarketBoard; }

			if (insideWidth && insideHeight) { return HUDRegions.GameArea; }

			bool insideOrdersBar = insideWidth && mousePos.y < HUDStyles.LEFTRIGHT_BAR_HEIGHT && mousePos.y > 0;
			if (insideOrdersBar) { return HUDRegions.OrdersBar; }

			bool insideResourceBar = insideWidth && mousePos.y < Screen.height && mousePos.y > Screen.height - HUDStyles.RESOURCE_BAR_HEIGHT;
			if (insideResourceBar) { return HUDRegions.ResourceBar; }

			bool insideMiniMap = false;
			if (insideMiniMap) { return HUDRegions.MiniMap; }

			return HUDRegions.Invalid;
        }

        public Rect GetPlayingArea()
        {
			return new Rect(0, HUDStyles.RESOURCE_BAR_HEIGHT, Screen.width, Screen.height - HUDStyles.RESOURCE_BAR_HEIGHT - HUDStyles.LEFTRIGHT_BAR_HEIGHT);
        }

        private int MaxNumRows(int areaHeight)
        {
			return areaHeight / HUDStyles.BUILD_IMAGE_HEIGHT;
        }

        private Rect GetButtonPos(int row, int column)
        {
			int left = column * HUDStyles.BUILD_IMAGE_WIDTH + HUDStyles.BUILD_IMAGE_X_SHIFT;
			float top = row * HUDStyles.BUILD_IMAGE_HEIGHT + HUDStyles.BUILD_IMAGE_Y_SHIFT;
			return new Rect(left, top, HUDStyles.BUILD_IMAGE_WIDTH, HUDStyles.BUILD_IMAGE_HEIGHT);
        }
    }
}
