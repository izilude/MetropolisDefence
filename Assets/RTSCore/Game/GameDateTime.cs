namespace Assets.RTSCore.Game
{
    public class GameDateTime
    {
        public int MonthIndex;
        public int Day;
        public int Year;

        private float _currentDayProgress = 0;

        public GameDateTime()
        {
            Year = GameDateTimeSettings.StartYear;
            Day = 0;
            MonthIndex = 0;
        }

        public Month Month
        {
            get
            {
                return (Month)MonthIndex;
            }
        }

        public void RealTimeElapsed(float realTimeElapsedInSeconds)
        {
            _currentDayProgress += realTimeElapsedInSeconds;
            if (_currentDayProgress > GameDateTimeSettings.SecondsPerDay)
            {
                _currentDayProgress -= GameDateTimeSettings.SecondsPerDay;
                Day++;
                if (Day >= GameDateTimeSettings.DaysPerMonth)
                {
                    Day = 0;
                    MonthIndex++;
                    if (MonthIndex >= GameDateTimeSettings.NumberOfMonths)
                    {
                        MonthIndex = 0;
                        Year++;
                    }
                }
            }
        }
    }
}
