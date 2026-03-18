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
            if (zone.ResourcePositions != null && zone.ResourcePositions.Count > 0)
                foreach (var pos in zone.ResourcePositions)
                {

                    {
                        GameObject prefab = zone.ZoneDefinition.ZonePrefabs[UnityEngine.Random.Range(0, zone.ZoneDefinition.ZonePrefabs.Count)];
                        Instantiate(prefab, pos, Quaternion.identity);
                    }
                }
            else
            {
                Debug.LogWarning($"Resource Position {zone.ResourcePositions} is empty!");
            }
        }
    }
}
