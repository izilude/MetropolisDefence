using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Assets.RTSCore.Data;
using Assets.RTSCore.Level;
using Assets.RTSCore.TerrainObjects;
using Assets.RTSCore.WorldObject.Buildings;
using UnityEngine;

namespace Assets.RTSCore.Game
{
    public class GameConfiguration : MonoBehaviour
    {

        public void LoadConfiguration()
        {
            string gameConfigPath = Path.Combine(Environment.CurrentDirectory, "Assets/RTSCore/Data/GameConfig.xml");
            var gameConfig = XmlSerializationTools.ReadFromFile<List<ItemConversion>>(gameConfigPath);

            for (int i = gameConfig.Count - 1; i >= 0; i--)
            {
                if (String.IsNullOrEmpty(gameConfig[i].ItemProduced)) gameConfig.RemoveAt(i);
            }

            Conversions = gameConfig;
        }

        public List<Building> Buildings = new List<Building>();

        public List<ItemConversion> Conversions = new List<ItemConversion>();

        public List<TerrainObject> TerrainObjects = new List<TerrainObject>();

        public List<GameResource> ResourceConfigurations = new List<GameResource>();

        private const float ProductionMarkUp = 0.10f;
        private const int SecondsPerUnitCost = 5;
        private const int ItemPerUnitCost = 1000;

        public List<ItemConversion> RequestConversions(string itemName)
        {
            var conversions = new List<ItemConversion>();

            foreach (var conversion in Conversions)
            {
                if (conversion.ItemProduced == itemName)
                {
                    conversions.Add(conversion);
                }
            }

            return conversions;
        }

        public int GetBasePriceOfItem(string itemName)
        {
            var itemConversion = Conversions.FirstOrDefault(x => x.ItemProduced == itemName);
            if (itemConversion == null)
            {
                Debug.Log(String.Format("{0} not found in item conversion chart!", itemName));
                return 0;
            }

            // temporary catch all for undefined items.
            if (itemConversion.AmtProd < 1) itemConversion.AmtProd = 10;
            if (itemConversion.BuildTime < 1) itemConversion.BuildTime = SecondsPerUnitCost;

            int basePrice = 0;
            if (itemConversion.ItemsNeedForConversion().Count == 0)
            {
                basePrice = (int)(((float)itemConversion.BuildTime / SecondsPerUnitCost)
                    * ((float)ItemPerUnitCost / itemConversion.AmtProd ));

                if (basePrice < 1) basePrice = 1;

                return basePrice;
            }

            foreach (var neededItem in itemConversion.ItemsNeedForConversion())
            {
                if (neededItem.Amount < 1) neededItem.Amount = 5;
                basePrice += neededItem.Amount*GetBasePriceOfItem(neededItem.Name);
            }

            basePrice = (int) ((1+ProductionMarkUp) * (basePrice / itemConversion.AmtProd + itemConversion.BuildTime/ SecondsPerUnitCost));

            return basePrice;
        }
    }
}
