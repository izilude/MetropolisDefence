using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.RTSCore.Game;
using UnityEngine;

namespace Assets.RTSCore.WorldMap
{
    public class WorldMapHud : MonoBehaviour
    {
        public LandingSite EasyLandingSite;
        public LandingSite HardLandingSite;

        public List<Planet> PlanetPrefabs;

        public Planet ActivePlanet;
        public List<Planet> Planets = new List<Planet>();

        public Texture2D LeftBackground;
        public Texture2D RightBackground;
        public Texture2D DescriptionBackground;
        public int Padding = 0;

        void Start()
        {

        }

        public LandingSite SelectedLandingSite;
        private Quaternion _targetRotation;
        public void LandingSiteSelected(LandingSite site)
        {
            if (SelectedLandingSite != null)
            {
                SelectedLandingSite.NormalDisplay.SetActive(true);
                SelectedLandingSite.SelectedDisplay.SetActive(false);
            }

            if (site == null)
            {
                SelectedLandingSite = null;
                return;
            }

            //Game.Game.Instance.SwitchToLevelView(site);
            SelectedLandingSite = site;
            SelectedLandingSite.NormalDisplay.SetActive(false);
            SelectedLandingSite.SelectedDisplay.SetActive(true);
            var toCamera = Quaternion.LookRotation(Game.Game.Instance.MainCamera.gameObject.transform.position - ActivePlanet.transform.position);
            var toSite = Quaternion.LookRotation(site.transform.localPosition);
            var fromSite = Quaternion.Inverse(toSite);
            _targetRotation = toCamera * fromSite;
        }

        public Planet InstantiateRandomPlanet()
        {
            var num = UnityEngine.Random.value;
            int index = (int)Math.Floor(num * PlanetPrefabs.Count);
            if (index == PlanetPrefabs.Count) index--;

            var planet = Instantiate(PlanetPrefabs[index]);
            planet.transform.parent = this.transform;

            return planet;
        }

        void Update()
        {
            if (SelectedLandingSite != null)
            {
                ActivePlanet.transform.rotation =
                  Quaternion.RotateTowards(ActivePlanet.transform.rotation, _targetRotation, 1);
            }
        }

        void OnGUI()
        {
            switch (Game.Game.Instance.State)
            {
                case GameState.Planet:
                    GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
                    Draw();
                    GUI.EndGroup();
                    break;
                default:
                    break;
            }
        }

        public void Draw()
        {
            DrawLeftWindow();
            DrawRightWindow();
        }

        private void DrawRightWindow()
        {
            float titleHeight = 30;
            float descriptionBoxHeight = 300;
            float buttonHeight = 24;

            var width = 300;
            var windowArea = new Rect(Screen.width - width - Padding, Padding, width, Screen.height - 2 * Padding);

            GUI.DrawTexture(windowArea, RightBackground);

            GUIStyle headerStyle = new GUIStyle();
            headerStyle.fontSize = 20;
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.normal.textColor = Color.white;

            GUIStyle descriptionStyle = new GUIStyle();
            descriptionStyle.fontSize = 12;
            descriptionStyle.alignment = TextAnchor.UpperLeft;
            descriptionStyle.normal.textColor = Color.white;
            descriptionStyle.wordWrap = true;
            descriptionStyle.normal.background = DescriptionBackground;

            if (SelectedLandingSite != null)
            {
                float y = Padding;
                GUI.Label(new Rect(windowArea.x + Padding, windowArea.y + y, width - 2 * Padding, titleHeight), 
                    SelectedLandingSite.Name, 
                    headerStyle);
                y += titleHeight + Padding;

                GUI.Box(
                    new Rect(windowArea.x + Padding, windowArea.y + y, width - 2 * Padding, descriptionBoxHeight), 
                    SelectedLandingSite.Description,
                    descriptionStyle);
                y += descriptionBoxHeight + Padding;

                if (GUI.Button(
                    new Rect(windowArea.x + Padding, windowArea.y + y, (width - 2 * Padding), buttonHeight),
                    "Start"))
                {
                    Game.Game.Instance.SwitchToLevelView();
                }
            }
        }

        private void DrawLeftWindow()
        {
            var width = 300;
            var windowArea = new Rect(Padding, Padding, width, Screen.height - 2 * Padding);

            GUI.DrawTexture(windowArea, LeftBackground);

            float itemSpacing = 0;

            float y = itemSpacing;
            var itemHeight = 24;

            Rect itemArea = new Rect(windowArea.x + Padding, Padding + windowArea.y + y, windowArea.width - 2 * Padding, itemHeight);
            LandingSiteListItem.DrawHeader(itemArea);

            foreach (var site in ActivePlanet.LandingSites)
            {
                y += itemHeight + itemSpacing;
                itemArea = new Rect(windowArea.x + Padding + itemSpacing, windowArea.y + y + Padding, windowArea.width - 2 * Padding,
                    itemHeight);
                LandingSiteListItem.Draw(itemArea, site);
            }
        }

        private static int currentDifficulty = 0;
        private static int levelsPerDifficutly = 0;

        public void CreateNewGame(GameSettings settings)
        {
            currentDifficulty = 0;
            levelsPerDifficutly = 0;

            Planets = new List<Planet>();
            for (int i = 0; i < settings.NumberOfPlanets; i++)
            {
                Planet planet = CreateNewPlanet(settings);
                Planets.Add(planet);
            }
        }

        public Planet CreateNewPlanet(GameSettings settings)
        {
            Planet newPlanet = InstantiateRandomPlanet();
            for (int i = 0; i < settings.NumberOfLevels; i++)
            {
                if (levelsPerDifficutly >= settings.LevelsPerDifficultySetting)
                {
                    levelsPerDifficutly = 0;
                    currentDifficulty++;
                }
                levelsPerDifficutly++;

                newPlanet.AddLandingSite(currentDifficulty < 2, currentDifficulty);
            }

            return newPlanet;
        }
    }
}
