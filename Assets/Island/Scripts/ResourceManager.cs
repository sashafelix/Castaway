using UnityEngine;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour
{
    private ResourceDefinition resourceDefinition;
    private List<PlacedZone> _zones;
    private ZoneRegistry _zoneRegistry;
    private void HandleResourceHarvested(ResourceNode node)
    {
        _zoneRegistry.DeRegisterPosition(node.transform.position);
    }

    public ResourceDefinition ResourceDefinition => resourceDefinition;
    public List<PlacedZone> Zones => _zones;

    public void Initialize(ZoneRegistry zoneRegistry)
    {
        _zoneRegistry = zoneRegistry;
        ResourceNode.OnResourceHarvested += HandleResourceHarvested;
    }

    private void Start()
    {
        var zoneRegistry = FindFirstObjectByType<ZoneRegistry>();
        Initialize(zoneRegistry);
    }

    private void OnDestroy()
    {
        ResourceNode.OnResourceHarvested -= HandleResourceHarvested;
    }

    public void ExecuteRespawn(ResourceNode node)
    {
        PlacedZone zone = _zoneRegistry.GetZoneAtPoint(node.transform.position);

        if (zone == null) return;

        foreach (Vector3 candidate in zone.ResourcePositions)
        {
            if (_zoneRegistry.IsPositionFree(candidate))
            {
                _zoneRegistry.RegisterPosition(candidate);
                node.Relocate(candidate);
                return;
            }
        }
    }
}