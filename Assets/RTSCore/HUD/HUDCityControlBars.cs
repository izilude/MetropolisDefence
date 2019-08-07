using System;
using System.Collections.Generic;
using Assets.RTSCore.AI;
using Assets.RTSCore.Game;
using Assets.RTSCore.Information;
using Assets.RTSCore.Misc;
using Assets.RTSCore.Requests;
using Assets.RTSCore.Styles;
using Assets.RTSCore.WorldObject;
using Assets.RTSCore.WorldObject.Buildings;
using Assets.RTSCore.WorldObject.Units;
using UnityEngine;

namespace Assets.RTSCore.HUD
{
    public class HUDCityControlBars
    {
        private Texture2D _topBarSkin;
        private GUISkin _leftBottomBarSkin;
        private GUISkin _rightBottomBarSkin;
        private Texture2D _bottomBarTexture;
        private List<BuildTab> _buildTabList;

        private BuildTab _selectedBuildTab;
        private int _selectedBuildTabIndex;

        public HUDCityControlBars(
            List<BuildTab> buildTabList,
            Texture2D bottomBarTexture,
            GUISkin leftBottomBarSkin,
            GUISkin rightBottomBarSkin,
            Texture2D topBarSkin)
        {
            _buildTabList = buildTabList;
            _topBarSkin = topBarSkin;
            _leftBottomBarSkin = leftBottomBarSkin;
            _rightBottomBarSkin = rightBottomBarSkin;
            _bottomBarTexture = bottomBarTexture;
        }

        public void Draw(WorldObject.WorldObject selectedWorldObject)
        {
            DrawBottomBar(selectedWorldObject);
            DrawRightBottomBar(selectedWorldObject);
            DrawLeftBottomBar();
            DrawTopBar();
        }

        private void DrawBottomBar(WorldObject.WorldObject selectedWorldObject)
        {
            int barWidth = Screen.width - 2 * HUDStyles.LEFTRIGHT_BAR_WIDTH;
            _bottomBarTexture.wrapMode = TextureWrapMode.Repeat;
            Rect position = new Rect(HUDStyles.LEFTRIGHT_BAR_WIDTH,
                                      Screen.height - HUDStyles.BOTTOM_BAR_HEIGHT,
                                      barWidth,
                                      HUDStyles.BOTTOM_BAR_HEIGHT);

            Rect texCoords = new Rect(0, 0, barWidth / _bottomBarTexture.width, 1);

            GUI.DrawTextureWithTexCoords(position, _bottomBarTexture, texCoords);

            DrawSelectedObjectInformation(position, selectedWorldObject);
        }

        protected void DrawSelectedObjectInformation(Rect bottomBar, WorldObject.WorldObject selectedWorldObject)
        {
            if (!selectedWorldObject) { return; }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = HUDStyles.TEXT_COLOR;

            Information.Information info = selectedWorldObject.GetInformation();
            if (info == null) { return; }

            int y = (int)bottomBar.y + HUDStyles.INFO_TEXT_SPACING;

            string line = string.Empty;
            if (selectedWorldObject is ProductionBuilding)
            {
                ProductionBuilding productionBuilding = selectedWorldObject as ProductionBuilding;
                line = String.Format("{0} ({1}/{2})", info.Name, productionBuilding.CurrentPopulation, productionBuilding.MaxPopulation);

                Texture2D texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.grey);
                texture.Apply();
                GUI.skin.box.normal.background = texture;
                float perc = productionBuilding.ProductionProgressPercent;
                float currentWidth = perc * HUDStyles.PROGRESS_BAR_WIDTH < 0 ? 0 : perc * HUDStyles.PROGRESS_BAR_WIDTH;
                Rect progressBarArea = new Rect(bottomBar.x + (bottomBar.width - HUDStyles.PROGRESS_BAR_WIDTH) / 2, y, currentWidth, HUDStyles.INFO_TEXT_SPACING * 0.75f);
                Rect progressBarFull = new Rect(bottomBar.x + (bottomBar.width - HUDStyles.PROGRESS_BAR_WIDTH) / 2, y, HUDStyles.PROGRESS_BAR_WIDTH, HUDStyles.INFO_TEXT_SPACING * 0.75f);
                GUI.Box(progressBarFull, GUIContent.none);

                texture.SetPixel(0, 0, Color.green);
                texture.Apply();
                GUI.skin.box.normal.background = texture;

                if (currentWidth > 5)
                {
                    GUI.Box(progressBarArea, GUIContent.none);
                }

                GUIStyle style2 = new GUIStyle();
                style2.normal.textColor = Color.black;
                GUI.Label(progressBarFull, productionBuilding.ProductionItem, style2);

                GUI.skin.box.normal.background = null;

                y += HUDStyles.INFO_TEXT_SPACING;
            }
            else if (selectedWorldObject is Building)
            {
                Building building = selectedWorldObject as Building;
                line = String.Format("{0} ({1}/{2})", info.Name, building.CurrentPopulation, building.MaxPopulation);
            }
            else if (selectedWorldObject is Unit)
            {
                Unit unit = selectedWorldObject as Unit;
                line = String.Format("{0}", info.Name);
            }

