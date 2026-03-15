using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyProfile", menuName = "Castaway/Difficulty Profile")]
public class DifficultyProfile : ScriptableObject
{
    // ── Serialized Fields ──────────────────────
    [SerializeField] private ResourceDensity resourceDensity;
    [SerializeField] private RespawnRate respawnRate;
    [SerializeField] private WaterAvailability waterAvailability;
    [SerializeField] private Hazards hazards;
    [SerializeField] private int maxTransferSlots;

    // ── Public Properties ──────────────────────
    public ResourceDensity ResourceDensity => resourceDensity;
    public RespawnRate RespawnRate => respawnRate;
    public WaterAvailability WaterAvailability => waterAvailability;
    public Hazards Hazards => hazards;
    public int MaxTransferSlots => maxTransferSlots;
}
