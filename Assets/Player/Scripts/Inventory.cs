using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Inventory : MonoBehaviour
{
    public event System.Action OnInventoryChanged;
    private List<InventorySlot> slots;
    private int maxSlots;
    public List<InventorySlot> Slots => slots;

    public void Initialize(int maxSlots = 20)
    {
        this.maxSlots = maxSlots;
        slots = new List<InventorySlot>();
    }
    public int AddItem(ResourceDefinition resource, int amount)
    {

        for (int i = 0; i < slots.Count; i++)
        {
            int slotFreeSpace = resource.MaxStackSize - slots[i].quantity;
            if (slots[i].resource == resource && amount <= slotFreeSpace)
            {
                InventorySlot slot = slots[i];
                slot.quantity += amount;
                slots[i] = slot;
                OnInventoryChanged?.Invoke();
                return amount;
            }
            else if (slots[i].resource == resource && amount > slotFreeSpace)
            {
                InventorySlot slot = slots[i];
                slot.quantity += slotFreeSpace;
                slots[i] = slot;
                OnInventoryChanged?.Invoke();
                return slotFreeSpace;
            }
        }

        if (slots.Count < maxSlots)
        {
            InventorySlot newSlot = new() { resource = resource, quantity = amount };
            slots.Add(newSlot);
            OnInventoryChanged?.Invoke();
            return amount;
        }
        return 0;
    }

    public bool RemoveItem(ResourceDefinition resource, int amount)
    {
        if (!HasAmount(resource, amount))
        {
            return false;
        }

        int remaining = amount;
        for (int i = slots.Count - 1; i >= 0; i--)
        {
            if (slots[i].resource == resource)
            {
                int takeFromSlot = Mathf.Min(slots[i].quantity, remaining);

                remaining -= takeFromSlot;

                InventorySlot updated = slots[i];
                updated.quantity -= takeFromSlot;

                if (updated.quantity <= 0)
                {
                    slots.RemoveAt(i);
                }
                else
                {
                    slots[i] = updated;
                }

                if (remaining <= 0)
                {
                    break;
                }
            }
        }

        OnInventoryChanged?.Invoke();
        return true;
    }


    public bool HasAmount(ResourceDefinition resource, int amount)
    {
        int resourceCount = 0;
        foreach (var slot in slots)
        {
            if (slot.resource == resource)
            {
                resourceCount += slot.quantity;
            }
        }
        return resourceCount >= amount;
    }

    public void Sort()
    {
        slots = slots.OrderBy(slot => slot.resource.ResourceCategory.ToString())
             .ThenBy(slot => slot.resource.DisplayName.ToString())
             .ThenBy(slot => slot.resource.Weight)
             .ToList();

        OnInventoryChanged?.Invoke();
    }

    public float TotalWeight()
    {
        float totalWeight = 0;
        foreach (var slot in slots)
        {
            totalWeight += slot.resource.Weight * slot.quantity;
        }
        return totalWeight;
    }
}