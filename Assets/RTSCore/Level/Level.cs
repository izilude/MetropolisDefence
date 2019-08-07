using System;
using System.Collections.Generic;
using System.Linq;
using Assets.RTSCore.AI;
using Assets.RTSCore.Controls;
using Assets.RTSCore.Game;
using Assets.RTSCore.HUD;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Map;
using Assets.RTSCore.Player;
using Assets.RTSCore.Services;
using Assets.RTSCore.WorldObject;
using Assets.RTSCore.WorldObject.Buildings;
using Assets.RTSCore.WorldObject.Misc;
using UnityEngine;

namespace Assets.RTSCore.Level
{
    public class Level : MonoBehaviour
    {
        public Light Sun;
        public Map.Map Map;
		public CreepWaveManager CreepWaveManager;
        public ServiceChart ServiceChart;
        public RandomEventEngine EventEngine;
        public GalacticMarketBoard GalacticMarketBoard;
        public HUD.HUD Hud;
        public UserInput UserInput;
		public List<MapFeatureOptions> NaturalResources = new List<MapFeatureOptions>();
        public Building Headquarters;
        public float UpdateTickTime;

        private float _deltaTime = 0;
        public bool ResumeLevel { get; set; }

        public WorldObject.WorldObject SelectedObject { get; set; }
        public GUISkin SelectBoxSkin;
        protected List<WorldObject.WorldObject> WorldObjects = new List<WorldObject.WorldObject>();

        protected virtual void Start()
        {
            Sun = Instantiate(Sun);
            Map = Instantiate(Map);

            ServiceChart = Instantiate(ServiceChart);
            EventEngine = Instantiate(EventEngine);
            GalacticMarketBoard = Instantiate(GalacticMarketBoard);
            Hud = Instantiate(Hud);
            UserInput = Instantiate(UserInput);
            CreepWaveManager = Instantiate(CreepWaveManager);

            Map.Initialize();

            Sun.transform.parent = this.transform;
            Map.transform.parent = this.transform;
            ServiceChart.transform.parent = this.transform;
            EventEngine.transform.parent = this.transform;
            GalacticMarketBoard.transform.parent = this.transform;
            Hud.transform.parent = this.transform;
            UserInput.transform.parent = this.transform;
            CreepWaveManager.transform.parent = this.transform;

            PlaceHeadquartersRandomly();
            Game.Game.Instance.MoveCameraToWorldObjectInstantly(Headquarters);

            CreepWaveManager.Initialize();

            AssignResourcesToBuildings();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (ResumeLevel)
            {
                Game.Game.Instance.MoveCameraToWorldObjectInstantly(Headquarters);
                ResumeLevel = false;
            }

            _deltaTime += GameTimeManager.DeltaTime;

            if (_deltaTime > UpdateTickTime)
            {
                _deltaTime = 0;
                RequestManager.Run(WorldObjects);

                List<Building> buildings = GetBuildings();
                PopulationManager.Run(buildings);
            }

            if (Hud.Ready)
            {
                UserInput.MouseActivity(this);
                UserInput.KeyboardActivity();
                Hud.ObjectFollowCursor(Map);
            }
        }

        public bool CanBuildObject(BuildableWorldObject worldObject)
        {
            if (worldObject.Cost > Game.Game.Instance.ActivePlayer.Money)
            {
                return false;
            }
            return true;
        }

        private void AssignResourcesToBuildings()
        {
            List<bool> conversionFound = new List<bool>();
            var conversions = Game.Game.Instance.Configuration.Conversions;
            for (var i = 0; i < conversions.Count; i++) conversionFound.Add(false);

            var hudMenus = Hud.BuildTabList;
            foreach (var buildTab in hudMenus)
            {
                foreach (var gameObj in buildTab.InfrastructurePrefabList)
                {
                    Building building = gameObj.GetComponent<Building>();
                    if (building == null) continue;

                    building.ItemFlags.Clear();

                    for (var i = 0; i < conversions.Count; i++)
                    {
                        var conversion = conversions[i];
                        string buildingName = conversion.Building;
                        if (building.Name == buildingName)
                        {
                            building.ItemFlags.Add(new ItemBuildingFlags
                            {
                                Name = conversion.ItemProduced,
                                Producible = true,
                                Keep = false,
                                Request = false,
                                RequireHarvestToTake = false,
                                SelfSufficient = false
                            });
                            conversionFound[i] = true;
                        }
                    }
                }
            }

            for (var i = 0; i < conversions.Count; i++)
            {
                var conversion = conversions[i];
                if (conversionFound[i]) continue;
                Debug.Log(String.Format("{0} could not be assigned to building {1}", conversion.ItemProduced, conversion.Building));
            }
        }

