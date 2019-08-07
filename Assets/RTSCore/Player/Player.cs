using UnityEngine;

namespace Assets.RTSCore.Player
{
    public class Player : MonoBehaviour
    {
		public int Money;
        public int MaxHealth;
        public int CurrentHealth;

        // Use this for initialization
		protected virtual void Start()
        {

        }

		protected virtual void OnGui()
        {
        }

        // Update is called once per frame
		float deltaTime = 0;

        protected virtual void Update()
        {
		}

        public void ReducePlayerHealth(int amountToReduce)
        {
            CurrentHealth -= amountToReduce;
            if (CurrentHealth <= 0)
            {
                Game.Game.Instance.PlayerDefeated();
            }
        }
    }
}
