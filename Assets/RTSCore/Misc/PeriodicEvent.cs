using System;

namespace Assets.RTSCore.Misc
{
    public class PeriodicEvent
    {
        public event EventHandler TimeElapsedEvent;

        public float Period;
        private float _timeElapsed;

        public PeriodicEvent(float period, EventHandler timeElapsedEvent)
        {
            Period = period;
            _timeElapsed = 0;
            TimeElapsedEvent += timeElapsedEvent;
        }

        public void Update(float deltaTime)
        {
            _timeElapsed += deltaTime;
            if (_timeElapsed > Period)
            {
                OnTimeElapsedEvent();
                _timeElapsed = 0;
            }
        }
 
        public void OnTimeElapsedEvent()
        {
            if (TimeElapsedEvent != null)
            {
                TimeElapsedEvent(this, new EventArgs());
            }
        }
    }
}
