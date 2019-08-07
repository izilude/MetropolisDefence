using System;
using System.Collections.Generic;
using System.Linq;
using Assets.RTSCore.AI;
using Assets.RTSCore.Game;
using Assets.RTSCore.Misc;
using Assets.RTSCore.WorldObject.Creep;
using UnityEngine;

namespace Assets.RTSCore.Player
{
    public class CreepWaveManager : MonoBehaviour
    {
        public CreepColony CreepColonyPrefab;
        public List<Creep> CreepPrefabs;

        public float NumberOfSpawnPoints = 1;

        public List<Vector3> SpawnPoints { get; set; }
        public List<CreepColony> CreepColonies { get; set; }

        public float TimeOfFirstWave;
        public float TimeBetweenWaves;

        public int FirstWaveDifficulty;
        public int MaxDifficulty;
        public int WavesTillMaxDifficulty;

        private int _currentDifficulty;
        private CreepColony _activeColony;
        private List<Creep> _nextWave;
        private List<Creep> _onDeckWave;

        private PeriodicEvent _spawnCreepWavePeriodicEvent;

        protected void Start()
        {
            _spawnCreepWavePeriodicEvent = new PeriodicEvent(TimeBetweenWaves, SpawnCreepWave);
            _currentDifficulty = FirstWaveDifficulty;
        }

        public void Initialize()
        {
            SpawnCreepColonies();
        }

        protected void Update()
        {
            if (_activeColony != null && _activeColony.WaveFinished)
            {
                _activeColony = null;
            }

            if (_onDeckWave == null)
            {
                GenerateOnDeckWave();
                IncreaseCurrentDifficulty();
            }
            
            if (_nextWave == null)
            {
                _nextWave = _onDeckWave;
                _onDeckWave = null;
            }

            if (GameTimeManager.TotalGameTime > TimeOfFirstWave && _activeColony == null)
            {
                if (_firstWave)
                {
                    SpawnCreepWave(this, null);
                    _firstWave = false;
                }

                _spawnCreepWavePeriodicEvent.Update(GameTimeManager.DeltaTime);
            }
        }

        bool _firstWave = true;

        private int MinLength = 20;

        private void SpawnCreepColonies()
        {
            int maxTries = 100;
            int cnt = 0;

            SpawnPoints = new List<Vector3>();
            CreepColonies = new List<CreepColony>();

            for (int i = 0; i < NumberOfSpawnPoints; i++)
            {
                CreepColony newColony = (CreepColony)Instantiate(CreepColonyPrefab);

                var location = new Vector3();
                while (cnt < maxTries)
                {
                    location = Game.Game.Instance.ActiveLevel.Map.GetRandomVector3();
                    newColony.transform.position = location;
                    var tiles = Game.Game.Instance.ActiveLevel.Map.FindOccupyingTiles(newColony);
                    if (tiles.FirstOrDefault(x => x.Buildable == false) != null)
                    {
                        cnt++;
                        continue;
                    }

                    int length;
                    float score;
                    bool pathExists = PathFinding.DoesPathExistBetweenWorldObjects(
                        newColony, 
                        Game.Game.Instance.ActiveLevel.Headquarters,
                        out length, 
                        out score);

                    if (pathExists && length > MinLength)
                    {
                        foreach (var tile in tiles)
                        {
                            tile.Buildable = false;
                            tile.Accessible = false;
                        }

                        break;
                    }
                    else cnt++;
                }

                newColony.SpawnPoint = location;
                Game.Game.Instance.ActiveLevel.AddWorldObject(newColony, true);
                CreepColonies.Add(newColony);
            }
        }

        private int maxNumberOfAttempts = 10;
        private void GenerateOnDeckWave()
        {
            int bestAttempt = 0;
            for(int i=0;i< maxNumberOfAttempts; i++)
            {
                int currentAttempt;
                List<Creep> wave = BuildCreepWave(out currentAttempt);

                if ( Math.Abs(currentAttempt - _currentDifficulty) < Math.Abs(bestAttempt - _currentDifficulty))
                {
                    _onDeckWave = wave;
                    bestAttempt = currentAttempt;
                    if (bestAttempt == _currentDifficulty) { return; }
                }
            }
        }

        private List<Creep> BuildCreepWave(out int difficulty)
        {
            List<Creep> wave = new List<Creep>();

            List<Creep> availablePrefabs = new List<Creep>();
            availablePrefabs = GetAvailablePrefabs(_currentDifficulty);
            difficulty = 0;
            while(availablePrefabs.Count != 0)
            {
                int index = (int)Math.Floor(UnityEngine.Random.value*availablePrefabs.Count);
                if (index >= availablePrefabs.Count) { index = availablePrefabs.Count - 1; }

                difficulty += availablePrefabs[index].ChallengeRating;
                wave.Add(availablePrefabs[index]);
                availablePrefabs = GetAvailablePrefabs(_currentDifficulty - difficulty);
            }

            return wave;
        }

        private List<Creep> GetAvailablePrefabs(int remainingDifficulty)
        {
            List<Creep> creeps = new List<Creep>();

            foreach (Creep creep in CreepPrefabs)
            {
                if (creep.ChallengeRating < remainingDifficulty)
                {
                    creeps.Add(creep);
                }
            }

            return creeps;
        }

        private void IncreaseCurrentDifficulty()
        {
            _currentDifficulty += (MaxDifficulty - FirstWaveDifficulty) / WavesTillMaxDifficulty;
            _currentDifficulty = _currentDifficulty > MaxDifficulty ? MaxDifficulty : _currentDifficulty;
        }

        private void SpawnCreepWave(object sender, EventArgs e)
        {
            if (CreepColonies == null || CreepColonies.Count == 0) { return; }

            int index = UnityEngine.Random.Range(0, CreepColonies.Count);
            CreepColonies[index].StartWave(_nextWave);
            _activeColony = CreepColonies[index];

        }
    }
}
