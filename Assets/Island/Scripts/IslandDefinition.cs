using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "IslandDefinition", menuName = "Castaway/Island Definition")]

public class IslandDefinition : ScriptableObject
{
    // ── Serialized Fields ──────────────────────
    [SerializeField] private string islandName;
    [SerializeField] private Vector2 size;
    [SerializeField] private Biome biome;
    [SerializeField] private List<ZoneDefinition> zoneDefinitions;

    // ── Public Properties ──────────────────────

    public string IslandName => islandName;
    public Vector2 Size => size;
    public Biome Biome => biome;
    public List<ZoneDefinition> ZoneDefinitions => zoneDefinitions;

    public void SetTestData(String name, Vector2 size, Biome biome, List<ZoneDefinition> zoneDefinitions)
    {
        islandName = name;
        this.size = size;
        this.biome = biome;
        this.zoneDefinitions = zoneDefinitions;
    }
}
