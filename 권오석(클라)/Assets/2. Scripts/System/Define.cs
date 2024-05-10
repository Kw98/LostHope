public enum GameState
{
    Play,
    Pause,
    Stop
}
public static class Define
{
    public class Data
    {
        public float CurHP;
        public float MaxHP;
        public float Power;
    }

    public class PlayerData : Data
    {
        
    }

    public class MonsterData : Data
    {
        public float HitDelayTime;
        public float AtkDelay;
    }

    // Other
    public static int stage = 1;
    public static GameState state = GameState.Stop;
}