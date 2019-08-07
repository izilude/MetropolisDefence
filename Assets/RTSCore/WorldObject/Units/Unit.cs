using System.Collections.Generic;
using Assets.RTSCore.AI;
using Assets.RTSCore.Game;
using Assets.RTSCore.Interfaces;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Map;
using Assets.RTSCore.Misc;
using Assets.RTSCore.Requests;
using Assets.RTSCore.Services;
using UnityEngine;

namespace Assets.RTSCore.WorldObject.Units
{
	public class Unit : MovingWorldObject, ITransaction
	{
		public List<string> Services;
		public List<ItemUnitFlags> ItemFlags;

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

			if (State == UnitState.Stopped) 
			{
				Request request = Requests.NextRequest(true);
				if (request != null) 
				{
					InitiateRequest(request);
					State = UnitState.Walking;
				}
			}
			else if (State == UnitState.Walking)
            {
				if (ArrivedAtWaypoint && CurrentRequest.State == RequestState.InProcess) 
				{
					FillRequest();
				} 
				else if (ArrivedAtWaypoint && CurrentRequest.State == RequestState.Filled) 
				{
					CompleteRequest();
				}
            }
			else if(State == UnitState.ProvidingService) 
			{
                _provideServiceTime += GameTimeManager.DeltaTime;
                if (_provideServiceTime >= _provideServiceTimeToComplete) 
				{
					FillRequest();
					CurrentRequest.State = RequestState.Filled;
				}
			}
        }

		public bool CanRetrieveItem (string itemName)
		{
			if (ItemFlags == null) {return false;}

			foreach (ItemUnitFlags name in ItemFlags) 
			{
				if (name.Name == itemName) 
				{
					return name.Retrieve;
				}
			}

			return false;
		}

		public bool CanDistributeItem (string itemName)
		{
			if (ItemFlags == null) {return false;}
			
			foreach (ItemUnitFlags name in ItemFlags) 
			{
				if (name.Name == itemName) 
				{
					return name.Distribute;
				}
			}
			
			return false;
		}

		public bool CanHarvestItem(string itemName) 
		{
			if (ItemFlags == null) {return false;}

			foreach (ItemUnitFlags name in ItemFlags) 
			{
				if (name.Name == itemName) 
				{ 
					return name.Harvest; 
				}
			}

			return false;
		}

		public bool CanDistributeAndRetrieveItem (string itemName)
		{
			return CanDistributeItem(itemName) && CanRetrieveItem(itemName);
		}

		public List<string> ServicesProvided ()
		{
			return Services;
		}

		private void FillRequest() 
		{
			if (CurrentRequest is ItemRequest) 
			{
				FillItemRequest(CurrentRequest as ItemRequest);
				CurrentRequest.State = RequestState.Filled;
			} 
			else if (CurrentRequest is ServiceRequest) 
			{
				FillServiceRequest(CurrentRequest as ServiceRequest);
			}
		}

		private float _provideServiceTime;
		private float _provideServiceTimeToComplete;
		private void FillServiceRequest(ServiceRequest request) 
		{
            Service service = null;
            foreach (string s in Services)
            {
                if (s == request.ServiceNeeded.Name)
                {
                    service = Game.Game.Instance.ActiveLevel.ServiceChart.GetService(s);
                }
            }

			if (State != UnitState.ProvidingService) 
			{
				_provideServiceTimeToComplete = service.TimeToComplete;
                _provideServiceTime = 0;
				State = UnitState.ProvidingService;
			} 
			else 
			{
				MyInventory.TrySubtractItems(service.Cost);
				request.DeliverService(service.MaxTime);
				State = UnitState.Walking;
			}
		}

		private void FillItemRequest(ItemRequest request) 
		{
			int amountTaken = request.ProviderOfItem.MyInventory.SubtractItems(request.Name, request.Amount);
			int excess = MyInventory.AddItems(request.Name, amountTaken);
			request.ProviderOfItem.MyInventory.AddItems(request.Name, excess);
		}

		private void CompleteRequest() 
		{
			if (CurrentRequest is ItemRequest) 
			{
				CompleteItemRequest(CurrentRequest as ItemRequest);
			}
			else if (CurrentRequest is ServiceRequest) 
			{
				CompleteServiceRequest(CurrentRequest as ServiceRequest);
			}

			CurrentRequest.State = RequestState.Complete;
			State = UnitState.Finished;
		}

		private void CompleteServiceRequest(ServiceRequest request) 
		{
			;
		}

		private void CompleteItemRequest(ItemRequest request) 
		{
			int amountTaken = MyInventory.SubtractItems(request.Name, request.Amount);
			int excess = request.InitiatorOfRequest.MyInventory.AddItems(request.Name, amountTaken);
			MyInventory.AddItems(request.Name, excess);
		}

		private void InitiateRequest(Request request) 
		{
			CurrentRequest = request;
			if (request is ItemRequest) 
			{
				InitiateItemRequest(request as ItemRequest);
			} 
			else if (request is ServiceRequest)
			{
				InitiateServiceRequest(request as ServiceRequest);
			}
		}

		private void InitiateServiceRequest(ServiceRequest request)
		{
            Service service = null;
            foreach (string s in Services)
            {
                if (s == request.ServiceNeeded.Name)
                {
                    service = Game.Game.Instance.ActiveLevel.ServiceChart.GetService(s);
                }
            }

			bool success = request.ServiceProvider.MyInventory.TrySubtractItems(service.Cost);

			List<MonoBehaviour> worldObjects = new List<MonoBehaviour>();
			worldObjects.Add (request.InitiatorOfRequest);
			worldObjects.Add (request.ServiceProvider);

            if (success) 
			{
				MyInventory.TryAddItems(service.Cost);
			    List<Tile> wayPoints;
				PathFinding.TryGetUnitPath(this, worldObjects, out wayPoints);

			    WayPoints = wayPoints;
			} 
			else 
			{
				request.State = RequestState.Complete;
			}
		}

		private void InitiateItemRequest(ItemRequest request) 
		{
			List<MonoBehaviour> worldObjects = new List<MonoBehaviour>();
			worldObjects.Add (request.ProviderOfItem);
			worldObjects.Add (request.InitiatorOfRequest);
			worldObjects.Add (request.TransporterOfItem);

			List<Tile> wayPoints;
			PathFinding.TryGetUnitPath(this, worldObjects, out wayPoints);

			WayPoints = wayPoints;
		}
    }
}
