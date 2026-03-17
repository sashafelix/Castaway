using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ZoneManagerTests
{
    private ZoneManager zoneManager;
    private List<PlacedZone> placedZones;
    private ZoneDefinition zoneDefinition;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject();
        zoneManager = go.AddComponent<ZoneManager>();

        zoneDefinition = ScriptableObject.CreateInstance<ZoneDefinition>();
        zoneDefinition.SetTestData(ZoneType.Beach, 2, 10, 10, 2);
        placedZones = new List<PlacedZone>();
        placedZones.Add(new PlacedZone { Position = new Vector3(50f, 0f, 50f), Radius = 10, ZoneDefinition = zoneDefinition, ResourcePositions = new List<Vector3> { new Vector3(50f, 0f, 50f), new Vector3(25f, 0f, 25f) } });

        zoneManager.Initialize(placedZones);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(zoneManager.gameObject);
        Object.DestroyImmediate(zoneDefinition);
    }

    [Test]
    public void ZoneManager_ReceivesCorrectZoneCount_ReturnsTrue()
    {
        Assert.That(zoneManager.Zones.Count, Is.EqualTo(placedZones.Count));
    }

    [Test]
    public void ZoneManager_EachZoneHasCorrectResourceCount_ReturnsTrue()
    {
        foreach (var zone in zoneManager.Zones)
        {
            Assert.That(zone.ResourcePositions.Count, Is.EqualTo(zone.ZoneDefinition.BaseResourceCount));
        }
    }

    [Test]
    public void ZoneManager_ZonePositionsAreWithinIslandBounds_ReturnsTrue()
    {
        foreach (var zone in zoneManager.Zones)
        {
            Assert.That(zone.Position.x, Is.InRange(-50f, 50f));
            Assert.That(zone.Position.z, Is.InRange(-50f, 50f));
        }
    }
}
