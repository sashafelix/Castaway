using NUnit.Framework;
using UnityEngine;

public class InventoryTests
{
    private Inventory inventory;
    private ResourceDefinition stick;
    private ResourceDefinition stone;
    private ResourceDefinition consumable;

    [SetUp]
    public void SetUp()
    {
        var go = new GameObject();
        inventory = go.AddComponent<Inventory>();
        inventory.Initialize();

        stick = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        stone = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        consumable = ScriptableObject.CreateInstance<ConsumableDefinition>();
        stick.SetTestData("Stick", 0.5f, 99, ResourceCategory.RawMaterial);
        stone.SetTestData("Stone", 1f, 50, ResourceCategory.RawMaterial);
        consumable.SetTestData("Consumable", 0.5f, 99, ResourceCategory.Consumable);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(inventory.gameObject);
        Object.DestroyImmediate(stick);
        Object.DestroyImmediate(stone);
        Object.DestroyImmediate(consumable);
    }

    [Test]
    public void TotalWeight_WithOneStackOfSticks_ReturnsCorrectWeight()
    {
        // Arrange
        inventory.Slots.Add(new InventorySlot { resource = stick, quantity = 10 });

        // Act
        float result = inventory.TotalWeight();

        // Assert
        Assert.AreEqual(5.0f, result);
    }

    [Test]
    public void TotalWeight_EmptyInventory_ReturnsCorrectWeight()
    {
        float result = inventory.TotalWeight();

        Assert.AreEqual(0, result);
    }

    [Test]
    public void HasAmount_DoesPlayerHaveExactAmountNeeded_ReturnTrue()
    {
        inventory.Slots.Add(new InventorySlot { resource = stick, quantity = 10 });
        Assert.True(inventory.HasAmount(stick, 10));
    }

    [Test]
    public void HasAmount_PlayerDoesNotHaveExactAmountNeeded_ReturnFalse()
    {
        inventory.Slots.Add(new InventorySlot { resource = stick, quantity = 9 });
        Assert.False(inventory.HasAmount(stick, 10));
    }

    [Test]
    public void AddItem_ToEmptyInventory_CreatesNewSlot()
    {
        int result = inventory.AddItem(stick, 5);

        Assert.AreEqual(1, inventory.Slots.Count);
        Assert.AreEqual(stick, inventory.Slots[0].resource);
        Assert.AreEqual(5, inventory.Slots[0].quantity);
        Assert.AreEqual(5, result);
    }

    [Test]
    public void AddItem_ExistingResource_StacksOntoExistingSlot()
    {
        inventory.Slots.Add(new() { resource = stick, quantity = 5 });
        int result = inventory.AddItem(stick, 5);

        Assert.AreEqual(10, inventory.Slots[0].quantity);
        Assert.AreEqual(1, inventory.Slots.Count);
        Assert.AreEqual(5, result);
    }

    [Test]
    public void AddItem_AllSlotsFull_NewResource_ReturnsZero()
    {
        for (int i = 0; i < 20; i++)
        {
            inventory.Slots.Add(new() { resource = stone, quantity = 50 });
        }
        int result = inventory.AddItem(stick, 5);

        Assert.AreEqual(20, inventory.Slots.Count);
        Assert.AreEqual(0, result);
    }

    [Test]
    public void AddItem_StackAtMaxSize_ReturnsZero()
    {
        for (int i = 0; i < 19; i++)
        {
            inventory.Slots.Add(new() { resource = stone, quantity = 50 });
        }

        inventory.Slots.Add(new() { resource = stick, quantity = 99 });

        int result = inventory.AddItem(stick, 5);
        Assert.AreEqual(0, result);
    }

    [Test]
    public void AddItem_PartialStack_ReturnsAmountAccepted()
    {
        for (int i = 0; i < 19; i++)
        {
            inventory.Slots.Add(new() { resource = stone, quantity = 50 });
        }

        inventory.Slots.Add(new() { resource = stick, quantity = 90 });

        int result = inventory.AddItem(stick, 10);

        Assert.AreEqual(9, result);
        Assert.AreEqual(99, inventory.Slots[19].quantity);
        Assert.AreEqual(20, inventory.Slots.Count);
    }

    [Test]
    public void RemoveItem_ExactAmount_RemovesSlotEntirely()
    {
        inventory.Slots.Add(new() { resource = stick, quantity = 5 });
        inventory.RemoveItem(stick, 5);

        Assert.AreEqual(0, inventory.Slots.Count);
    }

