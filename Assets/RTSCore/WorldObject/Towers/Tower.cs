using System.Collections.Generic;
using Assets.RTSCore.Game;
using Assets.RTSCore.WorldObject.Buildings;
using UnityEngine;

namespace Assets.RTSCore.WorldObject.Towers
{
	[System.Serializable]
	public class Tower : Building
	{
		public List<Ammunition.Ammunition> Ammunitions;
		Ammunition.Ammunition _currentAmmo;
		float _currentReloadTime;

		// Use this for initialization
		protected override void Start()
		{
            base.Start();
		}
		
		// Update is called once per frame
		protected override void Update()
		{
            _currentReloadTime += GameTimeManager.DeltaTime;

			bool readyToFile = ReadyToFire();
			if (readyToFile) 
			{
				WorldObject target;
				bool inRange = EnemyIsInRange(out target);
				if (inRange) 
				{
					Fire(target);
				}
			}

			base.Update ();
		}

		protected bool ReadyToFire()
		{
			if (!_currentAmmo || !MyInventory.ContainsTheseItems(_currentAmmo.ItemsNeeded)) 
			{ 
				_currentAmmo = SelectAmmunition();
				if (_currentAmmo) { _currentReloadTime = 0; }
			}

			if (_currentAmmo) 
			{
				 return (_currentReloadTime > _currentAmmo.ReloadTime) ;
			}
			return false;
		}

		protected void Fire(WorldObject target) 
		{
			_currentReloadTime = 0;
			TrySpawnAmmo(target);
			target.GetInformation().CurrentHealth -= _currentAmmo.Damage;
		}

		protected Ammunition.Ammunition SelectAmmunition() 
		{
			if (Ammunitions == null) { return null; }

			foreach (Ammunition.Ammunition ammo in Ammunitions) 
			{
				if (MyInventory.ContainsTheseItems(ammo.ItemsNeeded)) 
				{
					return ammo;
				}
			}

			return null;
		}

		protected bool EnemyIsInRange(out WorldObject target) 
		{
			target = null;
			//if (CurrentLevel == null) { return false; }

			//foreach (Player player in CurrentLevel.CreepWaves) 
			//{
   //             if (player == null) { continue; }

			//	/List<WorldObject> enemies = player.GetAttackableEnemies();
			//	foreach(WorldObject enemy in enemies) 
			//	{
			//		if (enemy.IsDead) {continue;}
			//		Vector3 diff = enemy.transform.position - this.transform.position;
			//		double mag = diff.sqrMagnitude;
			//		if (mag < _currentAmmo.AttackRadius) 
			//		{
			//			target = enemy;
			//			return true;
			//		}
			//	}
			//}

			return false;
		}

		protected bool TrySpawnAmmo(WorldObject target)
		{
			if (Game.Game.Instance.ActiveLevel.Map == null || _currentAmmo == null) {return false;}

			Vector3 spawnPoint = new Vector3();
			spawnPoint.x = this.transform.position.x;
			spawnPoint.y = this.transform.position.y + this.transform.localScale.y;
			spawnPoint.z = this.transform.position.z;
			
			Ammunition.Ammunition spawnedAmmo = (Ammunition.Ammunition)Instantiate (_currentAmmo, spawnPoint, new Quaternion ());
			spawnedAmmo.transform.parent = Game.Game.Instance.ActiveLevel.transform;
			spawnedAmmo.Target = target.transform.position;
			
			return true;
		}
	}
}

