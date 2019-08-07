using System;
using UnityEngine;
using System.Collections.Generic;
using Assets.RTSCore.StateMachineComponents;
using Assets.RTSCore.WorldMap;
using Random = UnityEngine.Random;

namespace Assets.RTSCore.Game
{
    public class Game : StateMachine
    {
        public string PlanetState = "Planet";
        public string PlanetToLevelState = "PlanetToLevel";
        public string LevelToPlanetState = "LevelToPlanet";
        public string LoadingLevelState = "LoadingLevel";
        public string LevelState = "Level";

        private void Level_OnUpdateEvent()
        {
            
        }

        private void PlanetState_OnUpdateEvent()
        {
            if (WorldMap.SelectedLandingSite != null)
            {
                _initialMag = (MainCamera.transform.position - WorldMap.SelectedLandingSite.transform.position)
                    .magnitude;
            }
        }

        private void LoadingLevel_OnUpdateEvent()
        {
            LoadLevel();
            MoveToState("Level");
        }

        private void PlanetToLevelt_OnUpdateEvent()
        {
            MainCamera.transform.position += _moveSpeed * (WorldMap.SelectedLandingSite.transform.position - MainCamera.transform.position).normalized;
            float mag = (MainCamera.transform.position - WorldMap.SelectedLandingSite.transform.position).magnitude;
            MainLight.intensity = 2.0f * (mag / _initialMag);
            if (mag < _moveSpeed)
            {
                MainCamera.transform.position = WorldMap.SelectedLandingSite.transform.position;
                MoveToState("LoadingLevel");
            }
        }

        private void LevelToPlanet_OnUpdateEvent()
        {
            WorldMap.LandingSiteSelected(null);
            WorldMap.ActivePlanet.ResumePlanetView();
            ActiveLevel.gameObject.SetActive(false);
            WorldMap.gameObject.SetActive(true);
            MainLight.intensity = 2.0f;
            MoveCameraToPlanetView(WorldMap.ActivePlanet);
            MoveToState("Planet");
        }

        public GameConfiguration Configuration;

        public Economy.Economy Economy = new Economy.Economy();

        public List<Level.Level> LevelPrefabs = new List<Level.Level>();
        public Level.Level ActiveLevel { get; set; }
        public Player.Player ActivePlayer;
        public WorldMapHud WorldMap;

        public Light MainLight;
        public Camera MainCamera;

        public static Game Instance;
        public const float CameraRatio = 0.5f;
        public const float StartingCameraHeight = 20;

        protected void Start()
        {
            AddState(PlanetState, PlanetState_OnUpdateEvent);
            AddState(PlanetToLevelState, PlanetToLevelt_OnUpdateEvent);
            AddState(LevelToPlanetState, LevelToPlanet_OnUpdateEvent);
            AddState(LoadingLevelState, LoadingLevel_OnUpdateEvent);
            AddState(LevelState, Level_OnUpdateEvent);

            AddTransition(PlanetState, PlanetToLevelState);
            AddTransition(PlanetToLevelState, LoadingLevelState);
            AddTransition(LoadingLevelState, LevelState);
            AddTransition(LevelState, LevelToPlanetState);
            AddTransition(LevelToPlanetState, PlanetState);

            GameTimeManager.CurrentGameTime = new GameDateTime();
            MoveToState(PlanetState);
            ActivePlayer = Instantiate(ActivePlayer);
            WorldMap = Instantiate(WorldMap);

            Instance = this;
            WorldMap.CreateNewGame(new GameSettings());
            WorldMap.ActivePlanet = WorldMap.Planets[0];

            Configuration.LoadConfiguration();
        }

        public void SwitchToWorldView()
        {
            MoveToState("LevelToPlanet");
        }

        public void SwitchToLevelView()
        {
            MoveToState("PlanetToLevel");
        }

        public void LoadLevel()
        {
            var site = WorldMap.SelectedLandingSite;

            if (site.Level != null) site.Level.ResumeLevel = true;
            if (site.Level == null) site.Level = GenerateNewLevel();
            ActiveLevel = site.Level;
            ActiveLevel.gameObject.SetActive(true);
            WorldMap.gameObject.SetActive(false);

            Economy.UpdateGallacticMarketItems();
        }

        public void MoveCameraToWorldObjectInstantly(WorldObject.WorldObject worldObject)
        {
            float xzOffSet = (1 - CameraRatio) * StartingCameraHeight;

            MainCamera.transform.position = new Vector3(worldObject.transform.position.x - xzOffSet, StartingCameraHeight, worldObject.transform.position.z- xzOffSet);
            MainCamera.transform.LookAt(worldObject.transform);
        }

        public Level.Level GenerateNewLevel()
        {
            int index = Mathf.FloorToInt(Random.value * LevelPrefabs.Count);
            if (index >= LevelPrefabs.Count) index--;

            var newLevel = Instantiate(LevelPrefabs[index]);
            return newLevel;
        }

        private float _moveSpeed = 0.5f;

        private float _initialMag = 1;
        protected override void Update()
        {
            base.Update();
            GameTimeManager.Update(Time.deltaTime);
        }

        public void PlayerDefeated()
        {
            
        }

        public void MoveCameraToPlanetView(Planet planet)
        {
            MainCamera.transform.position = new Vector3(0,0,-110);
            MainCamera.transform.LookAt(planet.transform);
        }
    }
}
