using UnityEngine;
using System;
using System.Collections.Generic;

public class ResourceNode : MonoBehaviour
{
    private ResourceDefinition resourceDefinition;

    public ResourceDefinition ResourceDefinition => resourceDefinition;

    public static event Action<ResourceNode> OnResourceHarvested;

    public void Initialize(ResourceDefinition resourceDefinition)
    {
        this.resourceDefinition = resourceDefinition;
    }

    public void Harvest()
    {
        gameObject.SetActive(false);
        OnResourceHarvested?.Invoke(this);
    }

    public void Relocate(Vector3 vector3)
    {
        gameObject.SetActive(true);
        transform.position = vector3;
    }
}