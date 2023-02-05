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

    public GameObject inventoryPanel;
    public GameObject resourceHudPrefab;

    Dictionary<ResourceType, float> resources = new Dictionary<ResourceType, float>();
    List<String> unlockedTrees = new List<String>();
    Dictionary<TreeItem, int> treeCounts = new Dictionary<TreeItem, int>();
    Dictionary<ResourceType, ResourceHud> resourceHuds = new Dictionary<ResourceType, ResourceHud>();
    Dictionary<ResourceType, int> resourceCapacities = new Dictionary<ResourceType, int>();
    int rootCount = 0;

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
            resourceCapacities.Add(resource, GetResourceDefinition(resource).defaultCapacity);
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

    public void AddCapacity(ResourceType resource, int amount)
    {
        resourceCapacities[resource] += amount;
    }

    public void AddRoot()
    {
        rootCount += 1;
    }

    public int GetRootCount()
    {
        return rootCount;
    }

    public void AddResource(ResourceType resource, float amount)
    {
        UpdateResource(resource, Mathf.Min(resources[resource] + amount, resourceCapacities[resource]));
    }

    public bool HasResource(ResourceCost cost)
    {
        return cost.type == ResourceType.None || resources[cost.type] >= cost.amount;
    }

    public bool PayResource(ResourceCost cost)
    {
        if (resources[cost.type] >= cost.amount)
        {
            UpdateResource(cost.type, resources[cost.type] - cost.amount);
            return true;
        }

        return false;
    }

    public void RemoveResource(ResourceCost cost)
    {
        UpdateResource(cost.type, Mathf.Max(0, resources[cost.type] - cost.amount));
    }

    void UpdateResource(ResourceType resource, float amount)
    {
        resources[resource] = amount;

        Resource resourceDefinition = GetResourceDefinition(resource);
        if (!resourceHuds.ContainsKey(resource))
        {
            GameObject hud = Instantiate(resourceHudPrefab, inventoryPanel.transform);
            ResourceHud hudScript = hud.GetComponent<ResourceHud>();

            hudScript.SetIcon(resourceDefinition.icon);
            resourceHuds.Add(resource, hudScript);
        }

        resourceHuds[resource].SetText(amount.ToString() + "/" + resourceCapacities[resource].ToString());
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
            UpdateResource(cost.type, resources[cost.type] - cost.amount);
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
