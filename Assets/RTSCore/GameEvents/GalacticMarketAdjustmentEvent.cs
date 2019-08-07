using System;

namespace Assets.RTSCore.GameEvents
{
    public class GalacticMarketAdjustmentEvent : GameEvent
    {
        public string ItemEffected;
        public float PriceAdjustment;
        public float AvailabilityAdjustment;

        private int originalPrice;
        private int originalAvailability;

        public override string Outcome
        {
            get
            {
                return String.Format("Price of {0} changed by {1}%", ItemEffected, 100*(1-PriceAdjustment));
            }
        }

        public override void ApplyEventEffects()
        {
            //foreach (MarketItem item in CurrentLevel.GalacticMarketBoard.Items)
            //{
            //    if (item.Name == ItemEffected)
            //    {
            //        originalAvailability = item.AnnualMax;
            //        originalPrice = item.BasePrice;
            //        item.AnnualMax = (int)((float)item.AnnualMax * AvailabilityAdjustment);
            //        item.BasePrice = (int)((float)item.BasePrice * PriceAdjustment);
            //        return;
            //    }
            //}
        }

        public override void RemoveEventEffects()
        {
            //foreach (MarketItem item in CurrentLevel.GalacticMarketBoard.Items)
            //{
            //    if (item.Name == ItemEffected)
            //    {
            //        item.AnnualMax = originalAvailability;
            //        item.BasePrice = originalPrice;
            //        return;
            //    }
            //}
        }
    }
}
