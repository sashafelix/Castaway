using UnityEngine;

public class DebugBootstrapper : MonoBehaviour
{
    [SerializeField] private IslandDefinition _islandDefinition;
    [SerializeField] private DifficultyProfile _difficultyProfile;
    [SerializeField] private int _seed = 12345;

    private void Awake()
    {
        SessionData.Start(_seed, _islandDefinition, _difficultyProfile);
    }
}