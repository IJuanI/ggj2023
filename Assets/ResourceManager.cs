using System.Collections.Generic;
using UnityEngine;

struct ResourceItem
{
    public int amount;
    public Resource resource;
}

public class ResourceManager : MonoBehaviour
{

    public static ResourceManager instance;

    public Resource[] resourceDefinitions;

    Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        foreach (ResourceType resource in System.Enum.GetValues(typeof(ResourceType)))
        {
            resources.Add(resource, 0);
        }
    }

    public Resource GetResourceDefinition(ResourceType resource)
    {
        foreach (Resource resourceDefinition in resourceDefinitions)
        {
            if (resourceDefinition.type == resource)
            {
                return resourceDefinition;
            }
        }

        return new Resource();
    }

    public void AddResource(ResourceType resource, int amount)
    {
        resources[resource] += amount;
        Debug.Log("Resource " + resource + " has " + resources[resource] + " units");
    }

}
