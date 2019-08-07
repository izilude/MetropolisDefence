using System.Collections.Generic;
using Assets.RTSCore.Misc;
using Assets.RTSCore.WorldObject.Buildings;
using Assets.RTSCore.WorldObject.Units;

namespace Assets.RTSCore.Requests
{
	public abstract class Request
	{
		public Request() {}
		public Request(Building initiator, PriorityLevel priority, bool initiateEvenIfNotWorking, int id) 
		{
			Priority = priority;
			InitiatorOfRequest = initiator;
			State = RequestState.Posted;
			InitiateEvenIfNotWorking = initiateEvenIfNotWorking;
			ID = id;
		}

		public int ID;
		public RequestState State;
		public PriorityLevel Priority;
		public Building InitiatorOfRequest;
		public bool InitiateEvenIfNotWorking;

		public override bool Equals (object obj)
		{
			if (obj is Request) 
			{
				Request request = obj as Request;
				return request.Priority == this.Priority;
			}

			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public virtual string Info() { return string.Empty; }

		public abstract bool CanMeetRequest(List<WorldObject.WorldObject> worldObjects);
		public abstract bool CanMeetRequest(Unit unit);
		public abstract void Initiate();
	}
}

