using System;

namespace Assets.RTSCore.Inventory
{
	[Serializable]
    public class Item
    {
        public Item(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }

		public Item()
        {
            _percentPerCapacity = 1;
        }

		public Item(Item copy) 
		{
			Name = copy.Name;
			Amount = copy.Amount;
		}

		public string Name;
		public int Amount;

        public float PercentOfCapacity
        {
            get
            {
                if (_percentPerCapacity < 0) { _percentPerCapacity = 0; }
                if (_percentPerCapacity > 1) { _percentPerCapacity = 1; }
                return _percentPerCapacity;
            }
            set
            {
                _percentPerCapacity = value;
            }
        } float _percentPerCapacity;
    }
}
