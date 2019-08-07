using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.RTSCore.Inventory
{
	[Serializable]
    public class Inventory
    {
		public int Capacity;
		public List<Item> Items;

		private Dictionary<string, Item> _itemDictionary;
		private Dictionary<string, Item> ItemDictionary
		{
			get 
			{
				if (_itemDictionary == null) 
				{
					_itemDictionary = new Dictionary<string, Item>();
					
					if (Items != null) 
					{
						foreach (Item item in Items) 
						{
							_itemDictionary.Add (item.Name, item);
						}
					}
				}

				return _itemDictionary;
			}
		}

		public Inventory()
		{
		}

		public Inventory(Inventory copy) 
		{
			Capacity = copy.Capacity;
			Items = new List<Item>();
			_itemDictionary = null;
			foreach (Item item in copy.Items) 
			{
				Items.Add (new Item(item));
			}
		}

		public bool ContainsTheseItems(List<Item> items) 
		{
			foreach (Item item in items)
			{
				int amount = GetItemAmount(item.Name);
				if (amount < item.Amount) 
				{
					return false;
				}
			}

			return true;
		}

		public bool TrySubtractItems(List<Item> itemsToSubtract) 
		{
			if (itemsToSubtract == null) { return true; }

			foreach (Item item in itemsToSubtract) 
			{
				SubtractItems(item.Name, item.Amount);
			}
			return true;
		}

		public bool TryAddItems(List<Item> itemsToAdd) 
		{
			foreach (Item item in itemsToAdd) 
			{
				AddItems(item.Name, item.Amount);
			}
			return true;
		}

		public bool ItemAvailableToGet(string itemName) 
		{
			Item item;
			if (ItemDictionary.TryGetValue(itemName, out item)) {
				return item.Amount > 0;
			}

			return false;
		}

		public int AddItems(string nameOfItem, int quantity)
		{
			int overflow = 0;
			if (quantity < 0) { return 0; }

			Item item;
			if (ItemDictionary.TryGetValue(nameOfItem, out item)) 
			{
				item.Amount += quantity;
			} 
			else 
			{
				item = new Item();
				item.Amount = quantity;
				item.Name = nameOfItem;
				ItemDictionary.Add (nameOfItem, item);

				if (Items == null) 
				{
					Items = new List<Item>();
				}
				Items.Add (item);

			}

			if (item.Amount > Capacity) {
				overflow = item.Amount - Capacity;
				item.Amount = Capacity;
			}

			return overflow;
		}
		
		public int SubtractItems(string nameOfItem, int quantity)
		{
			if (quantity < 0) { return 0; }

			Item item;
			if (ItemDictionary.TryGetValue(nameOfItem, out item)) {
				int amountTaken = item.Amount > quantity ? quantity : item.Amount;
				item.Amount -= quantity;
				item.Amount = item.Amount < 0 ? 0 : item.Amount;
				return amountTaken;
			}

			return 0;
		}
		
		public void SetItems(string nameOfItem, int quantity, out int overflow)
		{
			overflow = 0;
			if (quantity < 0) { return; }

			Item item;
			if (ItemDictionary.TryGetValue(nameOfItem, out item)) {
				item.Amount = quantity;
			} else {
				item = new Item();
				item.Amount = quantity;
				item.Name = nameOfItem;
				ItemDictionary.Add (nameOfItem, item);
			}

			overflow = 0;
			if (item.Amount > Capacity) {
				overflow = item.Amount - Capacity;
				item.Amount = Capacity;
			}
		}

		public List<string> ItemsInInventory { get {return ItemDictionary.Keys.ToList(); } }
		
		public Item GetItem(string nameOfItem)
		{
			Item item = null;
			if (!string.IsNullOrEmpty(nameOfItem) &&
                ItemDictionary != null &&
                !ItemDictionary.TryGetValue(nameOfItem, out item)) 
			{
				item = new Item();
				item.Amount = 0;
				item.Name = nameOfItem;
				ItemDictionary.Add (nameOfItem, item);
				Items.Add (item);
			}
			return item;
		}

		public int GetItemAmount(string itemName) 
		{
			Item item = null;
			if (!String.IsNullOrEmpty(itemName) 
                && ItemDictionary.TryGetValue(itemName, out item)) {
				return item.Amount;
			}
		    return 0;
		}
    }
}
