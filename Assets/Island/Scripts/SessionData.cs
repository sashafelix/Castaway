public class SessionData
{
    public static SessionData Current { get; private set; }

    public int Seed;
    public IslandDefinition IslandDefinition;
    public DifficultyProfile DifficultyProfile;

    public static void Start(int seed, IslandDefinition island, DifficultyProfile difficulty)
    {
        Current = new SessionData
        {
            Seed = seed,
            IslandDefinition = island,
            DifficultyProfile = difficulty
        };
    }
}