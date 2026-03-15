using UnityEngine;

public enum ResourceCategory
{
    RawMaterial,
    Tool,
    Consumable,
    BuildingComponent
}

public abstract class ResourceDefinition : ScriptableObject
{
    // ── Serialized Fields ──────────────────────
    [SerializeField] private string displayName;
    [SerializeField] private float weight;
    [SerializeField] private string description;
    [SerializeField] private Sprite sprite;
    [SerializeField] private int maxStackSize;
    [SerializeField] private ResourceCategory resourceCategory;

    // ── Public Properties ──────────────────────
    public string DisplayName => displayName;
    public float Weight => weight;
    public string Description => description;
    public Sprite Sprite => sprite;
    public int MaxStackSize => maxStackSize;
    public ResourceCategory ResourceCategory => resourceCategory;

    public void SetTestData(string name, float weight, int maxStackSize, ResourceCategory category)
    {
        displayName = name;
        this.weight = weight;
        this.maxStackSize = maxStackSize;
        this.resourceCategory = category;
    }
}