            GUI.Label(new Rect(bottomBar.x + (bottomBar.width - 200) / 2, y, 200, HUDStyles.INFO_TEXT_SPACING), line, style);

            if (selectedWorldObject is Building)
            {
                y += HUDStyles.INFO_TEXT_SPACING;
                if (GUI.Button(new Rect(bottomBar.x + (bottomBar.width - 200) / 2, y, 200, HUDStyles.INFO_TEXT_SPACING), "Inventory"))
                {
                    OnToggleShowBuildingInventoryEvent();
                }
            }

            if (selectedWorldObject is ProductionBuilding)
            {
                y += HUDStyles.INFO_TEXT_SPACING;
                if (GUI.Button(new Rect(bottomBar.x + (bottomBar.width - 200) / 2, y, 200, HUDStyles.INFO_TEXT_SPACING), "Production"))
                {
                    OnToggleShowProductionItemsEvent();
                }
            }
        }

        public event EventHandler ToggleShowBuildingInventoryEvent;
        private void OnToggleShowBuildingInventoryEvent()
        {
            if (ToggleShowBuildingInventoryEvent != null)
            {
                ToggleShowBuildingInventoryEvent(null, null);
            }
        }

        public event EventHandler ToggleShowProductionItemsEvent;
        private void OnToggleShowProductionItemsEvent()
        {
            if (ToggleShowProductionItemsEvent != null)
            {
                ToggleShowProductionItemsEvent(null, null);
            }
        }


        private void DrawLeftBottomBar()
        {
            GUI.skin = _leftBottomBarSkin;
            Rect position = new Rect(0, Screen.height - HUDStyles.LEFTRIGHT_BAR_HEIGHT, HUDStyles.LEFTRIGHT_BAR_WIDTH, HUDStyles.LEFTRIGHT_BAR_HEIGHT);
            GUI.BeginGroup(position);
            GUI.Box(new Rect(0, 0, HUDStyles.LEFTRIGHT_BAR_WIDTH, HUDStyles.LEFTRIGHT_BAR_HEIGHT), "");
            GUI.EndGroup();

            if (_selectedBuildTab != null)
            {
                DrawBuildTabList(position);
                DrawBuildList(_selectedBuildTab.InfrastructurePrefabList, position);
            }
            else
            {
                DrawBuildTabList(position);
            }
        }

        private int _selectedBuildListItem = -1;
        protected void DrawBuildList(List<GameObject> wobs, Rect bottomBar)
        {
            for (int i = 0; i < wobs.Count; i++)
            {
                Rect position = GetBuildIconPositionHex(bottomBar, _selectedBuildTabIndex / 2, i + 1, _selectedBuildTabIndex % 2);

                Texture image;
                if (position.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)) || _selectedBuildListItem == i)
                {
                    image = wobs[i].GetComponent<BuildableWorldObject>().BuildHoverImage;
                }
                else
                {
                    image = wobs[i].GetComponent<BuildableWorldObject>().BuildImage;
                }


