using System;
using System.Collections.Generic;
using Assets.RTSCore.AI;
using Assets.RTSCore.Game;
using Assets.RTSCore.Map;
using Assets.RTSCore.Misc;
using UnityEngine;

namespace Assets.RTSCore.WorldObject.Creep
{
    public class CreepColony : WorldObject
    {
        bool pathCalculated = false;

        public List<Tile> PathToHeadquarters = new List<Tile>();
        public Vector3 SpawnPoint { get; set; }
        public float TimeBetweenSpawns { get; set; }
        public Queue<Creep> CreepPrefabsToSpawn = new Queue<Creep>();
        public List<Creep> SpawnedCreeps;

        PeriodicEvent spawnEvent;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            TimeBetweenSpawns = 1;
            spawnEvent = new PeriodicEvent(TimeBetweenSpawns, SpawnCreep);
        }

        protected override void Update()
        {
            base.Update();

            if (!pathCalculated)
            {
                CalculatePathToHeadquarters();
            }

            if (!WaveFinished)
            {
                spawnEvent.Update(GameTimeManager.DeltaTime);
            }
        }

        public void StartWave(List<Creep> creeps)
        {
            foreach (Creep creep in creeps)
            {
                CreepPrefabsToSpawn.Enqueue(creep);
            }
        }

        private void SpawnCreep(object sender, EventArgs e)
        {
            Creep creepPrefab = CreepPrefabsToSpawn.Dequeue();
            Creep clone = (Creep)AddCreep(creepPrefab);
            clone.transform.position = SpawnPoint;

            foreach (Tile tile in PathToHeadquarters)
            {
                clone.AddTileToMoveQueue(tile);
            }

            clone.WayPoints.Add(PathToHeadquarters[PathToHeadquarters.Count-1]);
            clone.State = UnitState.Walking;
        }

        public Creep AddCreep(Creep worldObject)
        {
            Creep newCreep = (Creep)Instantiate(worldObject, worldObject.transform.position, worldObject.transform.rotation);

            newCreep.transform.parent = Game.Game.Instance.ActiveLevel.transform;
            newCreep.enabled = true;
            SpawnedCreeps.Add(newCreep);

            return newCreep;
        }

        public List<Creep> GetAttackableEnemies()
        {
            return SpawnedCreeps;
        }

        public bool WaveFinished
        {
            get
            {
                return CreepPrefabsToSpawn == null || CreepPrefabsToSpawn.Count == 0;
            }
        }

        private void CalculatePathToHeadquarters()
        {
            if (Game.Game.Instance.ActivePlayer == null
                || Game.Game.Instance.ActiveLevel.Headquarters == null
                || pathCalculated) { return; }

            if (PathFinding.TryGetPathToWorldObject(SpawnPoint, Game.Game.Instance.ActiveLevel.Headquarters, out PathToHeadquarters))
            {
                pathCalculated = true;
            }
            else
            {
                pathCalculated = true;
                Debug.Log("Path To Headquarters FAILED!");
            }
        }
    }
}
