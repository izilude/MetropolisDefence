using System.Collections.Generic;
using UnityEngine;

namespace Assets.RTSCore.WorldMap
{
    public class LandingSite : MonoBehaviour
    {
        public void GenerateRandom(int difficultyRating)
        {
            Name = GetRandomName();
            Description = GetRandomDescription();
            DifficultyRating = difficultyRating;
            CostToLand = GetCostToLand();
            ResourcesAvailable = GetResourcesAvailable();
            GetRandomPosition();
        }

        public Level.Level Level { get; set; }
        public Texture2D Background;
        public string Name;
        public string Description;
        public string Region;
        public int CostToLand;
        public List<string> ResourcesAvailable = new List<string>();
        public int DifficultyRating;

        public GameObject NormalDisplay;
        public GameObject SelectedDisplay;

        private static string GetRandomName()
        {
            return "LandingSite";
        }

        private static string GetRandomDescription()
        {
            return "This is a random description";
        }

        private static int GetCostToLand()
        {
            return 1000;
        }

        private static List<string> GetResourcesAvailable()
        {
            return new List<string>();
        }

        private void GetRandomPosition()
        {
            transform.localPosition = UnityEngine.Random.onUnitSphere/2;
        }
    }
}
