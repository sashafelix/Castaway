using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZoneDefinition", menuName = "Castaway/Zone Definition")]
public class ZoneDefinition : ScriptableObject
{
    // ── Serialized Fields ──────────────────────
    [SerializeField] private ZoneType zoneType;
    [SerializeField] private int baseResourceCount;
    [SerializeField] private float zoneSizePercent;
    [SerializeField] private float minSpacingPercent;
    [SerializeField] private int maxZoneCount;
    [SerializeField] private List<ResourceDefinition> spawnableResources;
    [SerializeField] private List<GameObject> zonePrefabs;

    // ── Public Properties ──────────────────────

    public ZoneType ZoneType => zoneType;
    public int BaseResourceCount => baseResourceCount;
    public float ZoneSizePercent => zoneSizePercent;
    public float MinSpacingPercent => minSpacingPercent;
    public int MaxZoneCount => maxZoneCount;
    public List<ResourceDefinition> SpawnableResources => spawnableResources;
    public List<GameObject> ZonePrefabs => zonePrefabs;

    public void SetTestData(ZoneType type, int resourceCount, float sizePercent, float spacingPercent, int zoneCount)
    {
        zoneType = type;
        baseResourceCount = resourceCount;
        zoneSizePercent = sizePercent;
        minSpacingPercent = spacingPercent;
        maxZoneCount = zoneCount;
    }

}
