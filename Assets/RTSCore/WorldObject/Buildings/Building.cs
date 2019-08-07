using System;
using System.Collections.Generic;
using Assets.RTSCore.Game;
using Assets.RTSCore.Information;
using Assets.RTSCore.Interfaces;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Level;
using Assets.RTSCore.Map;
using Assets.RTSCore.Misc;
using Assets.RTSCore.Requests;
using Assets.RTSCore.Services;
using Assets.RTSCore.WorldObject.Units;
using UnityEngine;

namespace Assets.RTSCore.WorldObject.Buildings
{
    public class Building : BuildableWorldObject, ITransaction
    {
        public List<Unit> SpawnableUnitPrefabs;
        protected List<Unit> _spawnedUnits;

        private float _refreshRequestsPeriod = 1;
        private PeriodicEvent _refreshRequestsPeriodicEvent;

        private float _processRequestsPeriod = 1;
        private PeriodicEvent _processRequestsPeriodicEvent;

        private float _checkIfWorkingPeriod = 1;
        private PeriodicEvent _checkIfWorkingPeriodicEvent;

        public List<Item> ItemsNeededPerTick;
        public BuildingInformation Information;
        public BuildingState State { get; set; }

        public int MaxPopulation;
        public int CurrentPopulation { get; set; }

        protected List<string> _activeConditions = new List<string>();
        public List<CountdownEvent> ActiveServices { get; set; }

        public List<ItemBuildingFlags> ItemFlags;

        // Use this for initialization
        protected override void Start()
        {
            ActiveServices = new List<CountdownEvent>();

            _refreshRequestsPeriodicEvent = new PeriodicEvent(_refreshRequestsPeriod, RefreshRequests);
            _processRequestsPeriodicEvent = new PeriodicEvent(_processRequestsPeriod, ProcessRequest);
            _checkIfWorkingPeriodicEvent = new PeriodicEvent(_checkIfWorkingPeriod, CheckIfWorking);

            _spawnedUnits = new List<Unit>();
            foreach (Unit unit in SpawnableUnitPrefabs)
            {
                _spawnedUnits.Add(null);
            }

            foreach (ItemBuildingFlags name in ItemFlags)
            {
                Item item = MyInventory.GetItem(name.Name);
            }
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            _checkIfWorkingPeriodicEvent.Update(GameTimeManager.DeltaTime);
            _refreshRequestsPeriodicEvent.Update(GameTimeManager.DeltaTime);
            _processRequestsPeriodicEvent.Update(GameTimeManager.DeltaTime);

            foreach (CountdownEvent activeService in ActiveServices)
            {
                activeService.Update(GameTimeManager.DeltaTime);
            }

            DestroyUnitsIfFinished();
        }
        
        private void CheckIfWorking(object sender, EventArgs e)
        {
            List<NotFunctioningReason> reasons;

            bool working = IsWorking(out reasons);

            if (working)
            {
                State = BuildingState.Working;
            }
            else
            {
                State = BuildingState.NotWorking;
            }
        }

        public bool IsUnitSpawned(int index)
        {
            return _spawnedUnits[index];
        }

        public void AddTimeToActiveService(Service service, float timeToAdd)
        {
            foreach (CountdownEvent activeService in ActiveServices)
            {
                if (activeService.Name == service.Name)
                {
                    activeService.AddTime(timeToAdd);
                    return;
                }
            }

            CountdownEvent timer = new CountdownEvent(service.MaxTime);
            timer.Name = service.Name;
            timer.AddTime(timeToAdd);
            ActiveServices.Add(timer);
        }

        public void AddCondition(string condition)
        {
            foreach (string c in _activeConditions)
            {
                if (condition == c)
                {
                    return;
                }
            }

            _activeConditions.Add(condition);
        }

        protected PriorityLevel GetPriorityLevel(float remaining, float max)
        {
            PriorityLevel priority = PriorityLevel.None;
            if (remaining / max < 0.25)
            {
                priority = PriorityLevel.High;
            }
            else if (remaining / max < 0.5)
            {
                priority = PriorityLevel.Medium;
            }
            else if (remaining / max < 0.75)
            {
                priority = PriorityLevel.Low;
            }

            return priority;
        }

