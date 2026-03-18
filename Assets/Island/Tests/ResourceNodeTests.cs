using NUnit.Framework;
using UnityEngine;

public class ResourceNodeTests
{
    private ResourceNode resourceNode;
    private ResourceDefinition stick;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject();
        resourceNode = go.AddComponent<ResourceNode>();

        stick = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        stick.SetTestData("Stick", 0.5f, 99, ResourceCategory.RawMaterial);

        resourceNode.Initialize(stick);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(resourceNode.gameObject);
        Object.DestroyImmediate(stick);
    }

    [Test]
    public void Initialize_SetsResourceDefinition()
    {
        Assert.That(resourceNode.ResourceDefinition, Is.EqualTo(stick));
    }

    [Test]
    public void Harvest_FiresOnResourceHarvestedEvent()
    {
        ResourceNode firedWith = null;
        void Handler(ResourceNode node) => firedWith = node;
        ResourceNode.OnResourceHarvested += Handler;

        resourceNode.Harvest();

        ResourceNode.OnResourceHarvested -= Handler;
        Assert.That(firedWith, Is.EqualTo(resourceNode));
    }

    [Test]
    public void Harvest_DeactivatesNode_AndRelocate_ReactivatesAtNewPosition()
    {
        resourceNode.Harvest();
        Assert.That(resourceNode.gameObject.activeSelf, Is.EqualTo(false));
        Vector3 newPosition = new Vector3(5f, 0f, 5f);
        resourceNode.Relocate(newPosition);
        Assert.That(resourceNode.gameObject.activeSelf, Is.EqualTo(true));
        Assert.That(resourceNode.transform.position, Is.EqualTo(newPosition));
    }
}
