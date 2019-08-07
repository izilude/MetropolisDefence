namespace Assets.RTSCore.Misc
{
    public class CountdownEvent
    {
        public string Name;
        public float MaxTime;
        public float TimeRemaining { get; set; }

        public CountdownEvent(float maxTime)
        {
            MaxTime = maxTime;
            TimeRemaining = 0;
        }

        public void AddTime(float deltaTime)
        {
            TimeRemaining += deltaTime;
            if (TimeRemaining > MaxTime) { TimeRemaining = MaxTime; }
            if (TimeRemaining < 0) { TimeRemaining = 0; }
        }

        public void Update(float deltaTime)
        {
            TimeRemaining -= deltaTime;
            if (TimeRemaining < 0)
            {
                TimeRemaining = 0;
            }
        }

        public bool IsExpired
        {
            get
            {
                return TimeRemaining == 0;
            }
        }

        public bool IsActive
        {
            get
            {
                return !IsExpired;
            }
        }
    }
}