        public void RemoveCondition(string condition)
        {
            for(int i=_activeConditions.Count-1;i>=0;i--)
            {
                if (_activeConditions[i] == condition)
                {
                    _activeConditions.RemoveAt(i);
                    return;
                }
            }
        }

        public override Information.Information GetInformation()
        {
            return Information;
        }

        public bool CanRetrieveItem(string itemName)
        {
            if (SpawnableUnitPrefabs == null) { return false; }

            foreach (Unit unit in SpawnableUnitPrefabs)
            {
                if (unit == null) { return false; }
                if (unit.CanRetrieveItem(itemName)) { return true; }
            }

            return false;
        }

        public bool CanDistributeItem(string itemName)
        {
            if (SpawnableUnitPrefabs == null) { return false; }

            foreach (Unit unit in SpawnableUnitPrefabs)
            {
                if (unit == null) { return false; }
                if (unit.CanDistributeItem(itemName)) { return true; }
            }

            return false;
        }

        public override bool WantsToKeepItem(string itemName)
        {
            foreach (ItemBuildingFlags s in ItemFlags)
            {
                if (s.Name.Equals(itemName))
                {
                    return s.Keep;
                }
            }

            return false;
        }

        public bool CanHarvestItem(string itemName)
        {
            if (SpawnableUnitPrefabs == null) { return false; }

            foreach (Unit unit in SpawnableUnitPrefabs)
            {
                if (unit == null) { return false; }
                if (unit.CanHarvestItem(itemName)) { return true; }
            }

            return false;
        }

        public bool RequireHarvestingToTake(string ItemName)
        {
            foreach (ItemBuildingFlags item in ItemFlags)
            {
                if (ItemName == item.Name)
                {
                    return item.RequireHarvestToTake;
                }
            }

            return false;
        }

        public bool IsSelfSufficient(string ItemName)
        {
            foreach (ItemBuildingFlags item in ItemFlags)
            {
                if (ItemName == item.Name)
                {
                    return item.SelfSufficient;
                }
            }

            return false;
        }

        public bool CanDistributeAndRetrieveItem(string itemName)
        {
            if (SpawnableUnitPrefabs == null) { return false; }

            foreach (Unit unit in SpawnableUnitPrefabs)
            {
                if (unit == null) { return false; }

                if (unit.CanDistributeAndRetrieveItem(itemName))
                {
                    return true;
                }
            }

            return false;
        }

        public override void LeftMouseClick(GameObject hitObject, Vector3 hitPoint)
        {
            AddCondition("Fire");
        }

        private void DestroyUnitsIfFinished()
        {
            for (int i = 0; i < SpawnableUnitPrefabs.Count; i++)
            {
                if (!IsUnitSpawned(i)) { continue; }

                if (_spawnedUnits[i].State == UnitState.Finished)
                {
                    GameObject.Destroy(_spawnedUnits[i].gameObject);
                    _spawnedUnits[i] = null;
                }
            }
        }

        protected virtual void ProcessRequest(object sender, EventArgs e)
        {
            float bid;
            for (int i = 0; i < SpawnableUnitPrefabs.Count; i++)
            {
                if (IsUnitSpawned(i)) { continue; }

                List<Request> allRequests = Requests.AllRequests();
                foreach (Request request in allRequests)
                {
                    if (request.State != RequestState.Accepted) { continue; }

                    if (request.CanMeetRequest(SpawnableUnitPrefabs[i]))
                    {
                        bool spawnSucceeded = TrySpawnUnitsToFillRequest(i, request);
                        if (spawnSucceeded) request.State = RequestState.InProcess;
                        return;
                    }
                }
            }
        }

        protected virtual void RefreshRequests(object sender, EventArgs e)
        {
            RefreshItemRequests();
            RefreshServiceRequests();
        }

        private void RefreshServiceRequests()
        {
            int cnt = 0;

            cnt++;
            RefreshSingleServiceRequest("FirePrevention", 800 + cnt);

            foreach (string condition in _activeConditions)
            {
                // TODO: Reinstall this.
                //Requests.PostRequest(new ServiceRequest(condition, this, PriorityLevel.High, true, 700 + cnt));
                cnt++;
            }
        }

