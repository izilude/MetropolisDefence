using System;
using Assets.RTSCore.Inventory;

namespace Assets.RTSCore.Economy
{
    [Serializable]
    public class MarketItem : Item
    {
        private const float MaxDemand = 2f;
        private const float MinDemand = 0.5f;
        private const float Center = MinDemand + (MaxDemand + MinDemand) / 2;
        private const float Delta = (MaxDemand - MinDemand)/6;

        public MarketItem(int basePrice)
        {
            _demand = 1;
            _basePrice = basePrice;
        }

        private int _basePrice;

        private float _demand;
        public float Demand
        {
            get
            {
                return _demand;
            }
            set
            {
                _demand = value;
                if (_demand < MinDemand) _demand = MinDemand;
                if (_demand > MaxDemand) _demand = MaxDemand;
            }
        }

        public string DemandText
        {
            get
            {
                if (Demand > Center + 2*Delta)
                {
                    return "Very High";
                }
                else if (Demand > Center + Delta)
                {
                    return "High";
                } 
                else if (Demand < Center - 2*Delta)
                {
                    return "Very Low";
                }
                else if (Demand < Center - Delta)
                {
                    return "Low";
                }
                else
                {
                    return "Normal";
                }
            }
        }

        public int Price
        {
            get
            {
                return (int)(_basePrice * Demand);
            }
        }

        public bool Exporting;
    }
}
