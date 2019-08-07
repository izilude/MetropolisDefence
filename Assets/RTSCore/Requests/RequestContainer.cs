using System.Collections.Generic;
using Assets.RTSCore.Misc;

namespace Assets.RTSCore.Requests
{
	public class RequestContainer
	{
		public Level.Level CurrentLevel;

		private List<Request> _lowPriorityRequests = new List<Request>();
		private List<Request> _mediumPriorityRequests = new List<Request>();
		private List<Request> _highPriorityRequests = new List<Request>();

		private List<Request> _postedRequests = new List<Request>();
		public List<Request> PostedRequests { get {return _postedRequests; } }

		public void RemoveCompletedRequests() 
		{
			RemoveCompletedRequestsFromList(_lowPriorityRequests);
			RemoveCompletedRequestsFromList(_mediumPriorityRequests);
			RemoveCompletedRequestsFromList(_highPriorityRequests);
			RemoveCompletedRequestsFromList(_postedRequests);
		}

		private void RemoveCompletedRequestsFromList(List<Request> array) 
		{
			for(int i=array.Count-1;i>=0;i--) 
			{
				if (array[i].State == RequestState.Complete) 
				{
					array.RemoveAt(i);
				}
			}
		}

		public void AddRequest(Request request) 
		{
			switch(request.Priority) 
			{
			case PriorityLevel.Low:
				_lowPriorityRequests.Add (request);
				break;
			case PriorityLevel.Medium:
				_mediumPriorityRequests.Add(request);
				break;
			default:
				_highPriorityRequests.Add (request);
				break;
			}
		}
	
		public List<Request> AllRequests()
		{
			List<Request> allRequests = new List<Request>();
			allRequests.AddRange(_highPriorityRequests);
			allRequests.AddRange(_mediumPriorityRequests);
			allRequests.AddRange(_lowPriorityRequests);
			return allRequests;
		}

		public Request NextRequest(bool working)
		{
			Request request = null;

			for(int i=0;i<_highPriorityRequests.Count;i++) 
			{
				request = _highPriorityRequests[i];
				if (request.InitiateEvenIfNotWorking || working) 
				{
					_highPriorityRequests.RemoveAt(i);
					return request;
				}
			}

			for(int i=0;i<_mediumPriorityRequests.Count;i++) 
			{
				request = _mediumPriorityRequests[i];
				if (request.InitiateEvenIfNotWorking || working) 
				{
					_mediumPriorityRequests.RemoveAt(i);
					return request;
				}
			}

			for(int i=0;i<_lowPriorityRequests.Count;i++) 
			{
				request = _lowPriorityRequests[i];
				if (request.InitiateEvenIfNotWorking || working) 
				{
					_lowPriorityRequests.RemoveAt(i);
					return request;
				}
			}

			return null;
		}

		public void PostRequest(Request request) 
		{
            if (request.Priority == PriorityLevel.None) { return; }
            
			foreach (Request postedRequest in _postedRequests) 
			{
				if (request.ID == postedRequest.ID) 
				{
					return;
				}
			}

			_postedRequests.Add (request);
		}
	}
}

