using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEditor.Build;

public class IslandGeneratorTests
{
    private List<PlacedZone> _generatedZones;
    private System.Action<List<PlacedZone>> _onGeneratedHandler;
    private IslandGenerator islandGenerator;
    private int seed;
    private IslandDefinition islandDefinition;
    private ZoneDefinition zoneDefinition;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject();
        islandGenerator = go.AddComponent<IslandGenerator>();

        seed = 12345;
        zoneDefinition = ScriptableObject.CreateInstance<ZoneDefinition>();
        zoneDefinition.SetTestData(ZoneType.Beach, 2, 10, 10, 2);
        islandDefinition = ScriptableObject.CreateInstance<IslandDefinition>();
        islandDefinition.SetTestData("TestIsland", new Vector2(100f, 100f), Biome.Tropical, new List<ZoneDefinition> { zoneDefinition });

        _onGeneratedHandler = zones => _generatedZones = zones;
        IslandGenerator.OnIslandGenerated += _onGeneratedHandler;

        islandGenerator.Initialize(seed, islandDefinition);
        islandGenerator.Generate();
    }

    [TearDown]
    public void TearDown()
    {
        IslandGenerator.OnIslandGenerated -= _onGeneratedHandler;
        Object.DestroyImmediate(islandGenerator.gameObject);
        Object.DestroyImmediate(zoneDefinition);
        Object.DestroyImmediate(islandDefinition);
    }

    [Test]
    public void PlacedZone_HasNoNullValues_ReturnsTrue()
    {
        foreach (var zone in _generatedZones)
        {
            Assert.That(zone.ZoneDefinition, Is.Not.Null);
            Assert.That(zone.Radius, Is.Not.Zero);
            Assert.That(zone.ResourcePositions, Is.Not.Null);
        }
    }

    [Test]
    public void PlacedZone_DoesEachZoneGetGenerated_ReturnsTrue()
    {
        Assert.That(_generatedZones.Count, Is.EqualTo(islandDefinition.ZoneDefinitions.Count));
    }

    [Test]
    public void ResourcePositions_AllShouldBeWihtinZoneRadius_RetursTrue()
    {
        foreach (var zone in _generatedZones)
        {
            foreach (var resourcePos in zone.ResourcePositions)
            {
                Assert.That(Vector3.Distance(resourcePos, zone.Position), Is.LessThanOrEqualTo(zone.Radius));
            }
        }
    }

    [Test]
    public void ZonePositions_AllShouldBeWihtinIslandRadius_RetursTrue()
    {
        foreach (var zone in _generatedZones)
        {
            Assert.That(zone.Position.x, Is.InRange(-50f, 50f));
            Assert.That(zone.Position.z, Is.InRange(-50f, 50f));
        }
    }

    [Test]
    public void ResourceSpacing_ResourcesShouldNotBeWithinMinSpacingDistance_RetursTrue()
    {
        foreach (var zone in _generatedZones)
        {
            for (int i = 0; i < zone.ResourcePositions.Count; i++)
            {
                for (int j = i + 1; j < zone.ResourcePositions.Count; j++)
                {
                    Assert.That(Vector3.Distance(zone.ResourcePositions[i], zone.ResourcePositions[j]), Is.GreaterThanOrEqualTo(zone.ZoneDefinition.MinSpacingPercent * zone.Radius));
                }
            }
        }
    }
}