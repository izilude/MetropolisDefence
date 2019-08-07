using System.Collections.Generic;
using Assets.RTSCore.AI;
using Assets.RTSCore.Game;
using Assets.RTSCore.Inventory;
using UnityEngine;

namespace Assets.RTSCore.Ammunition
{
	[System.Serializable]
	public class Ammunition : MonoBehaviour
	{
		public float AttackRadius;
		public float ReloadTime;
		public int Damage;
		public List<Item> ItemsNeeded;
		public float TimeToTarget;
		public float Gravity;

		public Vector3 Target {get; set;}
		private Trajectory TrajectoryToTarget;

		// Use this for initialization
		protected virtual void Start()
		{
			
		}
		
		// Update is called once per frame
		protected virtual void Update()
		{
			if (Target == null) { return; }

			if (TrajectoryToTarget == null) 
			{ 
				TrajectoryToTarget = new Trajectory(this.transform.position, Target, TimeToTarget, Gravity); 
			}

			this.transform.position = TrajectoryToTarget.TakeStep(GameTimeManager.DeltaTime);
			if (TrajectoryToTarget.AtTarget()) 
			{
				Destroy(this.gameObject);
			}
		}
	}
}

