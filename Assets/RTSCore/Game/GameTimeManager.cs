namespace Assets.RTSCore.Game
{
    public static class GameTimeManager
    {
        public static bool Paused = false;
        public static float GameSpeed = 1;
        public static GameDateTime CurrentGameTime = new GameDateTime();
        public static float DeltaTime = 0;
        public static float TotalGameTime = 0;

        public static void Update(float deltaTime)
        {
            if (!Paused)
            {
                TotalGameTime += deltaTime;
                DeltaTime = deltaTime*GameSpeed;
                CurrentGameTime.RealTimeElapsed(deltaTime*GameSpeed);
            }
            else
            {
                DeltaTime = 0f;
            }
        }
    }
}
