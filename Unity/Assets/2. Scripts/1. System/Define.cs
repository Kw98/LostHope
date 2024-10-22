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
        public int CurHP;
        public int MaxHP;
        public int Power;
        public int Level;
        public float Speed;
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
    public static GameState state = GameState.Stop;
}