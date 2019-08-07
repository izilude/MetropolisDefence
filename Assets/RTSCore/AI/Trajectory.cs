using UnityEngine;

namespace Assets.RTSCore.AI
{
	public class Trajectory
	{
		private Vector3 _velocityVector;
		private Vector3 _initialPosition;
		private float _gravity;
		private float _totalTime;
		private float _timeToTarget;

		public Trajectory(Vector3 initialPosition, Vector3 finalPosition, float timeToTarget, float gravity) 
		{
			InitializeTrajectory(initialPosition, finalPosition, timeToTarget, gravity);
		}

		private void InitializeTrajectory(Vector3 initialPosition, Vector3 finalPosition, float timeToTarget, float gravity) 
		{
			_totalTime = 0;
			_gravity = gravity;
			_timeToTarget = timeToTarget;
			float x = (finalPosition.x - initialPosition.x)/timeToTarget;
			float z = (finalPosition.z - initialPosition.z)/timeToTarget;
			float y = (finalPosition.y - initialPosition.y)/timeToTarget + 0.5f*gravity*timeToTarget;
			_initialPosition = new Vector3(initialPosition.x, initialPosition.y, initialPosition.z);
			_velocityVector = new Vector3(x,y,z);
		}

		public Vector3 TakeStep(float deltaTime) 
		{
			_totalTime += deltaTime;
			Vector3 position = _initialPosition + _totalTime*_velocityVector;
			position.y = position.y - 0.5f*_gravity*_totalTime*_totalTime;
			return position;
		}

		public bool AtTarget() 
		{
			return _totalTime > _timeToTarget;
		}
	}
}

