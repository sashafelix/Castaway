using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class ResourceManagerTests
{
    private ResourceManager resourceManager;
    private List<PlacedZone> placedZones;
    private ZoneDefinition zoneDefinition;
    private ZoneRegistry zoneRegistry;
    private ResourceDefinition stick;
    private ResourceNode resourceNode;

    [SetUp]
    public void SetUp()
    {
        var registryGo = new GameObject();
        zoneRegistry = registryGo.AddComponent<ZoneRegistry>();

        var managerGo = new GameObject();
        resourceManager = managerGo.AddComponent<ResourceManager>();

        zoneDefinition = ScriptableObject.CreateInstance<ZoneDefinition>();
        zoneDefinition.SetTestData(ZoneType.Beach, 2, 10, 10, 2);

        placedZones = new List<PlacedZone>();
        placedZones.Add(new PlacedZone
        {
            Position = new Vector3(50f, 0f, 50f),
            Radius = 10,
            ZoneDefinition = zoneDefinition,
            ResourcePositions = new List<Vector3> {
            new Vector3(50f, 0f, 50f),
            new Vector3(25f, 0f, 25f)
        }
        });

        zoneRegistry.Initialize(placedZones);
        resourceManager.Initialize(zoneRegistry);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(resourceManager.gameObject);
        Object.DestroyImmediate(zoneRegistry.gameObject);
    }

    [Test]
    public void ResourceManager_OnResourceHarvested_PositionIsDeregisteredInZoneRegistry()
    {
        // Arrange
        Vector3 position = new Vector3(50f, 0f, 50f);
        stick = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        stick.SetTestData("Stick", 0.5f, 99, ResourceCategory.RawMaterial);

        var resourceNodeGo = new GameObject();
        resourceNode = resourceNodeGo.AddComponent<ResourceNode>();
        resourceNodeGo.transform.position = position;
        resourceNode.Initialize(stick);

        zoneRegistry.RegisterPosition(position);

        // Act
        resourceNode.Harvest();

        // Assert
        Assert.That(zoneRegistry.IsPositionFree(position), Is.True);
    }

    [Test]
    public void ResourceManager_Respawn_NewPositionIsRegisteredInZoneRegistry()
    {
        Vector3 position = new Vector3(50f, 0f, 50f);
        stick = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        stick.SetTestData("Stick", 0.5f, 99, ResourceCategory.RawMaterial);

        var resourceNodeGo = new GameObject();
        resourceNode = resourceNodeGo.AddComponent<ResourceNode>();
        resourceNodeGo.transform.position = position;
        resourceNode.Initialize(stick);

        zoneRegistry.RegisterPosition(position);

        zoneRegistry.DeRegisterPosition(position);
        resourceManager.ExecuteRespawn(resourceNode);

        Assert.That(zoneRegistry.IsPositionFree(resourceNode.transform.position), Is.False);
    }

    [Test]
    public void ResourceManager_Respawn_RelocateIsCalledWithFreePosition()
    {
        Vector3 position = new Vector3(50f, 0f, 50f);
        stick = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        stick.SetTestData("Stick", 0.5f, 99, ResourceCategory.RawMaterial);

        var resourceNodeGo = new GameObject();
        resourceNode = resourceNodeGo.AddComponent<ResourceNode>();
        resourceNodeGo.transform.position = position;
        resourceNode.Initialize(stick);
        resourceNode.Harvest(); // deactivates the node

        zoneRegistry.RegisterPosition(position);
        zoneRegistry.DeRegisterPosition(position);

        resourceManager.ExecuteRespawn(resourceNode);

        Assert.That(resourceNode.gameObject.activeSelf, Is.True);
    }

    [Test]
    public void ResourceManager_Respawn_NodeRemainsInactiveWhenZoneIsFull()
    {
        Vector3 position = new Vector3(50f, 0f, 50f);
        stick = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        stick.SetTestData("Stick", 0.5f, 99, ResourceCategory.RawMaterial);

        var resourceNodeGo = new GameObject();
        resourceNode = resourceNodeGo.AddComponent<ResourceNode>();
        resourceNodeGo.transform.position = position;
        resourceNode.Initialize(stick);
        resourceNode.Harvest(); // deactivates the node

        // Fill all positions in the zone
        zoneRegistry.RegisterPosition(new Vector3(50f, 0f, 50f));
        zoneRegistry.RegisterPosition(new Vector3(25f, 0f, 25f));

        resourceManager.ExecuteRespawn(resourceNode);

        Assert.That(resourceNode.gameObject.activeSelf, Is.False);
    }
}