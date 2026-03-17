using UnityEngine;
using System.Collections.Generic;

public class ZoneManager : MonoBehaviour
{
    private List<PlacedZone> _zones;


    public void Initialize(List<PlacedZone> zones)
    {
        _zones = zones;
    }

    public List<PlacedZone> Zones => _zones;

    private void HandleIslandGenerated(List<PlacedZone> zones)
    {
        Initialize(zones);
        SpawnZones(zones);
    }

    private void Start()
    {
        IslandGenerator.OnIslandGenerated += HandleIslandGenerated;
    }

    private void SpawnZones(List<PlacedZone> zones)
    {
        foreach (var zone in zones)
        {
            if (zone.ZoneDefinition.ZonePrefabs != null && zone.ZoneDefinition.ZonePrefabs.Count > 0)
            {
                GameObject prefab = zone.ZoneDefinition.ZonePrefabs[UnityEngine.Random.Range(0, zone.ZoneDefinition.ZonePrefabs.Count)];
                Instantiate(prefab, zone.Position, Quaternion.identity);
            } else
            {
                Debug.LogWarning($"Zone {zone.ZoneDefinition.ZoneType} has no prefabs assigned!");                
            }            
        }
    }
}