        private void RefreshSingleServiceRequest(string serviceName, int id)
        {
            if (Game.Game.Instance.ActiveLevel == null) { return; }

            Service service = Game.Game.Instance.ActiveLevel.ServiceChart.GetService(serviceName);
            float remaining = 0;
            foreach (CountdownEvent activeService in ActiveServices)
            {
                if (activeService.Name == service.Name)
                {
                    remaining = activeService.TimeRemaining;
                }
            }
            PriorityLevel priority = GetPriorityLevel(remaining, service.MaxTime);
            Requests.PostRequest(new ServiceRequest(service, this, priority, true, id));
        }

        private void RefreshItemRequests()
        {
            if (ItemFlags == null) { return; }

            for (int i = 0; i < ItemFlags.Count; i++)
            {
                ItemBuildingFlags itemFlag = ItemFlags[i];
                if (!itemFlag.Request) { continue; }

                Item item;
                item = MyInventory.GetItem(itemFlag.Name);
                if (item == null) continue;
                
                float percent = (float)ItemFlags.Count * item.Amount / (float)MyInventory.Capacity;
                int delta = MyInventory.Capacity - item.Amount;
                if (percent < 0.25)
                {
                    Requests.PostRequest(new ItemRequest(item.Name, delta, this, PriorityLevel.High, true, i));
                }
                else if (percent < 0.5)
                {
                    Requests.PostRequest(new ItemRequest(item.Name, delta, this, PriorityLevel.Medium, true, i));
                }
                else if (percent < 1.0)
                {
                    Requests.PostRequest(new ItemRequest(item.Name, delta, this, PriorityLevel.Low, true, i));
                }
            }
        }

        public List<string> ServicesProvided()
        {
            List<string> services = new List<string>();
            foreach (Unit unit in SpawnableUnitPrefabs)
            {
                services.AddRange(unit.ServicesProvided());
            }
            return services;
        }

        protected virtual bool TrySpawnUnitsToFillRequest(int index, Request request)
        {
            if (!SpawnableUnitPrefabs[index]) { return false; }

            if (!_spawnedUnits[index] && request != null)
            {
                return TrySpawnUnit(index, request);
            }

            return false;
        }

        protected virtual bool IsWorking(out List<NotFunctioningReason> Reasons)
        {
            Reasons = new List<NotFunctioningReason>();

            foreach (Item item in ItemsNeededPerTick)
            {
                int amountInInventory = MyInventory.GetItemAmount(item.Name);
                int amountNeeded = item.Amount;
                if (amountInInventory >= amountNeeded)
                {
                    MyInventory.SubtractItems(item.Name, amountNeeded);
                }
                else
                {
                    Reasons.Add(NotFunctioningReason.NeedMaterials);
                }
            }

            return Reasons.Count == 0;
        }

        protected virtual bool TrySpawnUnit(int index, Request request)
        {
            if (Game.Game.Instance.ActiveLevel == null 
                || Game.Game.Instance.ActiveLevel.Map == null) { return false; }

            Vector3 spawnPoint;
            bool spawnPointFound = TryFindSpawnPoint(out spawnPoint);
            if (!spawnPointFound) { return false; }

            _spawnedUnits[index] = (Unit)Instantiate(SpawnableUnitPrefabs[index], spawnPoint, new Quaternion());
            _spawnedUnits[index].transform.parent = this.transform;
            _spawnedUnits[index].Requests.AddRequest(request);
            _spawnedUnits[index].State = UnitState.Stopped;

            return true;
        }

        protected virtual bool TryFindSpawnPoint(out Vector3 spawnPoint)
        {
            List<Tile> tiles = Game.Game.Instance.ActiveLevel.Map.FindSurroundingTiles(this, true);
            foreach (Tile tile in tiles)
            {
                spawnPoint = new Vector3(tile.transform.position.x,
                                            tile.transform.position.y,
                                            tile.transform.position.z);
                return true;
            }

            spawnPoint = new Vector3();
            return false;
        }
    }
}