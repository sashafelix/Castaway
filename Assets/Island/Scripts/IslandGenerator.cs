using System.Collections.Generic;
using UnityEngine;

public class IslandGenerator : MonoBehaviour
{
    // event - fires when generation is complete
    public static event System.Action<List<PlacedZone>> OnIslandGenerated;

    // private fields to store dependencies
    private int _seed;
    private IslandDefinition _islandDefinition;

    public void Initialize(int seed, IslandDefinition islandDefinition)
    {
        _seed = seed;
        _islandDefinition = islandDefinition;
    }

    private void Start()
    {
        Initialize(SessionData.Current.Seed,
                   SessionData.Current.IslandDefinition);

        Generate();
    }

    public void Generate()
    {
        System.Random rng = new System.Random(_seed);
        List<PlacedZone> placedZones = new List<PlacedZone>();

        foreach (ZoneDefinition zone in _islandDefinition.ZoneDefinitions)
        {
            // --- Position ---
            float halfX = _islandDefinition.Size.x / 2f;
            float halfZ = _islandDefinition.Size.y / 2f;
            float x = (float)(rng.NextDouble() * 2 - 1) * halfX;
            float z = (float)(rng.NextDouble() * 2 - 1) * halfZ;
            Vector3 position = new Vector3(x, 0f, z);

            // --- Radius ---
            float radius = zone.ZoneSizePercent * Mathf.Min(_islandDefinition.Size.x,
                                                             _islandDefinition.Size.y);

            // --- ResourcePositions ---
            float minSpacing = zone.MinSpacingPercent * radius;
            List<Vector3> resourcePositions = new List<Vector3>();
            int maxAttempts = zone.BaseResourceCount * 10;
            int attempts = 0;

            while (resourcePositions.Count < zone.BaseResourceCount && attempts < maxAttempts)
            {
                attempts++;
                float angle = (float)(rng.NextDouble() * 2 * Mathf.PI);
                float distance = Mathf.Sqrt((float)rng.NextDouble()) * radius;
                float rx = position.x + Mathf.Cos(angle) * distance;
                float rz = position.z + Mathf.Sin(angle) * distance;
                Vector3 candidate = new Vector3(rx, 0f, rz);

                bool tooClose = false;
                foreach (Vector3 existing in resourcePositions)
                {
                    if (Vector3.Distance(candidate, existing) < minSpacing)
                    {
                        tooClose = true;
                        break;
                    }
                }

                if (!tooClose)
                    resourcePositions.Add(candidate);
            }

            PlacedZone currentZone = new PlacedZone()
            {
                ZoneDefinition = zone,
                Position = position,
                Radius = radius,
                ResourcePositions = resourcePositions
            };

            placedZones.Add(currentZone);
        }

        OnIslandGenerated?.Invoke(placedZones);
    }
}