    [Test]
    public void RemoveItem_PartialAmount_ReducesSlotQuantity()
    {
        inventory.Slots.Add(new() { resource = stick, quantity = 10 });
        inventory.RemoveItem(stick, 5);

        Assert.True(inventory.HasAmount(stick, 5));
        Assert.AreEqual(1, inventory.Slots.Count);
        Assert.AreEqual(5, inventory.Slots[0].quantity);
    }

    [Test]
    public void RemoveItem_MoreThanAvailable_ReturnsFalse()
    {
        inventory.Slots.Add(new() { resource = stick, quantity = 10 });
        Assert.False(inventory.RemoveItem(stick, 15));
    }

    [Test]
    public void RemoveItem_ResourceNotInInventory_ReturnsFalse()
    {
        Assert.False(inventory.RemoveItem(stick, 10));
    }

    [Test]
    public void Sort_EmptyInventory_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => inventory.Sort());
    }

    [Test]
    public void Sort_SingleItem_RemainsUnchanged()
    {
        inventory.Slots.Add(new() { resource = stick, quantity = 1 });

        inventory.Sort();

        Assert.AreEqual(1, inventory.Slots.Count);
        Assert.AreEqual(stick, inventory.Slots[0].resource);
        Assert.AreEqual(1, inventory.Slots[0].quantity);
    }

    [Test]
    public void Sort_AlreadySorted_RemainsCorrectlyOrdered()
    {
        inventory.Slots.Add(new() { resource = consumable, quantity = 1 });
        inventory.Slots.Add(new() { resource = stick, quantity = 1 });

        inventory.Sort();

        Assert.AreEqual(2, inventory.Slots.Count);
        Assert.AreEqual(consumable, inventory.Slots[0].resource);
        Assert.AreEqual(1, inventory.Slots[0].quantity);
        Assert.AreEqual(stick, inventory.Slots[1].resource);
        Assert.AreEqual(1, inventory.Slots[1].quantity);
    }

    [Test]
    public void Sort_MixedCategories_GroupsByCategory()
    {
        inventory.Slots.Add(new() { resource = stick, quantity = 1 });
        inventory.Slots.Add(new() { resource = consumable, quantity = 1 });

        inventory.Sort();

        Assert.AreEqual(2, inventory.Slots.Count);
        Assert.AreEqual(consumable, inventory.Slots[0].resource);
        Assert.AreEqual(1, inventory.Slots[0].quantity);
        Assert.AreEqual(stick, inventory.Slots[1].resource);
        Assert.AreEqual(1, inventory.Slots[1].quantity);
    }

    [Test]
    public void Sort_WithinCategory_OrdersByDisplayName()
    {
        inventory.Slots.Add(new() { resource = stone, quantity = 1 });
        inventory.Slots.Add(new() { resource = stick, quantity = 1 });

        inventory.Sort();

        Assert.AreEqual(2, inventory.Slots.Count);
        Assert.AreEqual(stick, inventory.Slots[0].resource);
        Assert.AreEqual(1, inventory.Slots[0].quantity);
        Assert.AreEqual(stone, inventory.Slots[1].resource);
        Assert.AreEqual(1, inventory.Slots[1].quantity);

    }

    [Test]
    public void Sort_SameNameDifferentWeight_OrdersByWeight()
    {
        var heavyStick = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        var lightStick = ScriptableObject.CreateInstance<RawMaterialDefinition>();
        heavyStick.SetTestData("stick", 2f, 99, ResourceCategory.RawMaterial);
        lightStick.SetTestData("stick", 0.5f, 99, ResourceCategory.RawMaterial);

        inventory.Slots.Add(new() { resource = heavyStick, quantity = 1 });
        inventory.Slots.Add(new() { resource = lightStick, quantity = 1 });

        inventory.Sort();

        Assert.AreEqual(2, inventory.Slots.Count);
        Assert.AreEqual(lightStick, inventory.Slots[0].resource);
        Assert.AreEqual(1, inventory.Slots[0].quantity);
        Assert.AreEqual(heavyStick, inventory.Slots[1].resource);
        Assert.AreEqual(1, inventory.Slots[1].quantity);

        Object.DestroyImmediate(lightStick);
        Object.DestroyImmediate(heavyStick);
    }

    [Test]
    public void Sort_Fires_OnInventoryChanged()
    {
        inventory.Slots.Add(new() { resource = stick, quantity = 1 });
        bool eventFired = false;
        inventory.OnInventoryChanged += () => eventFired = true;
        inventory.Sort();
        Assert.True(eventFired);
    }
}