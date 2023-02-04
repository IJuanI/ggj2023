using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{

    public static ResourceManager instance;

    public Resource[] resourceDefinitions;

    Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();

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

    public void AddResource(ResourceType resource, float amount)
    {
        resources[resource] += amount;
        Debug.Log("Resource " + resource + " has " + resources[resource] + " units");
    }

    public bool PayResource(ResourceType resource, float amount)
    {
        if (resources[resource] >= amount)
        {
            resources[resource] -= amount;
            return true;
        }

        return false;
    }

}
