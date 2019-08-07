using Assets.RTSCore.Map;
using Assets.RTSCore.Requests;
using Assets.RTSCore.StateMachineComponents;
using UnityEngine;

namespace Assets.RTSCore.WorldObject
{
	[System.Serializable]
    public abstract class WorldObject : StateMachine
    {
        public string Name;

        public int MaxHealth;
        public int CurrentHealth;

        public Inventory.Inventory MyInventory;

        public string DeadState = "Dead";

		public Request CurrentRequest { get; set; }
		private RequestContainer _requests;
		public RequestContainer Requests {
			get {
				if (_requests == null) _requests = new RequestContainer();
				return _requests;
			}
		}

		protected bool CurrentlySelected;

		public TerrainType TerrainTypeToChangeTile = TerrainType.None;

		public virtual bool WantsToKeepItem(string itemName) 
		{
			return false;
		}

        // Use this for initialization
        protected virtual void Start()
        {
			AddState(new StateOverridable() {Name = DeadState} );
        }

        protected virtual void OnGUI()
        {

        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

			if (MaxHealth != 0 && CurrentHealth <= 0) 
			{
                MoveToState(DeadState);
			}
        }

        // called when the unit is selected, and the click is made.
        public virtual void LeftMouseClick(GameObject hitObject, Vector3 hitPoint)
        {

        }

        // called when the unit is selected, and the click is made.
        public virtual void RightMouseClick(GameObject hitObject, Vector3 hitPoint)
        {

        }

        public void SetSelected(bool selected)
        {
            CurrentlySelected = selected;
        }

        public virtual string GetDisplayText()
        {
            string textToDisplay = "Hello World!";

            return textToDisplay;
        }
    }
}