        public void PlaceHeadquartersRandomly()
        {
            int maxTries = 100;
            int cnt = 0;

            List<Tile> tiles = null;
            while (cnt < maxTries)
            {
                Vector3 location = Game.Game.Instance.ActiveLevel.Map.GetRandomVector3();
                location = new Vector3(location.x + 0.5f, 0, location.z + 0.5f);
                Headquarters.transform.position = location;

                tiles = Game.Game.Instance.ActiveLevel.Map.FindOccupyingTiles(Headquarters);
                if (tiles.FirstOrDefault(x => x.Buildable == false) != null) cnt++;
                else break;
            }

            if (tiles == null) throw new Exception("Could not find a place to initialize headquarters");

            foreach (var tile in tiles)
            {
                tile.Buildable = false;
                tile.Accessible = false;
            }

            AddWorldObject(Headquarters, true);
        }

        private List<Building> GetBuildings()
        {
            List<Building> buildings = new List<Building>();
            foreach (WorldObject.WorldObject wob in WorldObjects)
            {
                if (wob is Building)
                {
                    buildings.Add(wob as Building);
                }
            }

            return buildings;
        }

        public WorldObject.WorldObject AddWorldObject(WorldObject.WorldObject worldObject, bool ignoreCost)
        {
            if (!ignoreCost && worldObject is BuildableWorldObject)
            {
                BuildableWorldObject building = worldObject as BuildableWorldObject;
                if (!CanBuildObject(building)) { return null; }
                Game.Game.Instance.ActivePlayer.Money -= building.Cost;
            }

            if (worldObject is Road) return null;

            WorldObject.WorldObject newGameObject = Instantiate(worldObject, worldObject.transform.position, worldObject.transform.rotation);

            newGameObject.transform.parent = Game.Game.Instance.ActiveLevel.transform;
            newGameObject.enabled = true;
            WorldObjects.Add(newGameObject);

            return newGameObject;
        }

        public void RemoveWorldObject(WorldObject.WorldObject worledObject)
        {
            WorldObjects.Remove(worledObject);
        }

        public Vector3 HeadquarterPosition
        {
            get
            {
                var headquarters = WorldObjects.FirstOrDefault(x => x is GalacticMarket);
                if (headquarters == null) return new Vector3();
                return headquarters.transform.position;
            }
        }

        public void SetSelectedObject(HUD.HUD hud, WorldObject.WorldObject newSelectedObject)
        {
            // Clear the currently selected object if one is set
            if (SelectedObject)
            {
                SelectedObject.SetSelected(false);
                SelectedObject = null;
            }

            if (newSelectedObject)
            {
                newSelectedObject.SetSelected(true);
                SelectedObject = newSelectedObject;
            }
        }

        public List<Item> ItemsBeingExported()
        {
            List<Item> items = new List<Item>();

            return items;
        }

        public List<Item> ItemsBeingImported()
        {
            List<Item> items = new List<Item>();

            return items;
        }

        public List<string> AllItems()
        {
            List<string> items = new List<string>();

            foreach (ItemConversion conversion in Game.Game.Instance.Configuration.Conversions)
            {
                items.Add(conversion.ItemProduced);
            }

            items = items.Select(x => x).Distinct().ToList();

            for(int i=items.Count-1;i>=0;i--) if (String.IsNullOrEmpty(items[i])) items.RemoveAt(i);

            return items;
        }
    }
}
