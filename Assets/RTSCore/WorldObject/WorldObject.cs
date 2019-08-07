using Assets.RTSCore.Level;
using Assets.RTSCore.Map;
using Assets.RTSCore.Requests;
using UnityEngine;

namespace Assets.RTSCore.WorldObject
{
	[System.Serializable]
    public abstract class WorldObject : MonoBehaviour
    {
		public Inventory.Inventory MyInventory;

        public bool IsDead { get; set; }
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

		public abstract Information.Information GetInformation();

		public virtual bool WantsToKeepItem(string itemName) 
		{
			return false;
		}

        // Use this for initialization
        protected virtual void Start()
        {
			IsDead = false;
        }

        protected virtual void OnGUI()
        {

        }

        // Update is called once per frame
        protected virtual void Update()
        {
			var info = GetInformation ();

			if (info != null && GetInformation().MaxHealth != 0 && GetInformation().CurrentHealth <= 0) 
			{
				IsDead = true;
				GameObject.Destroy(this.gameObject);
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
			if (selected) 
			{

			} 
			else 
			{

			}

            CurrentlySelected = selected;
        }

        public virtual string GetDisplayText()
        {
            string textToDisplay = "Hello World!";

            return textToDisplay;
        }
    }
}
