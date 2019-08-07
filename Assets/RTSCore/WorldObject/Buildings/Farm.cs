using Assets.RTSCore.Game;
using Assets.RTSCore.Misc;
using UnityEngine;

namespace Assets.RTSCore.WorldObject.Buildings 
{
	public class Farm : ProductionBuilding 
	{
        public float RotationRadius;
        public float RotationSpeed;
        public float XOffSet;
        public float ZOffSet;
        public float BallHeight;
        public Light EnergyBall;

		// Use this for initialization
		protected override void Start () 
		{
			base.Start ();
		}
	
		// Update is called once per frame
		protected override void Update () 
		{
			base.Update();

            if (State == BuildingState.Working)
            {
                if (EnergyBall)
                {
                    EnergyBall.enabled = true;
                    MoveEnergyBall();
                }
            }
            else if (State == BuildingState.NotWorking)
            {
                if (EnergyBall)
                {
                    EnergyBall.enabled = false;
                }
            }
		}

        private float _currentAngle;
        private void MoveEnergyBall()
        {
            _currentAngle += GameTimeManager.DeltaTime * RotationSpeed;
            if (_currentAngle > 2*3.14) { _currentAngle = 2 * 3.14f - _currentAngle; }

            float x = XOffSet + RotationRadius * Mathf.Sin(_currentAngle);
            float z = ZOffSet + RotationRadius * Mathf.Cos(_currentAngle);

            EnergyBall.transform.localPosition = new Vector3(x, BallHeight, z);
        }
	}
}
