using System;
using Assets.RTSCore.Economy;
using Assets.RTSCore.Game;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Misc;

namespace Assets.RTSCore.WorldObject.Buildings
{
    public class GalacticMarket : Building
    {
        private PeriodicEvent _sellGoodsPeriodicEvent;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            _sellGoodsPeriodicEvent = new PeriodicEvent(60, SellGoods);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            _sellGoodsPeriodicEvent.Update(GameTimeManager.DeltaTime);

            ItemFlags.Clear();
            if (Game.Game.Instance.ActiveLevel)
            {
                foreach (MarketItem item in Game.Game.Instance.Economy.GallacticMarketItems)
                {
                    if (item.Exporting)
                    {
                        ItemBuildingFlags flags = new ItemBuildingFlags();
                        flags.Name = item.Name;
                        flags.Keep = true;
                        flags.Request = true;
                        ItemFlags.Add(flags);
                    }
                }
            }
        }

        private void SellGoods(object sender, EventArgs e)
        {
            foreach (Item item in MyInventory.Items)
            {
                if (item.Amount == 0) { continue; }
                
                foreach (MarketItem marketItem in Game.Game.Instance.Economy.GallacticMarketItems)
                {
                    if (marketItem.Name != item.Name) { continue; }

                    int price = marketItem.Price;

                    Game.Game.Instance.ActivePlayer.Money += item.Amount * price;
                    item.Amount = 0;
                }
                
            }
        }
    }
}
