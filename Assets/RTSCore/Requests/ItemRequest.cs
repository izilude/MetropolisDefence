using System;
using System.Collections.Generic;
using Assets.RTSCore.AI;
using Assets.RTSCore.Interfaces;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Misc;
using Assets.RTSCore.WorldObject.Buildings;
using Assets.RTSCore.WorldObject.Units;

namespace Assets.RTSCore.Requests
{
	public class ItemRequest : Request
	{
		public ItemRequest() {}

		public ItemRequest(string itemName, int amount, Building initiator, PriorityLevel priority, bool initiateEvenIfNotWorking, int id) :
			base(initiator, priority, initiateEvenIfNotWorking, id)
		{
			Name = itemName;
			Amount = amount;
		}

		public WorldObject.WorldObject ProviderOfItem;
		public WorldObject.WorldObject TransporterOfItem;

		private float _transporterBid;

		public string Name;
		public int Amount;
		public bool Retrieve;
		public bool Distribute;
		public bool Harvest;

		public override bool Equals (object obj)
		{
			bool parent =  base.Equals (obj);

			if (obj is ItemRequest) 
			{
				ItemRequest request = obj as ItemRequest;
				return parent && request.Name == this.Name;
			}

			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public override void Initiate() 
		{
			TransporterOfItem.Requests.AddRequest(this);
		}

		public override string Info ()
		{
			string line = string.Empty;
			if (ProviderOfItem != null) 
			{
				line += " ";
				line += ProviderOfItem.Name;
			} 

			if (TransporterOfItem != null) 
			{
				line += " ";
				line += TransporterOfItem.Name;
			}

			line += String.Format(" {0} {1}", Name, Amount);

			return line;
		}

		public override bool CanMeetRequest (List<WorldObject.WorldObject> worldObjects)
		{
			_possibleProvidersOfItem.Clear();
			_possibleProvidersOfItemBids.Clear();
			ProviderOfItem = null;

			foreach (WorldObject.WorldObject worldObject in worldObjects) 
			{
				SeeIfWorldObjectCanProvideItem (worldObject);
			}

			foreach (WorldObject.WorldObject provider in _possibleProvidersOfItem) 
			{
				ProviderOfItem = provider;

			    var building = InitiatorOfRequest;
			    if (building != null && building.IsSelfSufficient(Name)) 
				{
					SeeIfWorldObjectCanTransportItem(building);
				} 
				else 
				{
					foreach (WorldObject.WorldObject worldObject in worldObjects) 
					{
						SeeIfWorldObjectCanTransportItem (worldObject);
					}
				}

				if (TransporterOfItem != null) {break;}
			}

			bool requestReady = (TransporterOfItem != null && ProviderOfItem != null);
			bool requireHarvesting = false;
			if (ProviderOfItem is Building && (ProviderOfItem as Building).RequireHarvestingToTake(Name)) {requireHarvesting = true;}
			if (requestReady)
			{
				Retrieve = TransporterOfItem.Equals(InitiatorOfRequest) && !requireHarvesting;
				Harvest = TransporterOfItem.Equals(InitiatorOfRequest) && requireHarvesting;
				Distribute = TransporterOfItem.Equals(ProviderOfItem);
			}

			return requestReady;
		}

		void SeeIfWorldObjectCanTransportItem (WorldObject.WorldObject worldObject)
		{
			float bid;
			int pathlength;
		    float pathScore;
			if (!CanTransportItem (worldObject, out bid))
			{
			}
			else if (TransporterOfItem != null && _transporterBid < bid) 
			{
			}
			else if (!PathFinding.DoesPathExistBetweenWorldObjects (
                InitiatorOfRequest, 
                worldObject, 
                out pathlength,
                out pathScore)) 
			{
			}
			else 
			{
				bid += pathScore;
				TransporterOfItem = worldObject;
				_transporterBid = bid;
			}
		}

		List<WorldObject.WorldObject> _possibleProvidersOfItem = new List<WorldObject.WorldObject>();
		List<float> _possibleProvidersOfItemBids = new List<float>();
		void SeeIfWorldObjectCanProvideItem (WorldObject.WorldObject worldObject)
		{
			float bid;
		    float pathScore;
			int pathlength;

			if (worldObject.Name == InitiatorOfRequest.Name)
			{
				return;
			}
			else if (!CanProvideItem (worldObject, out bid)) 
			{
				return;
			}
			else if (!PathFinding.DoesPathExistBetweenWorldObjects (InitiatorOfRequest, worldObject, out pathlength, out pathScore)) 
			{
				return;
			}
			else 
			{
				bid += pathScore;
				bool addNew = true;
				for(int i=0;i<_possibleProvidersOfItem.Count;i++) 
				{
					if (bid < _possibleProvidersOfItemBids[i])
					{
						_possibleProvidersOfItem.Insert(i, worldObject);
						_possibleProvidersOfItemBids.Insert(i, bid);
						addNew = false;
						break;
					}
				}

				if (addNew) 
				{
					_possibleProvidersOfItem.Add (worldObject);
					_possibleProvidersOfItemBids.Add (bid);
				}
			}
		}

		public override bool CanMeetRequest(Unit unit) 
		{
			float bid = 0;
			return (CanTransportItem(unit, out bid));
		}

		private bool CanProvideItem(WorldObject.WorldObject worldObject, out float bid) 
		{
			bid = 0;
			Item item = worldObject.MyInventory.GetItem(Name);

			if (worldObject.Equals(InitiatorOfRequest)) {return false;}
			if (worldObject.WantsToKeepItem(Name)) {return false; }

			if (item.Amount == 0) 
			{
				return false;
			} 
			else 
			{
				//bid = Amount < item.Amount ? Amount : item.Amount;
				bid = -item.Amount;
				return true;
			}
		}

		private bool CanTransportItem(WorldObject.WorldObject worldObject, out float bid) 
		{
			if (worldObject is Building)
			{
				return CanTransportItem(worldObject as Building, out bid);
			}
			else if (worldObject is Unit)
			{
				return CanTransportItem(worldObject as Unit, out bid);
			}

			bid = 0;
			return false;
		}

		private bool CanTransportItem(Unit unit, out float bid)
		{
			bid = 0;
			if (Retrieve && unit.CanRetrieveItem(Name))
			{
				return true;
			} 
			else if(Harvest && unit.CanHarvestItem(Name))
			{
				return true;
			}
			else if(Distribute && unit.CanDistributeItem(Name))
			{
				return true;
			} 
			else if(unit.CanRetrieveItem(Name) && unit.CanDistributeItem(Name)) 
			{
				return true;
			}
			
			return false;
		}

		private bool CanTransportItem(Building worldObject, out float bid)
		{
			bid = 0;

			bool requireHarvesting = false;
			if (ProviderOfItem is Building && (ProviderOfItem as Building).RequireHarvestingToTake(Name)) {requireHarvesting = true;}

			if (!requireHarvesting && worldObject.Equals(InitiatorOfRequest)&& worldObject.CanRetrieveItem(Name)) 
			{
				return GetWorldObjectBid (worldObject, requireHarvesting, out bid);
			}
			else if (requireHarvesting && worldObject.Equals(InitiatorOfRequest)&& worldObject.CanHarvestItem(Name)) 
			{
				return GetWorldObjectBid (worldObject, requireHarvesting, out bid);
			}
			else if (worldObject.Equals(ProviderOfItem) && worldObject.CanDistributeItem(Name)) 
			{
				return GetWorldObjectBid (worldObject, requireHarvesting, out bid);
			} 
			else if (worldObject.CanDistributeAndRetrieveItem(Name)) 
			{
				return GetWorldObjectBid (worldObject, requireHarvesting, out bid);
			}

			return false;
		}

		bool GetWorldObjectBid (WorldObject.WorldObject worldObject, bool requireHarvesting, out float bid)
		{
			bid = 0;

		    if (!(worldObject is ITransaction)) return false;

			if (!requireHarvesting) 
			{
				if (worldObject.Equals (InitiatorOfRequest)) 
				{
					bid = -1;
				}
				return true;
			}

		    if (((ITransaction) worldObject).CanHarvestItem (Name)) 
		    {
		        if (worldObject.Equals (InitiatorOfRequest)) 
		        {
		            bid = -1;
		        }
		        return true;
		    }

		    return false;
		}
	}
}

