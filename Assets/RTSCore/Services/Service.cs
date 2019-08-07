using System;
using System.Collections.Generic;
using Assets.RTSCore.Inventory;

namespace Assets.RTSCore.Services
{
	[Serializable]
	public class Service
	{
		public string Name;
        public float MaxTime;
        public List<Item> Cost;
		public float TimeToComplete;
        public string ConditionRelieved;
    }
}

