using System;
using System.Collections.Generic;
using Assets.RTSCore.Game;
using Assets.RTSCore.Inventory;
using Assets.RTSCore.Misc;
using Assets.RTSCore.Requests;
using Assets.RTSCore.Services;

namespace Assets.RTSCore.WorldObject.Buildings
{
	public class House : Building
	{
		public House NextLevelHouse;
		public House PreviousLevelHouse;
		public List<Item> ItemsNeededToUpgrade;
        public List<string> ServicesNeededForUpgrade;

        private float _tryUpgradeDownGradePeriod = 1;
        private PeriodicEvent _tryUpgradeDownGradePeriodicEvent;

		// Use this for initialization
		protected override void Start()
		{
			base.Start();
			CurrentPopulation = MaxPopulation;
            _tryUpgradeDownGradePeriodicEvent = new PeriodicEvent(_tryUpgradeDownGradePeriod, TryUpgradeDownGrade);
        }
		
		// Update is called once per frame
		protected override void Update()
		{
            base.Update();

            UpdateActiveServices();
            _tryUpgradeDownGradePeriodicEvent.Update(GameTimeManager.DeltaTime);
		}

        protected override void RefreshRequests(object sender, EventArgs e)
        {
            base.RefreshRequests(sender, e);

            int cnt = 0;
            foreach (string serviceName in ServicesNeededForUpgrade)
            {
                cnt++;
                Service service = Game.Game.Instance.ActiveLevel.ServiceChart.GetService(serviceName);

                float remainingTime = 0;
                foreach (CountdownEvent activeService in ActiveServices)
                {
                    if (activeService.Name == serviceName)
                    {
                        remainingTime = activeService.TimeRemaining;
                    }
                }

                PriorityLevel priority = GetPriorityLevel(remainingTime, service.MaxTime);
                Requests.PostRequest(new ServiceRequest(service, this, priority, true, 700 + cnt));
            }

            foreach (Item item in ItemsNeededToUpgrade)
            {
                cnt++;
                PriorityLevel priority = GetPriorityLevel(MyInventory.GetItemAmount(item.Name), item.Amount);
                Requests.PostRequest(new ItemRequest(item.Name, item.Amount * 100, this, PriorityLevel.High, true, 700 + cnt));
            }
        }

        private void UpdateActiveServices()
        {
            foreach (CountdownEvent timer in ActiveServices)
            {
                timer.Update(GameTimeManager.DeltaTime);
            }
        }

        private void TryUpgradeDownGrade(object sender, EventArgs e)
        {
			TryUpgradeToNextLevel();
			TryDowngradeToPreviousLevel();
		}

		protected virtual void TryUpgradeToNextLevel() 
		{
            if (NextLevelHouse == null) { return; }

            if (ItemsNeededToUpgrade != null)
            {
                foreach (Item item in ItemsNeededToUpgrade)
                {
                    int currentAmount = MyInventory.GetItemAmount(item.Name);
                    if (currentAmount < item.Amount)
                    {
                        return;
                    }
                }
            }

            if (ServicesNeededForUpgrade != null)
            {
                foreach (string serviceName in ServicesNeededForUpgrade)
                {
                    bool notFound = true;
                    foreach (CountdownEvent activeService in ActiveServices)
                    {
                        if (activeService.Name == serviceName)
                        {
                            notFound = false;
                            if (activeService.IsExpired) { return; }
                            break;
                        }
                    }

                    if (notFound) { return; }
                }
            }

            ReplaceThisObject(NextLevelHouse);
		}
		
		protected virtual void TryDowngradeToPreviousLevel()
		{
			if (PreviousLevelHouse == null) { return; }

            if (PreviousLevelHouse.ItemsNeededToUpgrade != null)
            {
                foreach (Item item in PreviousLevelHouse.ItemsNeededToUpgrade)
                {
                    int currentAmount = MyInventory.GetItemAmount(item.Name);
                    if (currentAmount < item.Amount)
                    {
                        ReplaceThisObject(PreviousLevelHouse);
                        return;
                    }
                }
            }

            if (PreviousLevelHouse.ServicesNeededForUpgrade != null)
            {
                foreach (string serviceName in PreviousLevelHouse.ServicesNeededForUpgrade)
                {
                    foreach (CountdownEvent activeService in ActiveServices)
                    {
                        if (activeService.Name == serviceName && activeService.IsExpired)
                        {
                            ReplaceThisObject(PreviousLevelHouse);
                            return;
                        }
                    }
                }
            }
		}

		protected virtual void ReplaceThisObject(House house) 
		{
			House newHouse = (House)Game.Game.Instance.ActiveLevel.AddWorldObject(house, true);

			newHouse.transform.position = this.transform.position;
			newHouse.transform.rotation = this.transform.rotation;
			newHouse.MyInventory = new Inventory.Inventory(this.MyInventory);

		    Game.Game.Instance.ActiveLevel.RemoveWorldObject(this);
			Destroy(gameObject);
		}
	}
}
