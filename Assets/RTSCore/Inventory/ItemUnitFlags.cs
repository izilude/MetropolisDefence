using System;

namespace Assets.RTSCore.Inventory
{
	[Serializable]
	public class ItemUnitFlags
	{
		public string Name;

		public bool Distribute;
		public bool Retrieve;
		public bool Harvest;
	}
}
