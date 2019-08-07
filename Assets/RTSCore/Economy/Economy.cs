using System.Collections.Generic;
using System.Linq;
using Assets.RTSCore.Inventory;

namespace Assets.RTSCore.Economy
{
    public class Economy
    {
        public List<MarketItem> GallacticMarketItems = new List<MarketItem>();

        public void UpdateGallacticMarketItems()
        {
            List<Item> allExportedItems = new List<Item>();
            List<Item> allImportedItems = new List<Item>();

            var planets = Game.Game.Instance.WorldMap.Planets;
            foreach (var planet in planets)
            {
                foreach (var landingSite in planet.LandingSites)
                {
                    if (landingSite.Level == null) continue;

                    var exportedItems = landingSite.Level.ItemsBeingExported();
                    var importedItems = landingSite.Level.ItemsBeingImported();

                    AddItemsToList(exportedItems, allExportedItems);
                    AddItemsToList(importedItems, allImportedItems);
                }
            }

            // Stuff the colony ship wants
            foreach (var colonyShipItem in Game.Game.Instance.Configuration.Conversions)
            {
                allImportedItems.Add(new Item(colonyShipItem.ItemProduced, 25));
            }

            GallacticMarketItems.Clear();
            foreach (var import in allImportedItems)
            {
                var basePrice = Game.Game.Instance.Configuration.GetBasePriceOfItem(import.Name);
                var export = allExportedItems.FirstOrDefault(x => x.Name == import.Name);
                var demand = export == null ? 10000 : (float) import.Amount / export.Amount;
                GallacticMarketItems.Add(new MarketItem(basePrice) {Demand = demand, Name = import.Name});

                if (export!= null) allExportedItems.Remove(export);
            }

            foreach (var export in allExportedItems)
            {
                var basePrice = Game.Game.Instance.Configuration.GetBasePriceOfItem(export.Name);

                var demand = 0;
                GallacticMarketItems.Add(new MarketItem(basePrice) { Demand = demand, Name = export.Name});
            }
        }

        private static void AddItemsToList(List<Item> itemsToAdd, List<Item> allItems)
        {
            foreach (Item exportedItem in itemsToAdd)
            {
                var export = allItems.FirstOrDefault(x => x.Name == exportedItem.Name);
                if (export == null)
                {
                    allItems.Add(exportedItem);
                }
                else
                {
                    export.Amount += exportedItem.Amount;
                }
            }
        }
    }
}
