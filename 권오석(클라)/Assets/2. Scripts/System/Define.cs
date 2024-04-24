public enum PlayerState
{
    Stand,
    Run,
    Dead
}
public enum MonsterState
{
    Run,
    Hit,
    Dead
}
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
        public float Exp;
        public float MaxExp;
        public float Speed;
        public float Power;
    }

    public class PlayerData : Data
    {
        public int Level;
    }

    public class MonsterData : Data
    {
        public float HitDelayTime;
        public float AtkDistance;
        public float AtkDelay;
    }

    // Other
    public static int stage = 1;
    public static float pickupDistance = 3f;
    public static GameState state = GameState.Stop;
}