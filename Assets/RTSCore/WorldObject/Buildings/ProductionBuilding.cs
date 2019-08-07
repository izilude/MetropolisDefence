using System.Collections.Generic;
using Assets.RTSCore.Game;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Level;
using Assets.RTSCore.Misc;
using UnityEngine;

namespace Assets.RTSCore.WorldObject.Buildings
{
	public class ProductionBuilding : Building
	{
        public float TextHeight = 4.5f;
        public ItemConversion CurrentConversion
        {
            get
            {
                return _currentConversion;
            }
            set
            {
                _currentConversion = value;

                if (_currentConversion != null)
                {
                    foreach (Item neededItem in _currentConversion.ItemsNeedForConversion())
                    {
                        bool found = false;
                        foreach (ItemBuildingFlags item in ItemFlags)
                        {
                            if (item.Name == neededItem.Name)
                            {
                                item.Keep = true;
                                item.Request = true;
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                        {
                            ItemBuildingFlags newItem = new ItemBuildingFlags();
                            newItem.Name = neededItem.Name;
                            newItem.Keep = true;
                            newItem.Request = true;
                            newItem.Producible = false;
                            newItem.SelfSufficient = false;
                            newItem.RequireHarvestToTake = false;
                            ItemFlags.Add(newItem);
                        }
                    }
                }
            }
        } ItemConversion _currentConversion;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            _productionOrders = new Queue<ProductionOrder>();

            AutoSetProductionOrder();

            textGameObject = new GameObject();
            textGameObject.transform.parent = this.transform;
            textGameObject.transform.position = new Vector3(transform.position.x + 1, transform.position.y + TextHeight, transform.position.z + 1);
            textGameObject.transform.localEulerAngles = new Vector3(30, 30, 0);

            ProductionTextMesh = textGameObject.gameObject.AddComponent(typeof(TextMesh)) as TextMesh;
            ProductionTextMesh.text = string.Empty;
            ProductionTextMesh.characterSize = 0.25f;
            ProductionTextMesh.anchor = TextAnchor.MiddleCenter;

            ProductionMeshRenderer = textGameObject.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        }

        private void AutoSetProductionOrder()
        {
            ItemBuildingFlags selectedItem = null;
            foreach (ItemBuildingFlags item in ItemFlags)
            {
                if (item.Producible)
                {
                    selectedItem = item;
                    break;
                }
            }

            if (selectedItem == null) { return; }

            List<ItemConversion> conversions = Game.Game.Instance.Configuration.RequestConversions(selectedItem.Name);
            if (conversions != null && conversions.Count > 0)
            {
                CurrentConversion = conversions[0];
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            UpdateProductionOrder();

            ProductionTextMesh.text = string.Empty;
            if (Game.Game.Instance.ActiveLevel.Hud != null 
                && Game.Game.Instance.ActiveLevel.Hud.OverlayProduction 
                && CurrentConversion != null)
            {
                textGameObject.transform.LookAt(2 * textGameObject.transform.position 
                    - Game.Game.Instance.MainCamera.transform.position);
                ProductionTextMesh.text = CurrentConversion.ItemProduced;
            }
        }

        private GameObject textGameObject { get; set; }
        private TextMesh ProductionTextMesh { get; set; }
        private MeshRenderer ProductionMeshRenderer { get; set; }

        public List<string> ProducibleItems
		{
			get
			{
				List<string> items = new List<string>();
				foreach (ItemBuildingFlags item in ItemFlags)
				{
					if (item.Producible) 
					{
						items.Add (item.Name);
					}
				}
				return items;
			}
		}

        public float ProductionProgressPercent
        {
            get
            {
                if (_currentProductionOrder == null) { return 0; }
                return _currentProductionOrder.TotalTime / _currentProductionOrder.BuildTime;
            }
        }

        public string ProductionItem
        {
            get
            {
                if (_currentProductionOrder == null) { return string.Empty; }
                return _currentProductionOrder.ItemProduced;
            }
        }

        private Queue<ProductionOrder> _productionOrders;
		private ProductionOrder _currentProductionOrder;

		private void UpdateProductionOrder()
        {
            if (_currentProductionOrder == null && _productionOrders.Count > 0)
            {
                _currentProductionOrder = _productionOrders.Dequeue();
            }
            else if (_currentProductionOrder == null && _productionOrders.Count == 0)
            {
                if (CurrentConversion != null)
                {
                    TryCreateProductionOrder(CurrentConversion);
                }
            }

            if (_currentProductionOrder != null && State == BuildingState.Working)
            {
                _currentProductionOrder.TotalTime += GameTimeManager.DeltaTime;

                if (_currentProductionOrder.TotalTime > _currentProductionOrder.BuildTime)
                {
                    MyInventory.AddItems(_currentProductionOrder.ItemProduced, _currentProductionOrder.AmtProd);

                    _currentProductionOrder = null;
                }
            }
        }

		public bool TryCreateProductionOrder(ItemConversion conversion)  
		{
			ProductionOrder newProductionOrder = new ProductionOrder ();

			foreach (Item item in conversion.ItemsNeedForConversion()) 
			{
				if (MyInventory.GetItemAmount(item.Name) < item.Amount) 
				{
					return false;
				}
			}

			foreach (Item item in conversion.ItemsNeedForConversion()) 
			{
				MyInventory.SubtractItems(item.Name, item.Amount);
				newProductionOrder.EarMarkedItems.AddItems(item.Name, item.Amount);
			}

			newProductionOrder.AmtProd = conversion.AmtProd;
			newProductionOrder.BuildTime = conversion.BuildTime;
			newProductionOrder.ItemProduced = conversion.ItemProduced;
			_productionOrders.Enqueue(newProductionOrder);
			
			return true;
		}
	}
}