                if (GUI.Button(position, image, GUIStyle.none))
                {
                    _selectedBuildListItem = i;
                    OnBuildItemSelected(wobs[i]);
                }
            }
        }

        public event EventHandler BuildItemSelectedEvent;
        private void OnBuildItemSelected(GameObject gameObject)
        {
            _selectedBuildTab = null;
            _selectedBuildTabIndex = -1;
            _selectedBuildListItem = -1;

            if (BuildItemSelectedEvent != null)
            {
                BuildItemSelectedEvent(gameObject, null);
            }
        }

        private void DrawMiniMapBar()
        {

            //GUI.skin = miniMapSkin;
            GUI.BeginGroup(new Rect(Screen.width - HUDStyles.MINIMAP_BAR_WIDTH + HUDStyles.MINIMAP_BAR_HSHIFT,
                                    HUDStyles.MINIMAP_BAR_VSHIFT,
                                    HUDStyles.MINIMAP_BAR_WIDTH,
                                    HUDStyles.MINIMAP_BAR_HEIGHT));

            GUI.Box(new Rect(0, 0, HUDStyles.MINIMAP_BAR_WIDTH, HUDStyles.MINIMAP_BAR_HEIGHT), "");
            GUI.EndGroup();

        }

        private static Rect GetBuildIconPositionHex(Rect bottomBar, int n, int m, int i)
        {
            float sinTheta = 0.5885f;
            float theta = (float)Math.Asin(sinTheta);
            float tanTheta = (float)Math.Tan(theta);
            float size = 32;
            bool isA = i % 2 == 0;

            float x = n * (1 + 0.5f * tanTheta);
            x += isA ? 0 : (1 - 0.5f * tanTheta) + 1 / size;
            float y = isA ? m : m + 0.5f;
            x = bottomBar.x + size * x;
            y = bottomBar.y + HUDStyles.BUILD_MENU_Y_OFFSET + size * y;

            Rect position = new Rect(x, y, size, size);
            return position;
        }

        protected void DrawBuildTabList(Rect bottomBar)
        {
            for (int i = 0; i < _buildTabList.Count; i++)
            {
                int column = i % HUDStyles.BUILD_IMAGE_PER_ROW;
                int row = i / HUDStyles.BUILD_IMAGE_PER_ROW;

                int m = 0;
                int n = i / 2;

                Rect position = GetBuildIconPositionHex(bottomBar, n, m, i);

                Texture image;
                if (position.Contains(new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y)))
                {
                    image = _buildTabList[i].HoverImage;
                }
                else
                {
                    image = _buildTabList[i].Image;
                }

                if (GUI.Button(position, image, GUIStyle.none))
                {
                    _selectedBuildTab = _buildTabList[i];
                    _selectedBuildTabIndex = i;
                }
            }
        }

        private void DrawRightBottomBar(WorldObject.WorldObject selectedObject)
        {
            GUI.skin = _rightBottomBarSkin;
            Rect position = new Rect(Screen.width - HUDStyles.LEFTRIGHT_BAR_WIDTH, Screen.height - HUDStyles.LEFTRIGHT_BAR_HEIGHT, HUDStyles.LEFTRIGHT_BAR_WIDTH, HUDStyles.LEFTRIGHT_BAR_HEIGHT);
            GUI.BeginGroup(position);
            GUI.Box(new Rect(0, 0, HUDStyles.LEFTRIGHT_BAR_WIDTH, HUDStyles.LEFTRIGHT_BAR_HEIGHT), "");
            GUI.EndGroup();

            if (!selectedObject)
            {
                DrawRequestBoard(position);
            }
            else if (selectedObject is Building)
            {
                DrawBuildingInformation(selectedObject as Building, position);
            }
        }

        protected void DrawBuildingInformation(Building building, Rect bottomBar)
        {
            GUIStyle style = new GUIStyle();
            style.normal.textColor = HUDStyles.TEXT_COLOR;

            int y = (int)bottomBar.y + HUDStyles.INFO_TEXT_SPACING;

            foreach (CountdownEvent activeService in building.ActiveServices)
            {
                y += HUDStyles.INFO_TEXT_SPACING;
                string line = String.Format("{0} {1:0}/{2:0}", activeService.Name, activeService.TimeRemaining, activeService.MaxTime);

                GUI.Label(new Rect(bottomBar.x + (bottomBar.width - 200) / 2, y, 200, HUDStyles.INFO_TEXT_SPACING), line, style);
            }
        }

        protected void DrawRequestBoard(Rect bottomBar)
        {
            List<Request> requests = null;

            requests = RequestManager.GetPostedRequests();

            if (requests == null) { return; }

            GUIStyle style = new GUIStyle();
            style.normal.textColor = HUDStyles.TEXT_COLOR;

            int y = (int)bottomBar.y + HUDStyles.INFO_TEXT_SPACING;

            foreach (Request request in requests)
            {
                var info = request.InitiatorOfRequest.GetInformation();
                if (info is BuildingInformation)
                {
                    y += HUDStyles.INFO_TEXT_SPACING;
                    string line = String.Format("{0} {2}: {1}", (info as BuildingInformation).Name, request.Info(), request.State);

                    GUI.Label(new Rect(bottomBar.x + (bottomBar.width - 200) / 2, y, 200, HUDStyles.INFO_TEXT_SPACING), line, style);
                }
            }
        }

        protected virtual void DrawTopBar()
        {
            Rect position = new Rect(0, 0, Screen.width, HUDStyles.TOP_BAR_HEIGHT);

            _topBarSkin.wrapMode = TextureWrapMode.Repeat;
            Rect texCoords = new Rect(0, 0, Screen.width / _topBarSkin.width, 1);
            GUI.DrawTextureWithTexCoords(position, _topBarSkin, texCoords);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = HUDStyles.TEXT_COLOR;
            style.fontSize = 20;

            Rect galacticMarketButtonPosition = new Rect(
                Screen.width - HUDStyles.TOP_BAR_MONEY_WIDTH - HUDStyles.TOP_BAR_GALACTICMARKET_WIDTH - HUDStyles.TOP_BAR_XSPACING,
                (HUDStyles.TOP_BAR_HEIGHT - HUDStyles.TOP_BAR_GALACTICMARKET_HEIGHT) / 2,
                HUDStyles.TOP_BAR_GALACTICMARKET_WIDTH,
                HUDStyles.TOP_BAR_GALACTICMARKET_HEIGHT);

            string line = String.Format("{0} {1}, {2}", 
                GameTimeManager.CurrentGameTime.Month,
                GameTimeManager.CurrentGameTime.Day + 1,
                GameTimeManager.CurrentGameTime.Year);
            style.alignment = TextAnchor.MiddleLeft;
            GUI.Label(new Rect(10, 0, HUDStyles.TOP_BAR_MONEY_WIDTH, HUDStyles.TOP_BAR_HEIGHT), line, style);

            if (GUI.Button(galacticMarketButtonPosition, "Galactic Market"))
            {
                Game.Game.Instance.ActiveLevel.GalacticMarketBoard.Show = !Game.Game.Instance.ActiveLevel.GalacticMarketBoard.Show;
            }

            galacticMarketButtonPosition.x -= (galacticMarketButtonPosition.width + 10);
            if (GUI.Button(galacticMarketButtonPosition, "Show Production"))
            {
                Game.Game.Instance.ActiveLevel.Hud.OverlayProduction = !Game.Game.Instance.ActiveLevel.Hud.OverlayProduction;
            }

            line = String.Format("Money: {0}", Game.Game.Instance.ActivePlayer.Money);
            style.alignment = TextAnchor.MiddleLeft;
            GUI.Label(new Rect(Screen.width - HUDStyles.TOP_BAR_MONEY_WIDTH, 0, HUDStyles.TOP_BAR_MONEY_WIDTH, HUDStyles.TOP_BAR_HEIGHT), line, style);
        }
    }
}
