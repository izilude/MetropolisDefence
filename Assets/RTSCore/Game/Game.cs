using UnityEditorInternal;
using UnityEngine;
using System.Collections.Generic;
using Assets.RTSCore.CustomEngine;
using Assets.RTSCore.WorldMap;

namespace Assets.RTSCore.Game
{
    public enum GameState { Planet, PlanetToLevelTransition, LoadingLevel, Level, LevelToPlanetTransition}

    public class Game : MonoBehaviour
    {
        public GameState State { get; set; }
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
            GameTimeManager.CurrentGameTime = new GameDateTime();
            State = GameState.Planet;
            ActivePlayer = Instantiate(ActivePlayer);
            BackendEngine.Initialize();
            WorldMap = Instantiate(WorldMap);

            Instance = this;
            WorldMap.CreateNewGame(new GameSettings());
            WorldMap.ActivePlanet = WorldMap.Planets[0];

            Configuration.LoadConfiguration();
        }

        public void SwitchToWorldView()
        {
            State = GameState.LevelToPlanetTransition;
        }

        public void SwitchToLevelView()
        {
            State = GameState.PlanetToLevelTransition;
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
        protected void Update()
        {
            GameTimeManager.Update(Time.deltaTime);

            switch (State)
            {
                case GameState.Planet:
                    if (WorldMap.SelectedLandingSite != null)
                    {
                        _initialMag = (MainCamera.transform.position - WorldMap.SelectedLandingSite.transform.position)
                            .magnitude;
                    }
                    break;
                case GameState.PlanetToLevelTransition:
                    MainCamera.transform.position += _moveSpeed * (WorldMap.SelectedLandingSite.transform.position - MainCamera.transform.position).normalized;
                    float mag = (MainCamera.transform.position - WorldMap.SelectedLandingSite.transform.position).magnitude;
                    MainLight.intensity = 2.0f * (mag / _initialMag);
                    if (mag < _moveSpeed)
                    {
                        MainCamera.transform.position = WorldMap.SelectedLandingSite.transform.position;
                        State = GameState.LoadingLevel;
                    }
                    break;
                case GameState.LevelToPlanetTransition:
                    WorldMap.LandingSiteSelected(null);
                    WorldMap.ActivePlanet.ResumePlanetView();
                    ActiveLevel.gameObject.SetActive(false);
                    WorldMap.gameObject.SetActive(true);
                    MainLight.intensity = 2.0f;
                    MoveCameraToPlanetView(WorldMap.ActivePlanet);
                    State = GameState.Planet;
                    break;
                case GameState.LoadingLevel:
                    LoadLevel();
                    State = GameState.Level;
                    break;
                case GameState.Level:
                    break;
            }
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
