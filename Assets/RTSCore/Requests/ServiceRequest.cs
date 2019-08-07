using System;
using System.Collections.Generic;
using Assets.RTSCore.AI;
using Assets.RTSCore.Information;
using Assets.RTSCore.Interfaces;
using Assets.RTSCore.Misc;
using Assets.RTSCore.Services;
using Assets.RTSCore.WorldObject.Buildings;
using Assets.RTSCore.WorldObject.Units;

namespace Assets.RTSCore.Requests
{
	public class ServiceRequest : Request
	{
		public Service ServiceNeeded;

		public WorldObject.WorldObject ServiceProvider;
		private float _providerBid;

		public ServiceRequest(Service serviceName, Building initiator, PriorityLevel priority, bool initiateEvenIfNotWorking, int id)
			: base(initiator, priority, initiateEvenIfNotWorking, id)
		{
            ServiceNeeded = serviceName;
		}

		public void DeliverService(float timeToAdd) 
		{
            InitiatorOfRequest.AddTimeToActiveService(ServiceNeeded, timeToAdd);
        }

		public override bool CanMeetRequest (List<WorldObject.WorldObject> worldObjects)
		{
		    foreach (WorldObject.WorldObject worldObject in worldObjects)
			{
			    float bid;
			    int pathlength;
			    float pathScore;

                if (!CanProvideService(worldObject, out bid)) continue;
			    if (ServiceProvider != null && bid >= _providerBid) continue;
	
			    if (!PathFinding.DoesPathExistBetweenWorldObjects(InitiatorOfRequest, worldObject, out pathlength, out pathScore)) continue;

                ServiceProvider = worldObject;
			    _providerBid = bid + pathScore;
			    return true;
			}

			return false;
		}

		public override bool CanMeetRequest (Unit unit)
		{
			float bid;
			return CanProvideService(unit, out bid);
		}

        private bool CanProvideService(Unit unit, out float bid)
        {
            bid = 0;
            foreach (string service in unit.ServicesProvided())
            {
                if (service == ServiceNeeded.Name &&
                    ServiceProvider.MyInventory.ContainsTheseItems(ServiceNeeded.Cost))
                {
                    return true;
                }
            }

            return false;
        }

		private bool CanProvideService(WorldObject.WorldObject worldObject, out float bid)
		{
		    if (!(worldObject is ITransaction))
		    {
		        bid = 0;
		        return false;
		    }

			bid = 0;
            foreach (string service in ((ITransaction) worldObject).ServicesProvided())
            {
                if (service == ServiceNeeded.Name &&
                    worldObject.MyInventory.ContainsTheseItems(ServiceNeeded.Cost))
                {
                    return true;
                }
            }

            return false;
		}

		public override void Initiate ()
		{
			ServiceProvider.Requests.AddRequest(this);
		}

        public override string Info()
        {
            string line = string.Empty;
            if (ServiceProvider != null)
            {
                line += " ";
                line += ServiceProvider.Name;
            }

            line += String.Format(" {0}", ServiceNeeded.Name);

            return line;
        }

    }
}

