using System;

namespace Assets.RTSCore.Information
{
	[Serializable]
    public class Information
    {
        public String Name;

        public int MaxHealth;
        public int CurrentHealth;

        public void SetHealth(string name, int maxHealth)
        {
            Name = name;
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }
    }
}
