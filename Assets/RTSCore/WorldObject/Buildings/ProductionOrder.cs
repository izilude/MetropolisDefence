using Assets.RTSCore.Level;

namespace Assets.RTSCore.WorldObject.Buildings
{
	public class ProductionOrder : ItemConversion
	{
		public float TotalTime;
		public Inventory.Inventory EarMarkedItems;
		
		public ProductionOrder()
		{
			EarMarkedItems = new Inventory.Inventory();
		}
	}
}

