using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ResourceCost {
    public ResourceType type;
    public float amount;

    public ResourceCost(ResourceType type, float amount) {
        this.type = type;
        this.amount = amount;
    }
}

public class ResourceManager : MonoBehaviour
{

    public static ResourceManager instance;

    public Resource[] resourceDefinitions;

    Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();
    List<String> unlockedTrees = new List<String>();
    Dictionary<TreeItem, int> treeCounts = new Dictionary<TreeItem, int>();

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

    public void AddTree(TreeItem tree)
    {
        if (!treeCounts.ContainsKey(tree))
        {
            treeCounts.Add(tree, 0);
        }

        treeCounts[tree] += 1;
    }

    public int GetTreeCount(TreeItem tree)
    {
        if (!treeCounts.ContainsKey(tree))
        {
            return 0;
        }

        return treeCounts[tree];
    }

    public void AddResource(ResourceType resource, float amount)
    {
        resources[resource] += amount;
        Debug.Log("Resource " + resource + " has " + resources[resource] + " units");
    }

    public bool HasResource(ResourceCost cost)
    {
        return cost.type == ResourceType.None || resources[cost.type] >= cost.amount;
    }

    public bool PayResource(ResourceCost cost)
    {
        if (resources[cost.type] >= cost.amount)
        {
            resources[cost.type] -= cost.amount;
            return true;
        }

        return false;
    }

    public void RemoveResource(ResourceCost cost)
    {
        resources[cost.type] = Mathf.Max(0, resources[cost.type] - cost.amount);
    }

    public bool PayResources(ResourceCost[] costs)
    {

        foreach (ResourceCost cost in costs)
        {
            if (resources[cost.type] < cost.amount)
            {
                return false;
            }
        }


        foreach (ResourceCost cost in costs)
        {
            resources[cost.type] -= cost.amount;
        }

        return true;
    }

    public bool IsTreeUnlocked(String id)
    {
        return unlockedTrees.Contains(id);
    }

    public void UnlockTree(String id)
    {
        unlockedTrees.Add(id);
    }

}
