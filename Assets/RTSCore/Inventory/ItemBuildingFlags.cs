using System;

namespace Assets.RTSCore.Inventory
{
	[Serializable]
	public class ItemBuildingFlags
	{
		public string Name;

		public bool Keep;
		public bool Request;
		public bool Producible;
		public bool RequireHarvestToTake;
		public bool SelfSufficient;
	}
}
