using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions.Must;

public class ZoneRegistry : MonoBehaviour
{
    private List<PlacedZone> _zones;

    private HashSet<Vector2Int> _occupiedPositions;

    public void Initialize(List<PlacedZone> zones)
    {
        _zones = zones;
        _occupiedPositions = new HashSet<Vector2Int>();
    }

    public List<PlacedZone> Zones => _zones;

    public void RegisterPosition(Vector3 position)
    {
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.z)
            );
        _occupiedPositions.Add(gridPos);
    }

    public void DeRegisterPosition(Vector3 position)
    {
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.z)
            );
        _occupiedPositions.Remove(gridPos);
    }

    public bool IsPositionFree(Vector3 position)
    {
        Vector2Int gridPos = new Vector2Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.z)
            );
        return !_occupiedPositions.Contains(gridPos);
    }

    public PlacedZone GetZoneAtPoint(Vector3 position)
    {
        foreach (var zone in _zones)
        {
            if (Vector3.Distance(position, zone.Position) < zone.Radius)
            {
                return zone;
            }
        }
        return null;
    }
}