using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ZoneRegistryTests
{
    private ZoneRegistry zoneRegistry;
    private List<PlacedZone> placedZones;
    private ZoneDefinition zoneDefinition;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject();
        zoneRegistry = go.AddComponent<ZoneRegistry>();

        zoneDefinition = ScriptableObject.CreateInstance<ZoneDefinition>();
        zoneDefinition.SetTestData(ZoneType.Beach, 2, 10, 10, 2);
        placedZones = new List<PlacedZone>();
        placedZones.Add(new PlacedZone { Position = new Vector3(50f, 0f, 50f), Radius = 10, ZoneDefinition = zoneDefinition, ResourcePositions = new List<Vector3> { new Vector3(50f, 0f, 50f), new Vector3(25f, 0f, 25f) } });

        zoneRegistry.Initialize(placedZones);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(zoneRegistry.gameObject);
        Object.DestroyImmediate(zoneDefinition);
    }

    [Test]
    public void ZoneRegistry_Initialize_ZonesContainsExpectedPlacedZones()
    {
        Assert.That(zoneRegistry.Zones.Count, Is.EqualTo(placedZones.Count));

    }

    [Test]
    public void ZoneRegistry_RegisterPosition_PositionIsOccupied()
    {
        Vector3 position = new Vector3(10f, 0f, 10f);
        zoneRegistry.RegisterPosition(position);
        Assert.That(zoneRegistry.IsPositionFree(position), Is.False);
    }

    [Test]
    public void ZoneRegistry_DeregisterPosition_PositionIsFree()
    {
        Vector3 position = new Vector3(10f, 0f, 10f);
        zoneRegistry.RegisterPosition(position);
        zoneRegistry.DeRegisterPosition(position);
        Assert.That(zoneRegistry.IsPositionFree(position), Is.True);
    }

    [Test]
    public void ZoneRegistry_IsPositionFree_ReturnsTrueForUnregisteredPosition()
    {
        Assert.That(zoneRegistry.IsPositionFree(new Vector3(0f, 0f, 0f)), Is.True);
    }

    [Test]
    public void ZoneRegistry_IsPositionFree_ReturnsFalseForOccupiedPosition()
    {
        zoneRegistry.RegisterPosition(new Vector3(1f, 0f, 1f));
        Assert.That(zoneRegistry.IsPositionFree(new Vector3(1f, 0f, 1f)), Is.False);
    }

    [Test]
    public void ZoneRegistry_GetZoneAtPoint_ReturnsCorrectZoneForPointInsideBoundary()
    {
        Assert.That(zoneRegistry.GetZoneAtPoint(new Vector3(50f, 0f, 50f)), Is.EqualTo(placedZones[0]));
    }

    [Test]
    public void ZoneRegistry_GetZoneAtPoint_ReturnsNullForPointOutsideAllZones()
    {
        Assert.That(zoneRegistry.GetZoneAtPoint(new Vector3(0f, 0f, 0f)), Is.EqualTo(null));
    }

    [Test]
    public void ZoneRegistry_GridConversion_WorldPositionsRoundingToSameCellAreIdentical()
    {
        Vector3 posA = new Vector3(10.4f, 0f, 10.4f);
        Vector3 posB = new Vector3(10.2f, 0f, 10.2f);
        zoneRegistry.RegisterPosition(posA);
        Assert.That(zoneRegistry.IsPositionFree(posB), Is.False);
    }
}
