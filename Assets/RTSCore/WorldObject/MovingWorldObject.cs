using System;
using System.Collections.Generic;
using Assets.RTSCore.Game;
using Assets.RTSCore.Map;
using Assets.RTSCore.Misc;
using UnityEngine;

namespace Assets.RTSCore.WorldObject
{
	public abstract class MovingWorldObject : WorldObject
	{
	    public float WalkSpeed;
	    public float RunSpeed;

        public List<Tile> WayPoints;
		
		private List<Tile> _movementQueue;
		public List<Tile> MovementQueue 
		{
			get 
			{
				if (_movementQueue == null) {_movementQueue = new List<Tile>();}
				
				return _movementQueue;
			}
		}
		
		private UnitState _state;
		public UnitState State
        {
			get
            {
				return _state;
			}
            set
            {
				_state = value;
			}
		}
		
		// Update is called once per frame
		protected bool ArrivedAtWaypoint;
		protected override void Update()
		{
			base.Update();

			if (State == UnitState.Walking) 
			{
				float speed = WalkSpeed;
				ArrivedAtWaypoint = Move(speed);
			}
		}

		public void AddTileToMoveQueue(Tile tile) 
		{
			MovementQueue.Add (tile);
		}
		
		protected virtual bool Move(float speed)
		{
			UpdateCurrentTileIfNeeded();
			
			if (CurrentTile == null) { return false; }
			
			return MoveTowardCurrentTile();
		}
		
		private Tile CurrentTile { get; set; }
		private void UpdateCurrentTileIfNeeded() 
		{
			if (CurrentTile == null &&  
			    MovementQueue.Count > 0) 
			{
				CurrentTile = MovementQueue[0];
				UpdateRotationAngle();
				MovementQueue.RemoveAt(0);
			} 
			
			if (CurrentTile == null ||
			    !CurrentTile.Accessible) 
			{
				TerminateMovement();
			}
		}

		private void UpdateRotationAngle() 
		{
			float dx = transform.position.x - CurrentTile.transform.position.x;
			float dz = transform.position.z - CurrentTile.transform.position.z;

			float angle = 0f;
			if (dx > 0.5f) 
			{
				angle = 270;
			} 
			else if (dz > 0.5) 
			{
				angle = 180;
			} 
			else if (dx < -0.5f) 
			{
				angle = 90;
			} 
			else 
			{
				angle = 0;
			}

			transform.rotation = Quaternion.AngleAxis(angle, new Vector3(0,1,0));
		}
		
		private void TerminateMovement() 
		{
			State = UnitState.Finished;
			CurrentTile = null;
			MovementQueue.Clear();
		}
		
		private bool MoveTowardCurrentTile() 
		{
			float speed = WalkSpeed * GameTimeManager.DeltaTime*CurrentTile.SpeedFactor;
			
			Vector3 tilePosition = CurrentTile.transform.position;
			Vector3 unitPosition = transform.position;
			
			float dx = tilePosition.x - unitPosition.x;
			float dz = tilePosition.z - unitPosition.z;
			
			float dr = (float)Math.Sqrt(dx*dx + dz*dz);
			
			float newx = unitPosition.x + speed*dx/dr;
			float newz = unitPosition.z + speed*dz/dr;

			if (dr <= speed) {
				newx = tilePosition.x;
				newz = tilePosition.z;
				
				return ArrivedAtTile();
			}
			
			transform.position = new Vector3(newx, transform.position.y, newz);
			return false;
		}
		
		private bool ArrivedAtTile() 
		{
			bool isWaypoint = CurrentTile.Equals(WayPoints[0]);
			if (isWaypoint) 
			{
				WayPoints.RemoveAt(0);
			}
			
			CurrentTile = null;
			return isWaypoint;
		}
	}
}
